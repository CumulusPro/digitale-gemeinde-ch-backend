using Cpro.Forms.Service.Models.Payment;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cpro.Forms.Api.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class PaymentController : Controller
{
    private readonly IPaymentService _formService;

    public PaymentController(IPaymentService formService)
    {
        _formService = formService;
    }

    [HttpPost("Submit")]
    public async Task<IActionResult> SubmitForm([FromBody] PaymentRequest request)
    {
        var response = await _formService.CreatePaymentRequest(request);
        return Ok(response);
    }  
}
