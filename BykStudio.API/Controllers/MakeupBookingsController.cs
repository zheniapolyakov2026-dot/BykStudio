using System.Security.Claims;
using BykStudio.data;
using BykStudio.data.DTOs;
using BykStudio.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeupBookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MakeupBookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/makeupbookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateMakeupBookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validate table exists and is available
                var table = await _context.MakeupTables
                    .FirstOrDefaultAsync(t => t.MakeupTableId == request.MakeupTableId && t.IsAvailable);

                if (table == null)
                    return BadRequest("Makeup table not available");

                // 2. Check for overlapping bookings (excluding cancelled)
                var overlapping = await _context.MakeupBookings
                    .AnyAsync(mb => mb.MakeupTableId == request.MakeupTableId &&
                                    mb.Status != BookingStatus.Cancelled &&
                                    ((request.StartTime >= mb.StartTime && request.StartTime < mb.EndTime) ||
                                     (request.EndTime > mb.StartTime && request.EndTime <= mb.EndTime) ||
                                     (request.StartTime <= mb.StartTime && request.EndTime >= mb.EndTime)));

                if (overlapping)
                    return BadRequest("Makeup table already booked for selected time");

                // 3. Calculate price (simple hours * rate)
                decimal hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
                decimal totalPrice = hours * table.PricePerHour;

                // 4. Get current user if logged in
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isGuest = string.IsNullOrEmpty(userId);

                // 5. Create booking
                var booking = new MakeupBooking
                {
                    MakeupTableId = request.MakeupTableId,
                    UserId = isGuest ? "guest-" + Guid.NewGuid().ToString() : userId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    TotalPrice = totalPrice,
                    Status = BookingStatus.Pending,
                    IsGuestBooking = isGuest
                };

                _context.MakeupBookings.Add(booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                if (isGuest)
                {
                    return Ok(new
                    {
                        booking.MakeupBookingId,
                        TotalPrice = totalPrice,
                        Message = "Пожалуйста, предоставьте информацию для завершения бронирования",
                        RequiresContactInfo = true
                    });
                }

                return Ok(new
                {
                    booking.MakeupBookingId,
                    TotalPrice = totalPrice,
                    RequiresPayment = true
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // POST: api/makeupbookings/{id}/contact
        [HttpPost("{id}/contact")]
        public async Task<IActionResult> AddContactInfo(Guid id, [FromBody] GuestContactRequest request)
        {
            var booking = await _context.MakeupBookings
                .FirstOrDefaultAsync(mb => mb.MakeupBookingId == id && mb.IsGuestBooking);

            if (booking == null)
                return NotFound("Guest makeup booking not found");

            if (booking.Status != BookingStatus.Pending)
                return BadRequest("Booking cannot be modified");

            booking.GuestName = request.Name;
            booking.GuestEmail = request.Email;
            booking.GuestPhone = request.Phone;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                BookingId = id,
                Message = "Contact information saved. You can now proceed to payment."
            });
        }

        // GET: api/makeupbookings/available
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSlots([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            // For simplicity, just return whether the table is available during the whole period
            var isBooked = await _context.MakeupBookings
                .AnyAsync(mb => mb.Status != BookingStatus.Cancelled &&
                                ((start >= mb.StartTime && start < mb.EndTime) ||
                                 (end > mb.StartTime && end <= mb.EndTime) ||
                                 (start <= mb.StartTime && end >= mb.EndTime)));

            return Ok(new { IsAvailable = !isBooked });
        }

        // GET: api/makeupbookings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _context.MakeupBookings
                .Include(mb => mb.MakeupTable)
                .Include(mb => mb.Payment)
                .FirstOrDefaultAsync(mb => mb.MakeupBookingId == id);

            if (booking == null)
                return NotFound();

            return Ok(new
            {
                booking.MakeupBookingId,
                booking.StartTime,
                booking.EndTime,
                booking.TotalPrice,
                booking.Status,
                booking.IsGuestBooking,
                booking.GuestName,
                booking.GuestEmail,
                booking.GuestPhone,
                MakeupTable = new
                {
                    booking.MakeupTable.Name,
                    booking.MakeupTable.PricePerHour
                },
                Payment = booking.Payment == null ? null : new
                {
                    booking.Payment.MakeupPaymentId,
                    booking.Payment.Amount,
                    booking.Payment.IsSuccessful,
                    booking.Payment.PaymentDate
                }
            });
        }
    }
}
