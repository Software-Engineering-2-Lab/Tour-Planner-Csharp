using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.backend.Entities
{
    [Table("tour_images")]
    public class TourImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("file_path")]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [Column("file_name")]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [Column("tour_id")]
        public long TourId { get; set; }

        [ForeignKey("TourId")]
        public Tour? Tour { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}