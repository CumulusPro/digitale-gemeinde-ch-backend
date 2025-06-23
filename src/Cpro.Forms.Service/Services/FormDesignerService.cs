using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;
using Peritos.Common.Abstractions.Paging;
using System.Text;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing form design operations including creation, updates, duplication, and retrieval of form definitions.
/// </summary>
public class FormDesignerService : IFormDesignerService
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly IMapper _mapper;
    private readonly IFormDesignRepository _formDesignRepository;
    private readonly IFormDesignerHistoryService _formDesignHistoryService;

    public FormDesignerService(
        IFormDesignRepository formDesignRepository,
        IFormDesignerHistoryService formDesignHistoryService,
        IMapper mapper,
        IAzureBlobService azureBlobService)
    {
        _mapper = mapper;
        _azureBlobService = azureBlobService;
        _formDesignRepository = formDesignRepository;
        _formDesignHistoryService = formDesignHistoryService;
    }

    /// <summary>
    /// Creates or updates a form definition with the specified fields and configuration.
    /// </summary>
    /// <param name="fieldRequest">The field request containing form configuration</param>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="email">The email of the user creating/updating the form</param>
    /// <returns>The created or updated form design</returns>
    public async Task<FormDesign> CreateFormDefinitionAsync(FieldRequest fieldRequest, string formId, int? tenantId, string email, bool isImported = false)
    {
        var tenant = Convert.ToInt32(tenantId);
        var formDesign = await _formDesignRepository.GetFormDesign(formId, tenant);

        formDesign = formDesign == null
                        ? await CreateFormDesignAsync(fieldRequest, formId, email, tenant, isImported)
                        : await UpdateFormDesignAsync(fieldRequest, formId, formDesign);

        if (fieldRequest.Fields.Any())
        {
            var lastId = fieldRequest.Fields.Max(x => x.Id ?? 0);

            foreach (var field in fieldRequest.Fields.Where(f => f.Id == null))
            {
                Field newField = new Field
                {
                    Name = field.Name,
                    Label = field.DisplayName,
                    Mandatory = field.IsRequired ?? false,
                    FieldType = MapFieldType(field.Datatype),
                };

                field.Id = ++lastId;
                field.LookupValues = field.LookupValues != null
                                     ? field.LookupValues.Select((x, y) => new LookupValues { displayOrder = y++, displayValue = x.value, value = x.value }).ToList()
                                     : field.LookupValues;
            }
        }

        formId = formDesign.Id;
        fieldRequest.Id = formDesign.FormId;

        var json = JsonConvert.SerializeObject(fieldRequest);
        var jsonBytes = Encoding.UTF8.GetBytes(json);

        using (var memoryStream = new MemoryStream(jsonBytes))
        {
            memoryStream.Position = 0;
            await _azureBlobService.UploadFile(formDesign.StorageUrl, memoryStream);
        }

        await _formDesignHistoryService.SaveFormDesignVersionHistory(formDesign);
        formDesign.StorageUrl = _azureBlobService.GetSignedUrl(formDesign.StorageUrl);

        var response = _mapper.Map<FormDesign>(formDesign);
        return response;
    }

    /// <summary>
    /// Retrieves the JSON data for a specific form by its ID.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <returns>A signed URL to access the form's JSON data</returns>
    /// <exception cref="FileNotFoundException">Thrown when the form definition is not found</exception>
    public async Task<string> GetFormDataJsonAsync(string formId)
    {
        var formDesign = await _formDesignRepository.GetFormDesignByFormId(formId)
            ?? throw new FileNotFoundException($"FormDefinition not found with FormId: {formId}");

        return _azureBlobService.GetSignedUrl(formDesign.StorageUrl);
    }

    /// <summary>
    /// Retrieves the complete form definition response including tenant-specific design configurations.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A document response containing the complete form definition</returns>
    /// <exception cref="FileNotFoundException">Thrown when the form definition is not found</exception>
    public async Task<DocumentResponse> GetFormDefinitionResponseAsync(string formId, int? tenantId)
    {
        var form = await _formDesignRepository.GetFormDesign(formId, Convert.ToInt32(tenantId))
            ?? throw new FileNotFoundException($"FormDefinition not found with FormId: {formId}");

        var blobPath = !string.IsNullOrWhiteSpace(form.StorageUrl)
                        ? form.StorageUrl
                        : $"{formId}/v{form.Version}.json";

        var formDefinition = await _azureBlobService.GetFile(blobPath)
            ?? throw new FileNotFoundException($"FormDefinition not found at: {blobPath}");

        var documentResponse = JsonConvert.DeserializeObject<DocumentResponse>(formDefinition, GetSerializationSettings());

        if (documentResponse.useTenantDesign ?? false)
        {
            var tenantIdDefinition = await _azureBlobService.GetFile($"{tenantId}/v1.json") // check whether to maintain versioning for tenants?
                ?? throw new FileNotFoundException($"FormDefinition not found with TenantId: {tenantId}");

            var tenantDesign = JsonConvert.DeserializeObject<TenantDesign>(tenantIdDefinition, GetSerializationSettings());

            documentResponse.header = tenantDesign.header;
            documentResponse.footer = tenantDesign.footer;
            documentResponse.designConfig = tenantDesign.designConfig;
        }

        GetHeaderFooterSettings(documentResponse);
        documentResponse.IsActive = form.IsActive;

        return documentResponse;
    }

    /// <summary>
    /// Retrieves all form designs for a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A list of form designs with signed URLs</returns>
    public async Task<List<FormDesign>> GetFormDesignsByTenantIdAsync(int tenantId)
    {
        var formDesigns = await _formDesignRepository.GetFormDesignsByTenantId(tenantId);

        foreach (var form in formDesigns)
        {
            form.StorageUrl = _azureBlobService.GetSignedUrl(form.StorageUrl);
        }

        return _mapper.Map<List<FormDesign>>(formDesigns);
    }

    /// <summary>
    /// Deletes a form design and its associated storage folder.
    /// </summary>
    /// <param name="formId">The unique identifier of the form to delete</param>
    /// <param name="tenantId">The tenant identifier</param>
    public async Task DeleteFormDesignAsync(string formId, int tenantId)
    {
        await _azureBlobService.DeleteFolder(formId);
        await _formDesignRepository.DeleteFormDesignAsync(formId, tenantId);
    }

    /// <summary>
    /// Creates a duplicate of an existing form definition with a new ID and timestamped name.
    /// </summary>
    /// <param name="formId">The unique identifier of the form to duplicate</param>
    /// <param name="email">The email of the user creating the duplicate</param>
    /// <returns>The duplicated form design</returns>
    /// <exception cref="FileNotFoundException">Thrown when the original form definition is not found</exception>
    public async Task<FormDesign> DuplicateFormDefinitionAsync(string formId, string email)
    {
        var originalForm = await _formDesignRepository.GetFormDesignByFormId(formId)
            ?? throw new FileNotFoundException($"Original FormDefinition not found with formId: {formId}");

        var newFormId = Guid.NewGuid().ToString();
        var newFormCount = await _formDesignRepository.GetFormDesignCountAsync() + 1;

        var duplicatedForm = new Data.Models.FormDesign
        {
            Id = newFormId,
            Name = $"{originalForm.Name}_{DateTime.Now:yyyyMMddHHmmss}",
            TenantId = originalForm.TenantId,
            FormId = newFormCount,
            TenantName = originalForm.TenantName,
            Version = 1,
            StorageUrl = $"{newFormId}/v1.json",
            Designers = originalForm.Designers?.Select(designer => new Data.Models.Designer
            {
                Email = designer.Email,
                FormDesignId = newFormId
            }).ToList() ?? new List<Data.Models.Designer>(),
            Processors = originalForm.Processors?.Select(processor => new Data.Models.Processor
            {
                Email = processor.Email,
                FormDesignId = newFormId
            }).ToList() ?? new List<Data.Models.Processor>(),
            FormStates = originalForm.FormStates?.Select(s => new Data.Models.FormStatesConfig
            { 
                Label = s.Label,
                Value = s.Value,
                FormDesignId = newFormId
            })?.ToList(),
            Tags = originalForm.Tags?.Select(tag => new Data.Models.FormDesignTag
            {
                TagId = tag.TagId,
                FormDesignId = newFormId
            }).ToList() ?? new List<Data.Models.FormDesignTag>(),
            CreatedBy = email
        };

        var savedForm = await _formDesignRepository.CreateFormDesignAsync(duplicatedForm);

        // Get fields from blob
        var originalJson = await _azureBlobService.GetFile(originalForm.StorageUrl ?? $"{formId}/v{originalForm.Version}.json");
        var originalRequest = JsonConvert.DeserializeObject<FieldRequest>(originalJson);

        // Update name and ID in duplicated request
        originalRequest.Id = savedForm.FormId;
        originalRequest.Name = duplicatedForm.Name;

        var newJson = JsonConvert.SerializeObject(originalRequest);
        var jsonBytes = Encoding.UTF8.GetBytes(newJson);

        using (var memoryStream = new MemoryStream(jsonBytes))
        {
            memoryStream.Position = 0;
            await _azureBlobService.UploadFile(savedForm.StorageUrl, memoryStream);
        }

        await _formDesignHistoryService.SaveFormDesignVersionHistory(savedForm);
        savedForm.StorageUrl = _azureBlobService.GetSignedUrl(savedForm.StorageUrl);

        return _mapper.Map<FormDesign>(savedForm);
    }

    /// <summary>
    /// Searches for form designs based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="email">logged in user's email</param>
    /// <returns>A paged response containing matching form designs</returns>
    public async Task<PagingResponse<FormDesign>> SearchFormDesignsAsync(SearchRequest searchRequest, int tenantId, string email)
    {
        var datamodel = _mapper.Map<Data.Models.SearchRequest>(searchRequest);
        var formDesigns = await _formDesignRepository.SearchFormDesignsAsync(datamodel, tenantId, email);
        return _mapper.Map<PagingResponse<FormDesign>>(formDesigns);
    }

    /// <summary>
    /// Activates or deactivates a form definition.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="isActive">Whether the form should be active</param>
    /// <param name="tenantId">The tenant identifier</param>
    public async Task ActivateFormDefinitionAsync(string formId, bool isActive, int? tenantId)
    {
        var formDesign = await _formDesignRepository.GetFormDesign(formId, tenantId ?? 0)
            ?? throw new FileNotFoundException($"FormDefinition not found with FormId: {formId}");

        formDesign.IsActive = isActive;
        await _formDesignRepository.UpdateFormDesignAsync(formId, formDesign);
    }

    /// <summary>
    /// Retrieves all distinct tag names used across form designs.
    /// </summary>
    /// <returns>A list of unique tag names</returns>
    public async Task<List<string>> GetAllDistinctTagNamesAsync()
    {
        return await _formDesignRepository.GetAllDistinctTagNamesAsync();
    }

    private string MapFieldType(string datatype)
    {
        if (datatype.ToLower().Equals("number"))
            return "Integer";
        if (datatype.ToLower().Equals("amount"))
            return "Double";
        if (datatype.ToLower().Equals("datetime"))
            return "DateTime";
        if (datatype.ToLower().Equals("date"))
            return "Date";
        if (datatype.ToLower().Equals("string"))
            if (datatype.ToLower().Equals("ahvnumber"))
                return "String";

        return string.Empty;
    }

    private JsonSerializerSettings GetSerializationSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented
        };
    }

    private void GetHeaderFooterSettings(DocumentResponse? documentResponse)
    {
        var headerSettings = new Header() { showHeader = true, content = documentResponse?.header?.content, layoutProps = new Layoutprops(), layoutType = "STANDARD" };
        var footerSettings = new Footer() { showFooter = true, content = documentResponse?.footer?.content, layoutProps = new Layoutprops1(), layoutType = "STANDARD" };

        if (string.IsNullOrWhiteSpace(documentResponse?.header?.layoutProps?.logoUrl))
        {
            if (documentResponse != null)
            {
                documentResponse.header = headerSettings;
                documentResponse.footer = footerSettings;
            }
        }
    }

    private async Task<Data.Models.FormDesign?> CreateFormDesignAsync(FieldRequest fieldRequest, string formId, string email, int tenant, bool isImported)
    {
        var newFormId = string.IsNullOrWhiteSpace(formId) ? Guid.NewGuid().ToString() : formId;
        var formCount = await _formDesignRepository.GetFormDesignCountAsync() + 1;

        var formDesign = new Data.Models.FormDesign()
        {
            Id = newFormId,
            Name = isImported ? $"{fieldRequest.Name}_import_{DateTime.Now:yyyyMMdd}" : fieldRequest.Name,
            TenantId = tenant,
            FormId = formCount,
            TenantName = string.Empty,
            Version = 1,
            StorageUrl = $"{newFormId}/v1.json",
            Designers = MapToDesigners(fieldRequest.designers, newFormId),
            Processors = MapToProcessors(fieldRequest.processors, newFormId),
            FormStates = MapToFormStates(fieldRequest.formStatesConfig, newFormId),
            CreatedBy = email,
            Tags = await GetOrCreateTagsAsync(fieldRequest.tags, newFormId)
        };

        return await _formDesignRepository.CreateFormDesignAsync(formDesign);
    }

    private async Task<Data.Models.FormDesign?> UpdateFormDesignAsync(FieldRequest fieldRequest, string formId, Data.Models.FormDesign formDesign)
    {
        formDesign.Name = fieldRequest.Name;

        // Update tags
        formDesign.Tags.Clear();
        var newTags = await GetOrCreateTagsAsync(fieldRequest.tags, formId);

        foreach (var tag in newTags)
            formDesign.Tags.Add(tag);

        var incomingDesigners = MapToDesigners(fieldRequest.designers, formId);
        var incomingProcessors = MapToProcessors(fieldRequest.processors, formId);
        var incomingStates = MapToFormStates(fieldRequest.formStatesConfig, formId);

        SyncCollection(formDesign.Processors, incomingProcessors, x => x.Email);
        SyncCollection(formDesign.Designers, incomingDesigners, x => x.Email);
        SyncCollection(formDesign.FormStates, incomingStates, x => $"{x.Label.ToLower()}|{x.Value.ToLower()}");

        formDesign.Version += 1;
        formDesign.StorageUrl = $"{formId}/v{formDesign.Version}.json";

        return await _formDesignRepository.UpdateFormDesignAsync(formId, formDesign);
    }

    private List<Data.Models.Designer> MapToDesigners(List<string> designers, string formId)
    {
        return designers.Select(designerEmail => new Data.Models.Designer
        {
            Email = designerEmail,
            FormDesignId = formId
        }).ToList();
    }

    private List<Data.Models.Processor> MapToProcessors(List<string> processors, string formId)
    {
        return processors.Select(processorEmail => new Data.Models.Processor
        {
            Email = processorEmail,
            FormDesignId = formId
        }).ToList();
    }

    private List<Data.Models.FormStatesConfig> MapToFormStates(List<Models.FormStatesConfig> formStates, string formId)
    {
        return formStates.Select(state => new Data.Models.FormStatesConfig
        {
            Label = state.label,
            Value = state.value,
            FormDesignId = formId
        }).ToList();
    }

    private async Task<List<Data.Models.FormDesignTag>> GetOrCreateTagsAsync(List<string>? tags, string formDesignId)
    {
        var result = new List<Data.Models.FormDesignTag>();
        if (tags == null || !tags.Any()) return result;

        var existingTags = await _formDesignRepository.GetTagsByNamesAsync(tags);
        var existingTagNames = existingTags.Select(t => t.TagName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Create missing tags
        var newTags = tags
            .Where(t => !existingTagNames.Contains(t))
            .Select(t => new Data.Models.Tag { TagName = t })
            .ToList();

        await _formDesignRepository.AddTagsAsync(newTags);
        existingTags.AddRange(newTags);

        foreach (var tag in existingTags)
        {
            result.Add(new Data.Models.FormDesignTag
            {
                TagId = tag.TagId,
                FormDesignId = formDesignId
            });
        }

        return result;
    }

    private void SyncCollection<T>(List<T> existing, List<T> incoming, Func<T, object> keySelector)
    {
        var existingKeys = existing.Select(keySelector).ToHashSet();
        var incomingKeys = incoming.Select(keySelector).ToHashSet();

        // Remove existing items not in incoming
        existing.RemoveAll(x => !incomingKeys.Contains(keySelector(x)));

        // Add new items not in existing
        existing.AddRange(incoming.Where(x => !existingKeys.Contains(keySelector(x))));
    }
}
