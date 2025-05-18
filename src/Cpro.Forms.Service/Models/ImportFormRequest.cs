using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Cpro.Forms.Service.Models;

public class ImportFormRequest
{
    [Required]
    public IFormFile File { get; set; }

    [Required]
    public int? tenantId { get; set; }

    public string? ExistingFormId { get; set; }
}
