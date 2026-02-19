using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BykStudio.data.Models
{
    public class LoyaltyPoints
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Balance { get; set; }
        public DateTime LastUpdated { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
