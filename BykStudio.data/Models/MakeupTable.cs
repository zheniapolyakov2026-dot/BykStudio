using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BykStudio.data.Models
{
    public class MakeupTable
    {
        [Key]
        public Guid MakeupTableId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal PricePerHour { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string MainImageUrl { get; set; } = string.Empty;

        // Navigation
        public ICollection<MakeupBooking> MakeupBookings { get; set; } = new List<MakeupBooking>();
    }
}
