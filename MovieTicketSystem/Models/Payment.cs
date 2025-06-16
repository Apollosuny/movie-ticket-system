using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
  [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [Required]
        [Column("booking_id")]
        public int BookingId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("method")]
        [StringLength(100)]
        public string Method { get; set; } = string.Empty;

        [Required]
        [Column("paid_at", TypeName = "datetime")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }
    }
}