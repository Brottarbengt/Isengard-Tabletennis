using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace Tabletennis.Pages.Matches
{
    public class UpdateModel : PageModel
    {
        private readonly IMatchService _matchService;

        public UpdateModel(IMatchService matchService)
        {
            _matchService = matchService;
        }
        [BindProperty]
        public MatchUpdateDTO Match { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var matchDto = await _matchService.GetMatchUpdateDtoAsync(id);
            if (matchDto == null) return NotFound();

            Match = matchDto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var updated = await _matchService.UpdateMatchAsync(Match);
            if (!updated) return NotFound();

            TempData["SuccessMessage"] = "Match updated successfully.";
            return RedirectToPage("/Matches/MatchHistory");
        }
    }
}
