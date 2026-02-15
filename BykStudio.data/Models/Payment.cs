using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BykStudio.data.Models
{
    public class Payment
    {
        [Key]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        public Guid BookingId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // e.g., "Credit Card", "Cash"

        public bool IsSuccessful { get; set; } = true;

        // Navigation property
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;
    }
}
