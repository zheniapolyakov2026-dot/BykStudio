using System.Text.Json;
using BykStudio.data;
using BykStudio.data.DTOs;
using BykStudio.data.Interfaces;
using BykStudio.data.Models;
using BykStudio.data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.API.Controllers
{
    [Route("api/webhooks/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentController> _logger;
        private readonly ITinkoffPaymentService _tinkoffPaymentService;

        public PaymentController(
            ApplicationDbContext context,
            ITinkoffPaymentService tinkoffPaymentService,
            ILogger<PaymentController> logger)
        {
            _context = context;
            _tinkoffPaymentService = tinkoffPaymentService;
            _logger = logger;
        }

        // Tinkoff webhook endpoint
        [HttpPost("tinkoff")]
        public async Task<IActionResult> TinkoffWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var notification = JsonDocument.Parse(body).RootElement;

            // Get signature from headers
            var signature = Request.Headers["Signature"].ToString();

            // Verify signature
            if (!await _tinkoffPaymentService.VerifyWebhookSignature(notification, signature))
            {
                return Unauthorized("Invalid signature");
            }

            var paymentId = notification.GetProperty("PaymentId").GetString();
            var status = notification.GetProperty("Status").GetString();
            var amount = notification.GetProperty("Amount").GetDecimal() / 100;

            var payment = await _context.Payments
                .Include(p => p.Booking)
                .FirstOrDefaultAsync(p => p.PaymentId.ToString() == paymentId);

            if (payment == null)
                return NotFound();

            switch (status)
            {
                case "CONFIRMED":
                    payment.IsSuccessful = true;
                    payment.PaymentDate = DateTime.UtcNow;
                    payment.Booking.Status = BookingStatus.Confirmed;
                    break;

                case "REJECTED":
                case "CANCELLED":
                    payment.Booking.Status = BookingStatus.Cancelled;
                    break;

                case "REFUNDED":
                    payment.Booking.Status = BookingStatus.Cancelled;
                    break;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Endpoint to initiate payment (called from frontend)
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId);

            if (booking == null)
                return NotFound("Booking not found");

            // Create payment record
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = booking.TotalPrice,
                PaymentMethod = "Tinkoff",
                IsSuccessful = false
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Call Tinkoff API
            var successUrl = $"{Request.Scheme}://{Request.Host}/api/webhooks/Payment/success?paymentId={payment.PaymentId}";
            var failUrl = $"{Request.Scheme}://{Request.Host}/api/webhooks/Payment/fail?paymentId={payment.PaymentId}";

            var description = $"Booking: {booking.Room?.Name} on {booking.StartTime:dd.MM.yyyy HH:mm}";

            var tinkoffResponse = await _tinkoffPaymentService.CreatePaymentAsync(
                (int)(booking.TotalPrice * 100),   // convert to kopecks
                payment.PaymentId.ToString(),
                description,
                successUrl,
                failUrl
            );

            if (!tinkoffResponse.Success)
            {
                return BadRequest(new { Error = "Payment initiation failed", Details = tinkoffResponse.Message });
            }

            return Ok(new
            {
                PaymentId = payment.PaymentId,
                PaymentUrl = tinkoffResponse.PaymentURL
            });
        }
    }
}
