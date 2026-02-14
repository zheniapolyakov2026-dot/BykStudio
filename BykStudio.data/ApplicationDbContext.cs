using BykStudio.data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BykStudio.data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<LoyaltyPoints> LoyaltyPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-one between ApplicationUser and LoyaltyPoints
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.LoyaltyPoints)
                .WithOne(lp => lp.User)
                .HasForeignKey<LoyaltyPoints>(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many: User -> Bookings
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many: Room -> Bookings
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting room with bookings

            // Configure one-to-one: Booking -> Payment
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add unique index to prevent overlapping bookings (optional)
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime })
                .IsUnique(); // This ensures no double-booking, but needs careful handling

            // Configure decimal precision for all money fields
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
        }

    }
}
