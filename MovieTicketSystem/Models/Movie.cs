using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("movies")]
    public class Movie
    {
        [Key]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Required]
        [Column("title")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [Column("duration")]
        public int Duration { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [Column("rating")]
        [StringLength(10)]
        public string Rating { get; set; }

        [Column("release_date", TypeName = "date")]
        public DateTime? ReleaseDate { get; set; }
    }
}