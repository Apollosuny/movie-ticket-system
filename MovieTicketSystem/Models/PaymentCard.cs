using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
    [Table("payment_cards")]
    public class PaymentCard
    {
        [Key]
        [Column("card_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardId { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("card_number")]
        [StringLength(19)] // Format: XXXX-XXXX-XXXX-XXXX
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [Column("cardholder_name")]
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;

        [Required]
        [Column("expiry_date")]
        [StringLength(5)] // Format: MM/YY
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        [Column("card_type")]
        [StringLength(20)]
        public string CardType { get; set; } = string.Empty; // Visa, MasterCard, etc.
        
        [NotMapped] // Not stored in database
        public string CVV { get; set; } = string.Empty;

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
