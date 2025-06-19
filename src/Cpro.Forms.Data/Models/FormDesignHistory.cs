namespace Cpro.Forms.Data.Models;

public class FormDesignHistory
{
    public string Id { get; set; }
    public string FormDesignId { get; set; }
    public string Name { get; set; }
    public int TenantId { get; set; }
    public int FormId { get; set; }
    public string TenantName { get; set; }
    public string StorageUrl { get; set; }
    public int Version { get; set; }
    public List<DesignerHistory> Designers { get; set; } = new();
    public List<ProcessorHistory> Processors { get; set; } = new();
    public List<FormStatesConfigHistory> FormStates { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
    public string? CreatedBy { get; set; }
}

public class FormStatesConfigHistory
{
    public int FormStatesConfigHistoryId { get; set; }
    public string Label { get; set; }
    public string Value { get; set; }
    public string FormDesignId { get; set; }
    public int FormVersion { get; set; }
}

public class DesignerHistory
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FormDesignId { get; set; }
    public int FormVersion { get; set; }
}

public class ProcessorHistory
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FormDesignId { get; set; }
    public int FormVersion { get; set; }
}