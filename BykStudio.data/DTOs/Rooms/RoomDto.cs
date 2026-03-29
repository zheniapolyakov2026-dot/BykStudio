namespace BykStudio.data.DTOs
{
    public class RoomDto
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; }
        public decimal PricePerHour { get; set; }
        public string? Description { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> Photos { get; set; }
        public bool IsAvailable { get; set; }
    }
}
