using Microsoft.AspNetCore.Identity;
namespace BykStudio.data.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add any additional properties you want to include in your user model here
        public string? FullName { get; set; }

        // Navigation properties
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
