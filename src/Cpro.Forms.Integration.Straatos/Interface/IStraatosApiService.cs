using Cpro.Forms.Integration.Straatos.Models;

namespace Cpro.Forms.Integration.Straatos.Services;

public interface IStraatosApiService
{
    public Task<string> UploadSimple(dynamic jsonData, DocumentResponse documentResponse);
    public Task<string> GetCurrentUser(string token);
    public Task<string> GetTenantDetails(int? tenantId);
    public IDictionary<string, object> GetIndexFieldObject(dynamic jsonData, DocumentResponse documentResponse, out dynamic combinedObject, out dynamic combinedObjectWithFiles);
}