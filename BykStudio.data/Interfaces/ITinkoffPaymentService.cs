using System.Text.Json;
using BykStudio.data.DTOs;

namespace BykStudio.data.Interfaces
{
    public interface ITinkoffPaymentService
    {
        Task<TinkoffPaymentResponse> CreatePaymentAsync(decimal amount, string orderId, string description, string successUrl, string failUrl);
        Task<bool> VerifyWebhookSignature(JsonElement notification, string signature);
    }
}
