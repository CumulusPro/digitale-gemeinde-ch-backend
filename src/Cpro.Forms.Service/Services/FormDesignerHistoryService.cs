using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;

namespace Cpro.Forms.Service.Services;

public class FormDesignerHistoryService : IFormDesignerHistoryService
{
    private readonly IAzureBlobService _azureBlobService;
    private readonly IMapper _mapper;
    private readonly IFormDesignHistoryRepository _formDesignHistoryRepository;

    public FormDesignerHistoryService(
        IFormDesignHistoryRepository formDesignHistoryRepository,
        IMapper mapper,
        IAzureBlobService azureBlobService)
    {
        _mapper = mapper;
        _azureBlobService = azureBlobService;
        _formDesignHistoryRepository = formDesignHistoryRepository;
    }

    public async Task<List<FormDesign>> GetAllVersions(string formId)
    {
        var versions = await _formDesignHistoryRepository.GetAllVersions(formId);

        foreach (var version in versions) 
        {
            version.StorageUrl = _azureBlobService.GetSignedUrl(version.StorageUrl);
        }

        return _mapper.Map<List<FormDesign>>(versions);
    }

    public async Task<FieldRequest> GetVersion(string formId, int version)
    {
        var history = await _formDesignHistoryRepository.GetVersion(formId, version)
            ?? throw new FileNotFoundException($"Version {version} not found for FormId: {formId}");

        var json = await _azureBlobService.GetFile(history.StorageUrl);
        var fieldRequest = JsonConvert.DeserializeObject<FieldRequest>(json);
        return fieldRequest ?? throw new InvalidOperationException($"Failed to deserialize JSON for FormId: {formId} and Version: {version}");
    }

    public async Task SaveFormDesignVersionHistory(Data.Models.FormDesign? formDesign)
    {
        var history = new Data.Models.FormDesignHistory
        {
            Id = Guid.NewGuid().ToString(),
            FormDesignId = formDesign.Id,
            Name = formDesign.Name,
            TenantId = formDesign.TenantId,
            FormId = formDesign.FormId,
            TenantName = formDesign.TenantName,
            StorageUrl = formDesign.StorageUrl,
            Version = formDesign.Version,
            DateCreated = DateTimeOffset.UtcNow,
            CreatedBy = formDesign.CreatedBy
        };

        history.Designers = formDesign.Designers.Select(d => new Data.Models.DesignerHistory
        {
            DesignerHistoryId = d.DesignerId,
            FormDesignId = formDesign.Id,
            FormVersion = formDesign.Version
        }).ToList();

        history.Processors = formDesign.Processors.Select(p => new Data.Models.ProcessorHistory
        {
            ProcessorHistoryId = p.ProcessorId,
            FormDesignId = formDesign.Id,
            FormVersion = formDesign.Version
        }).ToList();

        history.FormStates = formDesign.FormStates.Select(s => new Data.Models.FormStatesConfigHistory
        {
            Label = s.Label,
            Value = s.Value,
            FormDesignId = formDesign.Id,
            FormVersion = formDesign.Version
        }).ToList();

        await _formDesignHistoryRepository.SaveFormDesignHistoryAsync(history);
    }

}
