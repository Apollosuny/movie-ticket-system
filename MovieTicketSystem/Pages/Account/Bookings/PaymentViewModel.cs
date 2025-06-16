using System.ComponentModel.DataAnnotations;
using MovieTicketSystem.Models;

namespace MovieTicketSystem.Pages.Account.Bookings
{
    public class PaymentViewModel
    {
        public int? SelectedCardId { get; set; }
        public List<PaymentCard>? ExistingCards { get; set; }
        
        [Display(Name = "Save card for future payments")]
        public bool SaveCard { get; set; } = true;
        
        [Display(Name = "Make this my default card")]
        public bool MakeDefault { get; set; } = false;
        
        [Display(Name = "Card Number")]
        [StringLength(19)]
        [Required(ErrorMessage = "Card number is required.")]
        [RegularExpression(@"^[\d\s-]{13,19}$", ErrorMessage = "Please enter a valid card number.")]
        public string CardNumber { get; set; } = string.Empty;
        
        [Display(Name = "Cardholder Name")]
        [Required(ErrorMessage = "Cardholder name is required.")]
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;
        
        [Display(Name = "Expiry Date")]
        [Required(ErrorMessage = "Expiry date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiry date must be in the format MM/YY.")]
        public string ExpiryDate { get; set; } = string.Empty;
        
        [Display(Name = "CVV")]
        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Please enter a valid CVV.")]
        public string CVV { get; set; } = string.Empty;
        
        [Display(Name = "Card Type")]
        public string CardType { get; set; } = "Visa";
    }
}
