using System;

namespace TourPlanner.backend.DTOs
{

public class RouteResponseDTO {

    public double Distance{get; set;}
    public double EstimatedTime{get; set;}
    public required string  MapImageUrl{get; set;}
    public required string GeometryJson{get; set;}
   
}
}
