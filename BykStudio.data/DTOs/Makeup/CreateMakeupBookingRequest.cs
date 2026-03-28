namespace BykStudio.data.DTOs
{
    public class CreateMakeupBookingRequest
    {
        public Guid MakeupTableId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        // No NumberOfPeople – always 1 person
    }
}
