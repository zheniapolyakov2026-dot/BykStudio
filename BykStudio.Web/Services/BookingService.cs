using BykStudio.data.DTOs;

namespace BykStudio.Web.Services
{
    public class BookingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BookingService> _logger;

        public BookingService(HttpClient httpClient, ILogger<BookingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<CreateBookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Bookings", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<CreateBookingResponse>()
                    ?? throw new InvalidOperationException("Failed to create booking");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw;
            }
        }

        public async Task<PaymentInitiationResult> InitiatePaymentAsync(Guid bookingId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Payment/initiate", new { BookingId = bookingId });
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PaymentInitiationResult>()
                    ?? throw new InvalidOperationException("Failed to initiate payment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment for booking {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<List<BookedSlotDto>> GetBookedSlotsAsync(Guid roomId, DateTime startUtc, DateTime endUtc)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<BookedSlotDto>>(
                    $"api/bookings/{roomId}/slots?start={startUtc:O}&end={endUtc:O}");
                return response ?? new List<BookedSlotDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booked slots for room {RoomId}", roomId);
                return new List<BookedSlotDto>();
            }
        }

        public async Task<BookingConfirmationDto?> GetBookingConfirmationAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BookingConfirmationDto>($"api/bookings/{id}/confirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking confirmation for {BookingId}", id);
                return null;
            }
        }

        public async Task<bool> AddContactInfoAsync(Guid bookingId, GuestContactRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/bookings/{bookingId}/contact", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact info for booking {BookingId}", bookingId);
                return false;
            }
        }
    }
}
