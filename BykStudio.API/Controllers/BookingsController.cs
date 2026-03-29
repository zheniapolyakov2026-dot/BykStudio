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
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Public endpoint - no auth required for creating booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            // Start a transaction to ensure consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Validate room exists and is available
                var room = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.RoomId == request.RoomId && r.IsAvailable);

                if (room == null)
                    return BadRequest("Room not available");

                // 2. Check for overlapping bookings (excluding cancelled)
                var overlappingBooking = await _context.Bookings
                    .AnyAsync(b => b.RoomId == request.RoomId &&
                                  b.Status != BookingStatus.Cancelled &&
                                  ((request.StartTime >= b.StartTime && request.StartTime < b.EndTime) ||
                                   (request.EndTime > b.StartTime && request.EndTime <= b.EndTime) ||
                                   (request.StartTime <= b.StartTime && request.EndTime >= b.EndTime)));

                if (overlappingBooking)
                    return BadRequest("Room already booked for selected time");

                // 3. Calculate base price
                decimal hours = (decimal)(request.EndTime - request.StartTime).TotalHours;
                decimal basePrice = hours * room.PricePerHour;

                decimal totalPrice = basePrice;
                if (request.NumberOfPeople > 6)
                {
                    totalPrice += (request.NumberOfPeople - 6) * 200;
                }

                // 4. Get current user if logged in
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isGuest = string.IsNullOrEmpty(userId);

                // 5. Create booking
                var booking = new Booking
                {
                    RoomId = request.RoomId,
                    UserId = isGuest ? null : userId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    TotalPrice = totalPrice,
                    Status = BookingStatus.Pending,
                    IsGuestBooking = isGuest,
                    NumberOfPeople = request.NumberOfPeople
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 6. Return appropriate response
                if (isGuest)
                {
                    return Ok(new
                    {
                        booking.BookingId,
                        TotalPrice = totalPrice,
                        Message = "Пожалуйста, предоставьте информацию, чтобы завершить бронирование",
                        RequiresContactInfo = true
                    });
                }

                return Ok(new CreateBookingResponse
                {
                    BookingId = booking.BookingId,
                    TotalPrice = totalPrice,
                    Message = isGuest ? "Пожалуйста, предоставьте информацию для завершения бронирования" : null,
                    RequiresContactInfo = isGuest,
                    RequiresPayment = !isGuest
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Endpoint for guests to provide contact info
        [HttpPost("{bookingId}/contact")]
        public async Task<IActionResult> AddContactInfo(Guid bookingId, [FromBody] GuestContactRequest request)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.IsGuestBooking);

            if (booking == null)
                return NotFound("Guest booking not found");

            // Ensure booking is still pending
            if (booking.Status != BookingStatus.Pending)
                return BadRequest("Booking cannot be modified");

            // Update guest info
            booking.GuestName = request.Name;
            booking.GuestEmail = request.Email;
            booking.GuestPhone = request.Phone;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                BookingId = bookingId,
                Message = "Contact information saved. You can now proceed to payment."
            });
        }

        // Get available rooms for a time period
        [HttpGet("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                           ((start >= b.StartTime && start < b.EndTime) ||
                            (end > b.StartTime && end <= b.EndTime) ||
                            (start <= b.StartTime && end >= b.EndTime)))
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            var availableRooms = await _context.Rooms
                .Where(r => r.IsAvailable && !bookedRoomIds.Contains(r.RoomId))
                .Select(r => new
                {
                    r.RoomId,
                    r.Name,
                    r.Description,
                    r.PricePerHour
                })
                .ToListAsync();

            return Ok(availableRooms);
        }

        // Get booking details (for confirmation page)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            return Ok(new
            {
                booking.BookingId,
                booking.StartTime,
                booking.EndTime,
                booking.TotalPrice,
                booking.Status,
                booking.IsGuestBooking,
                booking.GuestName,
                booking.GuestEmail,
                booking.GuestPhone,
                Room = new
                {
                    booking.Room.Name,
                    booking.Room.PricePerHour
                },
                Payment = booking.Payment == null ? null : new
                {
                    booking.Payment.PaymentId,
                    booking.Payment.Amount,
                    booking.Payment.IsSuccessful,
                    booking.Payment.PaymentDate
                }
            });
        }

        [HttpGet("{roomId}/slots")]
        public async Task<IActionResult> GetBookedSlots(Guid roomId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            // Ensure UTC
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();

            var bookedSlots = await _context.Bookings
                .Where(b => b.RoomId == roomId &&
                            b.Status != BookingStatus.Cancelled &&
                            b.StartTime < end && b.EndTime > start)
                .Select(b => new BookedSlotDto
                {
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                })
                .ToListAsync();

            return Ok(bookedSlots);
        }

        [HttpGet("{id}/confirmation")]
        public async Task<IActionResult> GetBookingConfirmation(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound();

            var dto = new BookingConfirmationDto
            {
                BookingId = booking.BookingId,
                RoomName = booking.Room?.Name ?? string.Empty,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                TotalPrice = booking.TotalPrice,
                IsGuestBooking = booking.IsGuestBooking,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                GuestPhone = booking.GuestPhone,
                NumberOfPeople = booking.NumberOfPeople,   // make sure your Booking model has this property
                Payment = booking.Payment == null ? null : new PaymentInfo
                {
                    PaymentId = booking.Payment.PaymentId,
                    Amount = booking.Payment.Amount,
                    IsSuccessful = booking.Payment.IsSuccessful,
                    PaymentDate = booking.Payment.PaymentDate
                }
            };

            return Ok(dto);
        }
    }
}
