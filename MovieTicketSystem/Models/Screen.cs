using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("screens")]
    public class Screen
    {
        [Key]
        [Column("screen_id")]
        public int ScreenId { get; set; }

        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Column("seat_capacity")]
        public int SeatCapacity { get; set; }
    }
}