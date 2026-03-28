using BykStudio.data.DTOs;

namespace BykStudio.Web.Services
{
    public class MakeupBookingService
    {
        private readonly HttpClient _httpClient;

        public MakeupBookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool Success, MakeupBookingResult? Result, string? Error)> CreateMakeupBookingAsync(CreateMakeupBookingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/makeupbookings", request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MakeupBookingResult>();
                return (true, result, null);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, null, error);
            }
        }

        public async Task<PaymentInitiationResult?> InitiateMakeupPaymentAsync(Guid bookingId)
        {
            // Adjust this to match your actual payment initiation endpoint
            var response = await _httpClient.PostAsync($"api/makeupbookings/{bookingId}/initiate-payment", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaymentInitiationResult>();
            }
            return null;
        }

        // Optional: method to add contact info for guest bookings
        public async Task<bool> AddContactInfoAsync(Guid bookingId, GuestContactRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/makeupbookings/{bookingId}/contact", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<BookedSlotDto>> GetBookedSlotsAsync(DateTime start, DateTime end)
        {
            try
            {
                // Ensure UTC
                start = start.ToUniversalTime();
                end = end.ToUniversalTime();
                var response = await _httpClient.GetFromJsonAsync<List<BookedSlotDto>>(
                    $"api/makeupbookings/slots?start={start:O}&end={end:O}");
                return response ?? new List<BookedSlotDto>();
            }
            catch
            {
                return new List<BookedSlotDto>();
            }
        }

        public async Task<MakeupBookingConfirmationDto?> GetBookingConfirmationAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<MakeupBookingConfirmationDto>($"api/makeupbookings/{id}");
            }
            catch
            {
                return null;
            }
        }
    }
}
