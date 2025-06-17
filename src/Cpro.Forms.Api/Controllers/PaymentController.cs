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

    /// <summary>
    /// Creates a payment request and returns a payment link for the user to complete the transaction.
    /// </summary>
    /// <param name="request">The payment request containing amount, currency, and redirect URLs</param>
    /// <returns>Payment link URL for the user to complete the transaction</returns>
    [HttpPost("Submit")]
    public async Task<IActionResult> SubmitForm([FromBody] PaymentRequest request)
    {
        var response = await _formService.CreatePaymentRequest(request);
        return Ok(response);
    }  
}
