using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("showtimes")]
    public class Showtime
    {
        [Key]
        [Column("showtime_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowtimeId { get; set; }

        [Required]
        [Column("movie_id")]
        public int MovieId { get; set; }

        [Required]
        [Column("screen_id")]
        public int ScreenId { get; set; }

        [Required]
        [Column("start_time", TypeName = "datetime")]
        public DateTime StartTime { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }

        [ForeignKey("ScreenId")]
        public Screen Screen { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}