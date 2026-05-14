using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourPlanner.backend.Enums;

namespace TourPlanner.backend.Entities
{
    [Table("tours")]
    public class Tour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("from_location")]
        public string FromLocation { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("to_location")]
        public string ToLocation { get; set; }

        [Required]
        [Column("transport_type")]
        public TransportType TransportType { get; set; }

        [Column("distance")]
        public double? Distance { get; set; }

        [Column("estimated_time")]
        public double? EstimatedTime { get; set; }

        [Column("route_image_path")]
        public string? RouteImagePath { get; set; }

        [Column("popularity")]
        public int? Popularity { get; set; }

        [Column("child_friendliness")]
        public double? ChildFriendliness { get; set; }
        

        [ForeignKey("UserId")]
        public User User { get; set; }
        public long UserId { get; set; }

        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}