using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.backend.Entities
{
    [Table("logs")]
    public class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("date_time")]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Column("comment")]
        public string Comment { get; set; }

        [Required]
        [Column("difficulty")]
        public int Difficulty { get; set; }

        [Required]
        [Column("total_distance")]
        public double TotalDistance { get; set; }

        [Required]
        [Column("total_time")]
        public double TotalTime { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        // Relația ManyToOne
        [ForeignKey("TourId")]
        public Tour Tour { get; set; }
        
        public long TourId { get; set; }
    }
}