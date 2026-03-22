namespace BykStudio.data.DTOs
{
    public class MakeupTableDto
    {
        public Guid MakeupTableId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PricePerHour { get; set; }
        public string? Description { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
