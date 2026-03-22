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

        public async Task<MakeupBookingResult?> CreateMakeupBookingAsync(CreateMakeupBookingRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/makeupbookings", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MakeupBookingResult>();
            }
            // Handle errors – you may want to throw or return null with error details
            return null;
        }

        public async Task<MakeupPaymentResult?> InitiateMakeupPaymentAsync(Guid bookingId)
        {
            // Adjust this to match your actual payment initiation endpoint
            var response = await _httpClient.PostAsync($"api/makeupbookings/{bookingId}/initiate-payment", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MakeupPaymentResult>();
            }
            return null;
        }

        // Optional: method to add contact info for guest bookings
        public async Task<bool> AddContactInfoAsync(Guid bookingId, GuestContactRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/makeupbookings/{bookingId}/contact", request);
            return response.IsSuccessStatusCode;
        }
    }
}
