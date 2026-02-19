namespace BykStudio.data.DTOs
{
    public class CreateBookingRequest
    {
        public Guid RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumberOfPeople { get; set; } = 1;
        public int RedeemPoints { get; set; } = 0;
    }
}
