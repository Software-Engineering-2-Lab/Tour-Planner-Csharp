// DTOs/RoutePreviewResponseDto.cs
namespace TourPlanner.backend.DTOs
{
    public class RoutePreviewResponseDto
    {
        public double DistanceKm { get; set; }
        public double DurationMin { get; set; }
        public string ResolvedFrom { get; set; } = string.Empty;
        public string ResolvedTo { get; set; } = string.Empty;
    }
}