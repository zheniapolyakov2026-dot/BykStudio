using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BykStudio.data.Models
{
    public class MakeUpBooking
    {
        [Key]
        public Guid MakeupBookingId { get; set; } = Guid.NewGuid();

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public Guid MakeupTableId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(MakeupTableId))]
        public MakeupTable MakeupTable { get; set; } = null!;

        public MakeupPayment? Payment { get; set; }

        // Guest info
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public bool IsGuestBooking { get; set; }
    }
}