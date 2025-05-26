using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("seats")]
    public class Seat
    {
        [Key]
        [Column("seat_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeatId { get; set; }

        [Required]
        [Column("screen_id")]
        public int ScreenId { get; set; }

        [Required]
        [Column("row")]
        [StringLength(1)]
        public string Row { get; set; }

        [Required]
        [Column("number")]
        public int Number { get; set; }

        [Column("type")]
        [StringLength(20)]
        public string? Type { get; set; }

        [ForeignKey("ScreenId")]
        public Screen? Screen { get; set; }
        public ICollection<Ticket>? Tickets { get; set; }
    }
}