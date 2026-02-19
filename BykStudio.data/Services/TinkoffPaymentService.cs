using System.Text;
using System.Text.Json;
using BykStudio.data.DTOs;
using BykStudio.data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BykStudio.data.Services
{
    public class TinkoffPaymentService : ITinkoffPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TinkoffPaymentService> _logger;
        private readonly string _terminalKey;
        private readonly string _password;
        private const string ApiUrl = "https://securepay.tinkoff.ru/v2";

        public TinkoffPaymentService(HttpClient httpClient, IConfiguration configuration, ILogger<TinkoffPaymentService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _terminalKey = configuration["Tinkoff:TerminalKey"];
            _password = configuration["Tinkoff:Password"];
        }

        public async Task<TinkoffPaymentResponse> CreatePaymentAsync(decimal amount, string orderId, string description, string successUrl, string failUrl)
        {
            var request = new Dictionary<string, object>
            {
                ["TerminalKey"] = _terminalKey,
                ["Amount"] = (int)(amount * 100), // Tinkoff expects amount in kopecks
                ["OrderId"] = orderId,
                ["Description"] = description,
                ["SuccessURL"] = successUrl,
                ["FailURL"] = failUrl
            };

            // Add token for signature (required by Tinkoff)
            request["Token"] = GenerateToken(request);

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrl}/Init", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Tinkoff API error: {Error}", error);
                throw new Exception($"Tinkoff API error: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TinkoffPaymentResponse>(responseJson);
        }

        public Task<bool> VerifyWebhookSignature(JsonElement notification, string signature)
        {
            // Implement signature verification based on Tinkoff docs
            // This is simplified - you need to follow their exact algorithm
            var token = GenerateToken(JsonSerializer.Deserialize<Dictionary<string, object>>(notification.GetRawText()));
            return Task.FromResult(token == signature);
        }

        private string GenerateToken(Dictionary<string, object> data)
        {
            // Tinkoff token generation algorithm:
            // 1. Filter out empty values and 'Token' field itself
            // 2. Sort by key name
            // 3. Concatenate values + Password
            // 4. Get SHA-256 hash

            var values = data
                .Where(kv => kv.Value != null && !string.IsNullOrEmpty(kv.Value.ToString()) && kv.Key != "Token")
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value.ToString())
                .ToList();

            values.Add(_password);

            var stringToHash = string.Concat(values);
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
