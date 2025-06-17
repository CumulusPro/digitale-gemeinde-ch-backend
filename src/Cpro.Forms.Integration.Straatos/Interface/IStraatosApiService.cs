using Cpro.Forms.Service.Models;

namespace Cpro.Forms.Integration.Straatos.Services;

/// <summary>
/// Interface for Straatos API service operations including document uploads, user management, and tenant operations.
/// </summary>
public interface IStraatosApiService
{
    /// <summary>
    /// Uploads form data to Straatos workflow system with file handling and document creation.
    /// </summary>
    /// <param name="jsonData">The form data to upload</param>
    /// <param name="documentResponse">The document response containing form configuration</param>
    /// <returns>The document ID returned from Straatos</returns>
    public Task<string> UploadSimple(dynamic jsonData, DocumentResponse documentResponse);
    
    /// <summary>
    /// Retrieves the current user information from Straatos using the provided authentication token.
    /// </summary>
    /// <param name="token">The authentication token for the current user</param>
    /// <returns>JSON string containing current user information</returns>
    public Task<string> GetCurrentUser(string token);
    
    /// <summary>
    /// Retrieves tenant details from Straatos for the specified tenant ID.
    /// </summary>
    /// <param name="tenantId">The tenant identifier</param>
    /// <returns>JSON string containing tenant details</returns>
    public Task<string> GetTenantDetails(int? tenantId);
    
    /// <summary>
    /// Extracts and processes index field objects from form data, separating file uploads from regular fields.
    /// </summary>
    /// <param name="jsonData">The form data containing field values</param>
    /// <param name="documentResponse">The document response containing field definitions</param>
    /// <param name="combinedObject">Output parameter containing combined object without files</param>
    /// <param name="combinedObjectWithFiles">Output parameter containing combined object with files</param>
    /// <returns>A dictionary containing the processed index fields</returns>
    public IDictionary<string, object> GetIndexFieldObject(dynamic jsonData, DocumentResponse documentResponse, out dynamic combinedObject, out dynamic combinedObjectWithFiles);
}