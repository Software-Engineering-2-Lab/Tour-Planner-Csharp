using System;
using System.Text.Json.Serialization;
using TourPlanner.backend.Enums;

namespace TourPlanner.backend.DTOs{

      public class TourDto
      {
          public long Id { get; set; }
          public required string Name { get; set; }
          public string? Description { get; set; }
          public required string FromLocation { get; set; }
          public required string ToLocation { get; set; }
          public TransportType TransportType { get; set; }
          public double Distance { get; set; }
          public double EstimatedTime { get; set; }
          public string? RouteImagePath { get; set; }
          public int Popularity { get; set; }
          public double ChildFriendliness { get; set; }
          public long UserId { get; set; }
      }
}
