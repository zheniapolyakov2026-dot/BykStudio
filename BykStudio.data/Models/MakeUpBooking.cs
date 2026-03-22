using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BykStudio.data.Models
{
    public class MakeUpBooking
    {
        [Key]
        public Guid RoomId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal PricePerHour { get; set; }

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;
        public string MainImageUrl { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
