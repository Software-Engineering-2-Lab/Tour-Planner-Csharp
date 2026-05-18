using System;
using System.Text.Json.Serialization;

namespace TourPlanner.backend.DTOs
{
    public class LogDto
    {
        public long Id { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }

        public string ?Comment { get; set; }

        public int Difficulty { get; set; }

        public double TotalDistance { get; set; }

        public double TotalTime { get; set; }

        public int Rating { get; set; }

        public long TourId { get; set; }
    }
}