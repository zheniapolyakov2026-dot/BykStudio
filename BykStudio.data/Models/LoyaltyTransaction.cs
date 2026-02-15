using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BykStudio.data.Models
{
    public class LoyaltyTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int Points { get; set; } // Positive for earning, negative for redemption/refund

        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // "Earn", "Redeem", "Refund", "Adjustment"

        public Guid? PaymentId { get; set; } // Link to payment if earned/refunded

        public Guid? BookingId { get; set; } // Link to booking if redeemed

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(PaymentId))]
        public Payment? Payment { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }
    }
}
