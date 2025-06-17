using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Integration.Straatos.Configuration;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Dynamic;
using System.Text;

namespace Cpro.Forms.Integration.Straatos.Services;

/// <summary>
/// Service for integrating with the Straatos API, handling document uploads, user management, and tenant operations.
/// </summary>
public class StraatosApiService : IStraatosApiService
{
    private readonly IStraatosConfiguration _configuration;
    private readonly IHttpService _httpService;
    private readonly IAzureBlobService _azureBlobService;

    public StraatosApiService(IStraatosConfiguration configuration, IHttpService httpService, IAzureBlobService azureBlobService)
    {
        _configuration = configuration;
        _httpService = httpService;
        _azureBlobService = azureBlobService;
    }

    /// <summary>
    /// Uploads form data to Straatos workflow system with file handling and document creation.
    /// </summary>
    /// <param name="jsonData">The form data to upload</param>
    /// <param name="documentResponse">The document response containing form configuration</param>
    /// <returns>The document ID returned from Straatos</returns>
    public async Task<string> UploadSimple(dynamic jsonData, DocumentResponse documentResponse)
    {
        string baseUrl = _configuration.BaseUrl;
        string workflowStepId = _configuration.WorkflowStepId;

        string initiateUrl = $"{baseUrl}/API/Upload/{jsonData.tenantId}/{workflowStepId}";
        string uploadUrl = $"{baseUrl}/API/Upload/{jsonData.tenantId}/<uploadId>/file/url?additionalDataKey=<additionalDataKey>&fileName=<filename>&fileNumber=1&visible=true";
        string completeUrl = $"{baseUrl}/API/Upload/{jsonData.tenantId}/<uploadId>/complete?async=false";
        dynamic combinedObject, combinedObjectWithFiles;
        GetIndexFieldObject(jsonData, documentResponse, out combinedObject, out combinedObjectWithFiles);


        //Phase 1: Initiate the request and get upload id
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        var initiateRequest = new { IndexFields = new { FormId = $"{jsonData.data.FormId}" } };
        var initiateContent = new StringContent(JsonConvert.SerializeObject(initiateRequest, settings), Encoding.UTF8, "application/json");
        var initiateResponse = await _httpService.PostAsync(initiateUrl, initiateContent, "Initiate");

        var uploadId = JsonConvert.DeserializeObject<string>(initiateResponse);

        // Phase 2: Get the uploadFileUrl and Upload the file
        // Create JSON Object
        var jsonObject = CreateJsonObject(combinedObject, documentResponse, uploadId, jsonData);
        var jsonObjectString = JsonConvert.SerializeObject(jsonObject);
        var jsonObjectBytes = Encoding.UTF8.GetBytes(jsonObjectString);

        // Upload JSON object
        var fileUploadUrl = $"{uploadUrl.Replace("<filename>", $"formData.json").Replace("<uploadId>", uploadId).Replace("<additionalDataKey>", $"formData")}";
        var fileUploadResponse = await _httpService.GetAsync(fileUploadUrl, "Upload");
        var uploadFileUrl = JsonConvert.DeserializeObject<string>(fileUploadResponse);

        await _azureBlobService.UploadFile(uploadFileUrl, jsonObjectBytes);

        List<FileUploadAdditionalData> fileUploads = GetFileUploads(combinedObjectWithFiles, documentResponse);
        foreach (var fileUpload in fileUploads)
        {
            var fileUploadUrl1 = $"{uploadUrl.Replace("<filename>", fileUpload.name).Replace("<uploadId>", uploadId).Replace("<additionalDataKey>", fileUpload.key)}";
            var fileUploadResponse1 = await _httpService.GetAsync(fileUploadUrl1, "Upload");
            var uploadFileUrl1 = JsonConvert.DeserializeObject<string>(fileUploadResponse1);
            var filedata = ConvertToByteArray(fileUpload.base64File);
            await _azureBlobService.UploadFile(uploadFileUrl1, filedata);
        }

        // Phase 3: Complete the upload
        var completeUploadUrl = completeUrl.Replace("<uploadId>", uploadId);
        var completeResponse = await _httpService.GetAsync(completeUploadUrl, "Complete");
        var documentId = JsonConvert.DeserializeObject<string>(completeResponse);

        jsonObject = CreateJsonObjectWithFiles(combinedObjectWithFiles, documentResponse, uploadId, jsonData);
        jsonObjectString = JsonConvert.SerializeObject(jsonObject);
        jsonObjectBytes = Encoding.UTF8.GetBytes(jsonObjectString);
        using (var memoryStream = new MemoryStream(jsonObjectBytes))
        {
            memoryStream.Position = 0;

            await _azureBlobService.UploadFile($"{documentId}.json", memoryStream);
        }

        return documentId;
    }

    /// <summary>
    /// Extracts and processes index field objects from form data, separating file uploads from regular fields.
    /// </summary>
    /// <param name="jsonData">The form data containing field values</param>
    /// <param name="documentResponse">The document response containing field definitions</param>
    /// <param name="combinedObject">Output parameter containing combined object without files</param>
    /// <param name="combinedObjectWithFiles">Output parameter containing combined object with files</param>
    /// <returns>A dictionary containing the processed index fields</returns>
    public IDictionary<string, object> GetIndexFieldObject(dynamic jsonData, DocumentResponse documentResponse, out dynamic combinedObject, out dynamic combinedObjectWithFiles)
    {
        var tabs = documentResponse.Fields.Select(x => x.TabName).Distinct();

        List<dynamic> indexFields = new List<dynamic>();

        combinedObject = new ExpandoObject();
        var combinedDict = (IDictionary<string, object>)combinedObject;

        combinedObjectWithFiles = new ExpandoObject();
        var combinedDictWithFiles = (IDictionary<string, object>)combinedObjectWithFiles;

        var data = jsonData.data;

        if (tabs.Count() > 0) {
            foreach (var tab in tabs)
            {
                indexFields.AddRange(jsonData.data[tab]);
            }
                
            foreach (var property in indexFields)
            {
                combinedDictWithFiles[property.Name] = property.Value;

                var fieldDefinition = documentResponse.Fields.FirstOrDefault(x => x.Name.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                if (fieldDefinition != null && fieldDefinition.Datatype.Equals("file", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                combinedDict[property.Name] = property.Value;
            }
        }

        combinedDict["formId"] = jsonData.data.FormId;
        return combinedDict;
    }

    /// <summary>
    /// Converts base64 file data to byte array for file upload processing.
    /// </summary>
    /// <param name="filedata">The base64 encoded file data</param>
    /// <returns>Byte array representation of the file data</returns>
    private byte[] ConvertToByteArray(string filedata)
    {
        if (!string.IsNullOrWhiteSpace(filedata)) 
        {
            string base64Data = filedata.Substring(filedata.IndexOf(',') + 1);
            return Convert.FromBase64String(base64Data);
        }

        return null;
    }

    /// <summary>
    /// Retrieves the current user information from Straatos using the provided authentication token.
    /// </summary>
    /// <param name="token">The authentication token for the current user</param>
    /// <returns>JSON string containing current user information</returns>
    public async Task<string> GetCurrentUser(string token)
    {
        string baseUrl = _configuration.BaseUrl;
        string getDocumentTypesUrl = $"{baseUrl}/IAM/Users/CurrentUser";

        return await _httpService.GetAsync(getDocumentTypesUrl, token: token);
    }

    /// <summary>
    /// Retrieves tenant details from Straatos for the specified tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>JSON string containing tenant details</returns>
    public async Task<string> GetTenantDetails(int? tenantId)
    {
        string baseUrl = _configuration.BaseUrl;
        string tenantDetails = $"{baseUrl}/IAM/Tenants/{tenantId}";

        return await _httpService.GetAsync(tenantDetails);
    }

    /// <summary>
    /// Creates a JSON object for form data upload without file attachments.
    /// </summary>
    /// <param name="jsonData">The processed form data</param>
    /// <param name="documentResponse">The document response containing form configuration</param>
    /// <param name="uploadId">The upload identifier from Straatos</param>
    /// <param name="raw">The raw form data</param>
    /// <returns>A JSON object ready for upload</returns>
    private object CreateJsonObject(dynamic jsonData, DocumentResponse documentResponse, string uploadId, dynamic raw)
    {
        var jsonObject = new
        {
            Id = documentResponse.Id,
            DocumentTypeName = documentResponse.DocumentTypeName,
            Name = documentResponse.Name,
            Category = documentResponse.Category,
            DocumentId = uploadId,
            Fields = documentResponse.Fields.ToList(),
            Ref = $"{raw.data.documentId}"
        };

        IDictionary<string, object> dataDict = jsonData;

        foreach (var field in documentResponse.Fields)
        {
            if (dataDict.ContainsKey(field.Name))
            {
                if (dataDict[field.Name] is FileUploadAdditionalData)
                    field.Value = (dataDict[field.Name] as FileUploadAdditionalData).name;
                else
                    field.Value = dataDict[field.Name]?.ToString();
            }
        }

        return jsonObject;
    }

    /// <summary>
    /// Extracts file upload data from form fields for processing.
    /// </summary>
    /// <param name="jsonData">The form data containing file uploads</param>
    /// <param name="documentResponse">The document response containing field definitions</param>
    /// <returns>A list of file upload additional data objects</returns>
    private List<FileUploadAdditionalData> GetFileUploads(dynamic jsonData, DocumentResponse documentResponse)
    {
        List<FileUploadAdditionalData> fileUploads = new List<FileUploadAdditionalData>();

        IDictionary<string, object> dataDict = jsonData;
        foreach (var field in documentResponse.Fields)
        {
            if (dataDict.ContainsKey(field.Name) && field.Datatype == "file")
            {
                var fileUpload = JsonConvert.DeserializeObject<FileUploadAdditionalData>(JsonConvert.SerializeObject(dataDict[field.Name]));
                if (fileUpload == null)
                    continue;

                fileUpload.key = field.Name;
                fileUploads.Add(fileUpload);
            }

        }

        return fileUploads;
    }

    private object CreateJsonObjectWithFiles(dynamic jsonData, DocumentResponse documentResponse, string uploadId, dynamic raw)
    {
        var jsonObject = new
        {
            Id = documentResponse.Id,
            DocumentTypeName = documentResponse.DocumentTypeName,
            Name = documentResponse.Name,
            Category = documentResponse.Category,
            DocumentId = uploadId,
            Fields = documentResponse.Fields.ToList(),
            formStatesConfig = documentResponse.formStatesConfig,

            Ref = $"{raw.data.documentId}"
        };

        IDictionary<string, object> dataDict = jsonData;

        foreach (var field in documentResponse.Fields)
        {
            if (dataDict.ContainsKey(field.Name))
            {
                if (dataDict[field.Name] is FileUploadAdditionalData) {
                    field.Value = JsonConvert.SerializeObject(dataDict[field.Name]);
                }
                else
                    field.Value = dataDict[field.Name]?.ToString();
            }
            Console.WriteLine(JsonConvert.SerializeObject(field));
        }

        return jsonObject;
    }

}
