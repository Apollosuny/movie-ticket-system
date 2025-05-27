using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTicketSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [StringLength(100)]
        public string? FullName { get; set; }

        [PersonalData]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [PersonalData]
        [StringLength(255)]
        public string? Avatar { get; set; }

    [PersonalData]
    [StringLength(100)]
        public string? Email { get; set; }

        // Mối quan hệ với bảng Booking hiện có
    public virtual ICollection<Booking>? Bookings { get; set; }
    }
}
