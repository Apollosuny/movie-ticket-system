using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MovieTicketSystem.Pages.Shared
{
    public class LoginPartialModel : PageModel
    {
        [Microsoft.AspNetCore.Mvc.BindProperty(SupportsGet = true)]
        public bool Mobile { get; set; }

        public void OnGet()
        {
        }
    }
}
