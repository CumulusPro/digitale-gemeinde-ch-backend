using AutoMapper;
using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Newtonsoft.Json;

namespace Cpro.Forms.Service.Services;

/// <summary>
/// Service for managing form design version history including retrieval and storage of historical versions.
/// </summary>
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

    /// <summary>
    /// Retrieves all versions of a form design with signed URLs for blob storage access.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <returns>A list of form design versions</returns>
    public async Task<List<FormDesign>> GetAllVersions(string formId)
    {
        var versions = await _formDesignHistoryRepository.GetAllVersions(formId);

        foreach (var version in versions) 
        {
            version.StorageUrl = _azureBlobService.GetSignedUrl(version.StorageUrl);
        }

        return _mapper.Map<List<FormDesign>>(versions);
    }

    /// <summary>
    /// Retrieves a specific version of a form design by form ID and version number.
    /// </summary>
    /// <param name="formId">The unique identifier of the form</param>
    /// <param name="version">The version number to retrieve</param>
    /// <returns>The field request for the specified version</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified version is not found</exception>
    /// <exception cref="InvalidOperationException">Thrown when JSON deserialization fails</exception>
    public async Task<FieldRequest> GetVersion(string formId, int version)
    {
        var history = await _formDesignHistoryRepository.GetVersion(formId, version)
            ?? throw new FileNotFoundException($"Version {version} not found for FormId: {formId}");

        var json = await _azureBlobService.GetFile(history.StorageUrl);
        var fieldRequest = JsonConvert.DeserializeObject<FieldRequest>(json);
        return fieldRequest ?? throw new InvalidOperationException($"Failed to deserialize JSON for FormId: {formId} and Version: {version}");
    }

    /// <summary>
    /// Saves a form design version to the history repository for future reference.
    /// </summary>
    /// <param name="formDesign">The form design to save to history</param>
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
            Email = d.Email,
            FormDesignId = formDesign.Id,
            FormVersion = formDesign.Version
        }).ToList();

        history.Processors = formDesign.Processors.Select(p => new Data.Models.ProcessorHistory
        {
            Email = p.Email,
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
