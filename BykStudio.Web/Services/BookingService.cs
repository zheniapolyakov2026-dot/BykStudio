using BykStudio.data.DTOs;

namespace BykStudio.Web.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;

        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CreateBookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Bookings", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CreateBookingResponse>()
                   ?? throw new InvalidOperationException("Failed to create booking");
        }

        public async Task<PaymentInitiateResponse> InitiatePaymentAsync(Guid bookingId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Payment/initiate", new { BookingId = bookingId });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaymentInitiateResponse>()
                   ?? throw new InvalidOperationException("Failed to initiate payment");
        }
    }
}
