using Cpro.Forms.Service.Models.Payment;

namespace Cpro.Forms.Service.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentRequest(PaymentRequest paymentRequest);
}
