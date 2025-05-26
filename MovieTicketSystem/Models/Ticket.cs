using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("tickets")]
    public class Ticket
    {
        [Key]
        [Column("ticket_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketId { get; set; }

        [Required]
        [Column("booking_id")]
        public int BookingId { get; set; }

        [Required]
        [Column("seat_id")]
        public int SeatId { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }
    }
}