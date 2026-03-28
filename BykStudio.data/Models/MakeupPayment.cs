using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BykStudio.data.Models
{
    public class MakeupPayment
    {
        [Key]
        public Guid MakeupPaymentId { get; set; } = Guid.NewGuid();

        public Guid MakeupBookingId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? TransactionId { get; set; }
        public bool IsSuccessful { get; set; }
        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // e.g., "Credit Card", "Cash"

        [ForeignKey(nameof(MakeupBookingId))]
        public MakeUpBooking MakeupBooking { get; set; } = null!;
    }
}
