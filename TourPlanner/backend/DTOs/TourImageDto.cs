namespace TourPlanner.backend.DTOs
{
    public class TourImageDto
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long TourId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}