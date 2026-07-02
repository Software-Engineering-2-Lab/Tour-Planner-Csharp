namespace TourPlanner.backend.DTOs
{
    public class UserProfileResponseDto
    {
        public long Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public int TotalTours { get; set; }
    }
}