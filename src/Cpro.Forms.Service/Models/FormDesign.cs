namespace Cpro.Forms.Service.Models;

public class FormDesign
{
    public string id { get; set; }
    public string Name { get; set; }
    public int TenantId { get; set; }
    public int FormId { get; set; }
    public string TenantName { get; set; }
    public List<Designer> designers { get; set; } = new();
    public List<Processor> processors { get; set; } = new();
    public bool? IsActive { get; set; }
    public string? StorageUrl { get; set; }
    public int Version { get; set; }
    public List<FormStatesConfig> FormStates { get; set; } = new();
    public DateTimeOffset DateCreated { get; set; }
    public DateTimeOffset? DateUpdated { get; set; }
    public List<string>? tags { get; set; }
}

public class FormStatesConfig
{
    public int FormStatesConfigId { get; set; }
    public string label { get; set; }
    public string value { get; set; }
}

public class Designer
{
    public int DesignerId { get; set; }
}

public class Processor
{
    public int ProcessorId { get; set; }
}