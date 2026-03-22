using System.Text.Json;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure one-to-many: User -> Bookings
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many: Room -> Bookings
            builder.Entity<Room>()
                .HasMany(r => r.Bookings)
                .WithOne(b => b.Room)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting room with bookings

            // Configure one-to-one: Booking -> Payment
            builder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add unique index to prevent overlapping bookings (optional)
            builder.Entity<Booking>()
                .HasIndex(b => new { b.RoomId, b.StartTime, b.EndTime })
                .IsUnique(); // This ensures no double-booking, but needs careful handling

            // Configure decimal precision for all money fields
            builder.Entity<Booking>()
                .Property(b => b.TotalPrice)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Optional: Configure Photos to be stored as JSON (if not already)
            builder.Entity<Room>()
                .Property(r => r.Photos)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
                );

            // Seed Rooms
            builder.Entity<Room>().HasData(
                new Room
                {
                    RoomId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Зал 1",
                    PricePerHour = 1000m,
                    Description = "Зал | \r\n\r\nЗал в стиле минимализм. \r\nВ ваше пользование будет предоставлено:\r\n- циклорама \r\n- проф.оборудование \r\n- флаги\r\n- 2-х метровый кожаный диван\r\n- бумажные фоны\r\n- тканевые фоны \r\n- кресло \r\n- 4 стула \r\n- черная кожаная банкетка   \r\n- зеркало\r\n- рейл\r\n в данном зале блэкаут шторы",
                    Capacity = 10,
                    MainImageUrl = "/images/room7.jpg",
                    Photos = ["/images/room7.jpg", "/images/room7.jpg"],
                    IsAvailable = true
                },
                new Room
                {
                    RoomId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Зал 2",
                    PricePerHour = 1500m,
                    Description = "Equipped with professional gear",
                    Capacity = 15,
                    MainImageUrl = "/images/room7.jpg",
                    Photos = ["room7.jpg", "room7.jpg"],
                    IsAvailable = true
                }
            );
        }

    }
}
