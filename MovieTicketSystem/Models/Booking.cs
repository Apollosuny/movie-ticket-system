using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("bookings")]
    public class Booking
    {
        [Key]
        [Column("booking_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("showtime_id")]
        public int ShowtimeId { get; set; }

        [Required]
        [Column("booking_time", TypeName = "datetime")]
        public DateTime BookingTime { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("ShowtimeId")]
        public Showtime? Showtime { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}