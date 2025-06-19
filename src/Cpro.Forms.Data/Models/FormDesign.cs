namespace Cpro.Forms.Data.Models;

public class FormDesign
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int TenantId { get; set; }
    public int FormId { get; set; }
    public string TenantName { get; set; }
    public List<Designer> Designers { get; set; } = new();
    public List<Processor> Processors { get; set; } = new();
    public string? StorageUrl { get; set; }
    public bool? IsActive { get; set; }
    public int Version { get; set; }
    public List<FormStatesConfig> FormStates { get; set; } = new();
    public string? CreatedBy { get; set; }
    public DateTimeOffset DateCreated { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DateUpdated { get; set; }
    public ICollection<FormDesignTag> Tags { get; set; } = new List<FormDesignTag>();
}

public class FormStatesConfig
{
    public int FormStatesConfigId { get; set; }
    public string Label { get; set; }
    public string Value { get; set; }
    public string FormDesignId { get; set; }
}

public class Designer
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FormDesignId { get; set; }
}

public class Processor
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FormDesignId { get; set; }
}

public class Tag
{
    public int TagId { get; set; }
    public string TagName { get; set; } = null!;
    public ICollection<FormDesignTag> FormDesignTags { get; set; } = new List<FormDesignTag>();
}

public class FormDesignTag
{
    public string FormDesignId { get; set; }
    public FormDesign FormDesign { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}