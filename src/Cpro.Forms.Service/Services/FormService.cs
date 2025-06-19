using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.SendGrid;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Integration.Straatos.Services;
using Cpro.Forms.Service.Configuration;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Models.Payment;
using Newtonsoft.Json;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing form operations including data retrieval, submission, and navigation.
/// </summary>
public class FormService : IFormService
{
    private readonly IStraatosApiService _straatosApiService;
    private readonly IAzureBlobService _azureBlobService;
    private readonly IFormDesignerService _formDesignerService;
    private readonly IPaymentService _paymentService;
    private readonly ISendGridService _sendgridService;
    private readonly IFormDataRepository _formDataRepository;
    private readonly IMapper _mapper;
    private readonly IRequestContext _requestContext;
    private readonly IServiceConfig _serviceConfig;

    public FormService(
        IStraatosApiService straatosApiService,
        IAzureBlobService azureBlobService, 
        IFormDataRepository formDataRepository,
        IPaymentService paymentService,
        IMapper mapper,
        ISendGridService sendgridService,
        IRequestContext requestContext,
        IFormDesignerService formDesignerService,
        IServiceConfig serviceConfig)
    {
        _straatosApiService = straatosApiService;
        _azureBlobService = azureBlobService;
        _formDesignerService = formDesignerService;
        _mapper = mapper;
        _requestContext = requestContext;
        _paymentService = paymentService;
        _sendgridService = sendgridService;
        _formDataRepository = formDataRepository;
        _serviceConfig = serviceConfig;
    }

    /// <summary>
    /// Retrieves form data based on form ID and optional document ID.
    /// If document ID is provided, returns existing form data; otherwise returns form definition for new submissions.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="documentId">Optional document identifier for existing form data</param>
    /// <returns>A document response containing form data or definition</returns>
    public async Task<DocumentResponse> GetFormDataAsync(string formId, int? tenantId, string documentId)
    {
        var tenant = Convert.ToInt32(tenantId);

        if (!string.IsNullOrWhiteSpace(documentId))
        {
            var formData = await _formDataRepository.GetFormDataByDocumentId(documentId);
            Console.WriteLine(JsonConvert.SerializeObject(formData));

            var document = await _azureBlobService.GetFile($"{documentId}.json")
                ?? throw new FileNotFoundException($"FormDefinition not found for DocumentId: {documentId} ");

            var documentResponse = JsonConvert.DeserializeObject<DocumentResponse>(document, GetSerializationSettings());
            documentResponse.header = new Header() { showHeader = false };
            documentResponse.footer = new Footer() { showFooter = false };
            documentResponse.ShowSubmit = false;
            documentResponse.State = formData.Status;

            documentResponse.Fields.ForEach(f => f.ReadOnly = true);

            var indexToRemove = documentResponse.formStatesConfig?.FindIndex(s => s?.value == documentResponse.State) ?? -1;
            if (indexToRemove >= 0)
            {
                documentResponse.formStatesConfig.RemoveAt(indexToRemove);
            }

            return documentResponse;
        }

        var currentUser = await GetCurrentUserDetails();
        Console.WriteLine($"Current User Id: {currentUser.Id}");

        var documenTypeResponse = await _formDesignerService.GetFormDefinitionResponseAsync(formId, tenantId);
        documenTypeResponse.isFormConfigDisabled = !documenTypeResponse?.designers?.Contains(currentUser.Emails.FirstOrDefault());

        return documenTypeResponse;
    }

    /// <summary>
    /// Retrieves form navigation data including form counts by status for a specific tenant.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A list of form navigation items with status-based counts</returns>
    public async Task<List<FormNavigation>> GetFormNavigationAsync(int? tenantId)
    {
        var tenant = Convert.ToInt32(tenantId);

        // Step 1: Get form designs
        var formDesigns = await _formDesignerService.GetFormDesignsByTenantIdAsync(tenant);

        // Step 2: Prepare initial response list
        var response = formDesigns.Select(x => new FormNavigation
        {
            Id = x.id,
            Name = x.Name,
            Count = 0, // default
            Request = new Request
            {
                FormId = x.id
            }
        }).ToList();

        // Step 3: Get form data and group by FormId
        var formData = await _formDataRepository.GetFormDatasByTenantId(tenant);
        var grouped = formData
                      .GroupBy(x => new { x.FormId, x.Status })
                      .Select(g => new
                      {
                          g.Key.FormId,
                          g.Key.Status,
                          Count = g.Count()
                      })
                      .ToList();

        // Step 4: Map grouped data into FormStatusNavigation
        foreach (var group in grouped)
        {
            var formNav = response.FirstOrDefault(f => f.Request?.FormId == group.FormId);
            if (formNav != null)
            {
                formNav.FormNavigations ??= new List<FormStatusNavigation>();

                formNav.FormNavigations.Add(new FormStatusNavigation
                {
                    Id = $"{formNav.Id}-{group.Status}",
                    Name = group.Status,
                    Count = group.Count,
                    Request = new Request
                    {
                        FormId = group.FormId,
                        Status = group.Status,
                    }
                });

                formNav.Count += group.Count; // Sum all statuses' counts
            }
        }

        return response;
    }

    /// <summary>
    /// Searches form data based on specified criteria with pagination support.
    /// </summary>
    /// <param name="searchRequest">The search criteria including keywords, form ID, and status</param>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>A paged response containing matching form data</returns>
    public async Task<PagingResponse<FormData>> SearchFormData(FormSearchRequest searchRequest, int tenantId)
    {
        var datamodel = _mapper.Map<Data.Models.FormSearchRequest>(searchRequest);
        var updated = await _formDataRepository.SearchFormDatasAsync(datamodel, tenantId);
        return _mapper.Map<PagingResponse<FormData>>(updated);
    }

    /// <summary>
    /// Submits form data, handling payment processing, email notifications, and data persistence.
    /// </summary>
    /// <param name="jsonData">The form data to submit</param>
    /// <param name="origin">The origin URL for payment processing</param>
    /// <returns>A form response containing the document ID</returns>
    public async Task<FormResponse> SubmitTaskAsync(dynamic jsonData, string origin)
    {
        jsonData.data.FormId = jsonData.formId;
        var formId = jsonData.formId.ToString();
        var tenantId = Convert.ToInt32(jsonData.tenantId.ToString());

        DocumentResponse documentResponse = await _formDesignerService.GetFormDefinitionResponseAsync(formId, tenantId);
        jsonData.data.FormTitle = documentResponse.Name;
        string? documentId = await GetDocumentId(jsonData, documentResponse);

        var paymentResponse = await HandlePaymentIfNeeded(jsonData, origin, documentResponse, documentId);

        if (paymentResponse != null)
            return paymentResponse;

        await SendEmailIfConfigured(documentResponse, documentId);

        await SaveFormData(jsonData, documentResponse, documentId);

        return new FormResponse() { documentId = documentId };
    }

    private async Task<FormResponse?> HandlePaymentIfNeeded(dynamic jsonData, string origin, DocumentResponse documentResponse, string documentId)
    {
        var paymentConfig = documentResponse.paymentConfig;
        if (paymentConfig == null || paymentConfig.isEnabled == 0)
            return null;

        if (paymentConfig.isEnabled == 2 && paymentConfig.PaymentCondition != null)
        {
            dynamic combinedObject, combinedObjectWithFiles;
            var combinedDict = _straatosApiService.GetIndexFieldObject(jsonData, documentResponse, out combinedObject, out combinedObjectWithFiles);
            var conditionField = paymentConfig.PaymentCondition.conditionField;
            var fieldValue = (combinedDict as IDictionary<string, object>)[conditionField]?.ToString();
            var conditionValue = paymentConfig.PaymentCondition.conditionValue;

            var conditionMatched = paymentConfig.PaymentCondition.operatorField switch
            {
                "=" => fieldValue == conditionValue,
                "!=" => fieldValue != conditionValue,
                "startswith" => fieldValue?.StartsWith(conditionValue) ?? false,
                "contains" => fieldValue?.Contains(conditionValue) ?? false,
                "substring" => conditionValue?.Contains(fieldValue) ?? false,
                _ => false
            };

            if (!conditionMatched)
                return new FormResponse { documentId = documentId };
        }

        var paymentUrl = await _paymentService.CreatePaymentRequest(new PaymentRequest
        {
            amount = paymentConfig.amount.ToString(),
            currency = paymentConfig.currency,
            apikey = paymentConfig.apiKey,
            referenceId = documentId,
            formId = jsonData.formId,
            tenantId = jsonData.tenantId,
            baseurl = origin,
            instance = paymentConfig.instance
        });

        return new FormResponse { documentId = documentId, redirectUrl = paymentUrl };
    }

    private async Task SendEmailIfConfigured(DocumentResponse documentResponse, string documentId)
    {
        if (documentResponse.emailNotificationConfig.notificationEnabled ?? false)
        {
            var emailBody = documentResponse.emailNotificationConfig.emailsBody?
                .Replace("documenturl", $"{documentResponse.emailNotificationConfig.baseurl}/new-task-details?newUI=true&documentId={documentId}&isArchiveTask=false");

            await _sendgridService.SendEmail(new Integration.SendGrid.Dto.EmailDto()
            {
                HtmlContent = emailBody,
                ToEmail = documentResponse.emailNotificationConfig.emailsTo,
                Subject = documentResponse.emailNotificationConfig.emailsSubject,
            });
        }
    }

    private async Task SaveFormData(dynamic jsonData, DocumentResponse documentResponse, string documentId)
    {
        var formData = new Data.Models.FormData()
        {
            Id = Guid.NewGuid().ToString(),
            Name = documentResponse.Name,
            DocumentId = documentId,
            Status = documentResponse.formStatesConfig?.FirstOrDefault()?.value ?? "Neu",
            TenantId = jsonData.tenantId,
            FormId = jsonData.formId,
            TenantName = "",
            StorageUrl = $"{documentId}.json",
            SubmittedDate = DateTime.Now
        };

        await _formDataRepository.CreateFormDataAsync(formData);
    }

    /// <summary>
    /// Updates the status of an existing form document.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <param name="formId">The form identifier</param>
    /// <param name="documentId">The document identifier</param>
    /// <param name="status">The new status to set</param>
    /// <returns>The updated document response</returns>
    public async Task<DocumentResponse> UpdateFormStatus(int? tenantId, string formId,string documentId, string status)
    {
        var formData = await _formDataRepository.GetFormDataByDocumentId(documentId);
        Console.WriteLine("formData: " + formData.FormId);

        formData.Status = status;
        await _formDataRepository.UpdateFormDataAsync(formData.Id, formData);
        Console.WriteLine("formData Updated: " + formData.FormId);

        return await GetFormDataAsync(formData.FormId, tenantId, documentId);
    }

    private JsonSerializerSettings GetSerializationSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented
        };
    }

    private async Task<CurrentUser?> GetCurrentUserDetails()
    {
        if (_serviceConfig.UseStraatos)
        {
            var currentUserResponse = await _straatosApiService.GetCurrentUser(_requestContext.Token);
            var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserResponse);
            return currentUser;
        }
        else
        {
            CurrentUser currentUser = new CurrentUser { Id = _requestContext.UserId.Value, Emails = new List<string> { _requestContext.UserEmail } };
            return currentUser;
        }
    }

    private async Task<string?> GetDocumentId(dynamic jsonData, DocumentResponse documentResponse)
    {
        if (_serviceConfig.UseStraatos)
        {
            return await _straatosApiService.UploadSimple(jsonData, documentResponse);
        }
        else
        {
            return await _formDataRepository.GetNextSequenceDocumentId();
        }
    }
}
