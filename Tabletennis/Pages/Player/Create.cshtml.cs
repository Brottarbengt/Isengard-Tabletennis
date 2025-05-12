using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Player
{
    public class CreateModel : PageModel
    {

        
        public PlayerCreateViewModel NewPlayer { get; set; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Om valideringen misslyckas, visa formuläret igen med fel
                return Page();
            }

            

            return RedirectToPage("Index");
        }
    }
}
