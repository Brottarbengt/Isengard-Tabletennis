using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches
{
    public class EndMatchModel : PageModel
    {
        private readonly IMatchService _matchService;

        public EndMatchModel(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public EndMatchViewModel EndMatchVM { get; set; }

        public async Task<IActionResult> OnGetAsync(int matchId)
        {
            ViewData["ShowHeader"] = false;
            var matchDTO = await _matchService.GetMatchForEndGameByIdAsync(matchId);
            if (matchDTO == null)
                return NotFound();

            EndMatchVM = matchDTO.Adapt<EndMatchViewModel>();
            EndMatchVM.DurationSeconds = matchDTO.DurationSeconds;
            return Page();
        }

    }
}
