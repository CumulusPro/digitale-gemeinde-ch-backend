using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;
using Peritos.Common.Abstractions.Paging;
using System.Text;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing form template operations including creation, updates, deletion, and retrieval.
/// </summary>
public class FormTemplateService : IFormTemplateService
{
    private readonly IFormTemplateRepository _formTemplateRepository;
    private readonly IMapper _mapper;
    private readonly IAzureBlobService _azureBlobService;

    public FormTemplateService(IFormTemplateRepository formTemplateRepository, IMapper mapper, IAzureBlobService azureBlobService)
    {
        _formTemplateRepository = formTemplateRepository;
        _mapper = mapper;
        _azureBlobService = azureBlobService;
    }

    /// <summary>
    /// Creates a new form template with version 1 and stores it in blob storage.
    /// </summary>
    /// <param name="formTemplate">The form template request containing template details</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The created form template</returns>
    public async Task<FormTemplate> CreateFormTemplate(CreateFormTemplateRequest formTemplate, int? tenantId)
    {
        formTemplate.TenantId = tenantId;        

        var datamodel = _mapper.Map<Data.Models.FormTemplate>(formTemplate);
        datamodel.Id = Guid.NewGuid().ToString();
        datamodel.Version = 1;
        datamodel.StorageUrl = $"Templates/{datamodel.Id}/v{datamodel.Version}.json";

        var created = await _formTemplateRepository.CreateFormTemplateAsync(datamodel);

        var jsonObjectString = JsonConvert.SerializeObject(formTemplate);
        var jsonObjectBytes = Encoding.UTF8.GetBytes(jsonObjectString);
        using (var memoryStream = new MemoryStream(jsonObjectBytes))
        {
            memoryStream.Position = 0;
            await _azureBlobService.UploadFile(created.StorageUrl, memoryStream);
        }
        
        return _mapper.Map<FormTemplate>(created);
    }

    /// <summary>
    /// Deletes a form template and its associated storage folder.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template to delete</param>
    public async Task DeleteFormTemplate(string formTemplateId)
    {
        await _azureBlobService.DeleteFolder($"Templates/{formTemplateId}");
        await _formTemplateRepository.DeleteFormTemplateAsync(formTemplateId);
    }

    /// <summary>
    /// Retrieves a form template by its unique identifier.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template</param>
    /// <returns>The form template if found</returns>
    /// <exception cref="FileNotFoundException">Thrown when the form template is not found</exception>
    public async Task<FormTemplate> GetFormTemplate(string formTemplateId)
    {
        var template = await _formTemplateRepository.GetFormTemplate(formTemplateId)
            ?? throw new FileNotFoundException($"FormTemplate not found: {formTemplateId}");

        var blobPath = !string.IsNullOrWhiteSpace(template.StorageUrl)
                        ? template.StorageUrl
                        : $"Templates/{formTemplateId}/v{template.Version}.json";

        var blobContent = await _azureBlobService.GetFile(blobPath)
            ?? throw new FileNotFoundException($"FormTemplate not found at: {blobPath}");

        return JsonConvert.DeserializeObject<FormTemplate>(blobContent, GetSerializationSettings());
    }

    /// <summary>
    /// Updates an existing form template, creating a new version and storing it in blob storage.
    /// </summary>
    /// <param name="formTemplateId">The unique identifier of the form template to update</param>
    /// <param name="formTemplate">The updated form template data</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>The updated form template</returns>
    /// <exception cref="FileNotFoundException">Thrown when the form template is not found</exception>
    public async Task<FormTemplate> UpdateFormTemplate(string formTemplateId, FormTemplate formTemplate, int tenantId)
    {
        formTemplate.TenantId = tenantId;

        var existing = await _formTemplateRepository.GetFormTemplate(formTemplateId)
            ?? throw new FileNotFoundException($"FormTemplate not found: {formTemplateId}");

        var datamodel = _mapper.Map<Data.Models.FormTemplate>(formTemplate);
        var newVersion = existing.Version + 1;
        datamodel.Version = newVersion;
        datamodel.StorageUrl = $"Templates/{formTemplateId}/v{newVersion}.json";

        await _formTemplateRepository.UpdateFormTemplateAsync(formTemplateId, datamodel);

        var jsonObjectString = JsonConvert.SerializeObject(formTemplate);
        var jsonObjectBytes = Encoding.UTF8.GetBytes(jsonObjectString);
        using (var memoryStream = new MemoryStream(jsonObjectBytes))
        {
            memoryStream.Position = 0;
            await _azureBlobService.UploadFile(datamodel.StorageUrl, memoryStream);
        }

        return _mapper.Map<FormTemplate>(datamodel);
    }

    /// <summary>
    /// Searches for form templates based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria</param>
    /// <returns>A paged response containing matching form templates</returns>
    public async Task<PagingResponse<FormTemplate>> SearchFormTemplate(SearchRequest searchRequest)
    {
        var datamodel = _mapper.Map<Data.Models.SearchRequest>(searchRequest);
        var updated = await _formTemplateRepository.SearchFormTemplatesAsync(datamodel);
        return _mapper.Map<PagingResponse<FormTemplate>>(updated);
    }

    private JsonSerializerSettings GetSerializationSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented
        };
    }
}
