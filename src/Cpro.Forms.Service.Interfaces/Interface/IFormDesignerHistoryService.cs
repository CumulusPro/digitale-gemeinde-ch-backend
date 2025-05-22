using Cpro.Forms.Service.Models;

namespace Cpro.Forms.Service.Services;

public interface IFormDesignerHistoryService
{
    Task<List<FormDesign>> GetAllVersions(string formId); 
    Task<FieldRequest> GetVersion(string formId, int version);
    Task SaveFormDesignVersionHistory(Data.Models.FormDesign? formDesign);
}
