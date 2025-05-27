using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace Tabletennis.Pages.Matches
{
    public class DeleteModel : PageModel
    {
        private readonly IMatchService _matchService;

        public DeleteModel(IMatchService matchService)
        {
            _matchService = matchService;
        }
        [BindProperty]
        public MatchDeleteDTO Match { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var match = await _matchService.GetMatchDeleteDtoAsync(id);
            if (match == null) return NotFound();
            Match = match;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _matchService.DeleteMatchAsync(Match.MatchId);
            if (!result) return NotFound();
            TempData["SuccessMessage"] = "Match was successfully deleted.";
            return RedirectToPage("/Matches/MatchHistory");
        }
    }
}
