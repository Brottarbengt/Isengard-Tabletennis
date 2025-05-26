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
            var matchDTO = await _matchService.GetMatchForEndGameByIdAsync(matchId);
            if (matchDTO == null)
                return NotFound();

            EndMatchVM = matchDTO.Adapt<EndMatchViewModel>();
            EndMatchVM.MatchStartTime = matchDTO.StartTime;
            EndMatchVM.MatchEndTime = matchDTO.EndTime;
            for (int i = 0; i < EndMatchVM.Sets.Count; i++)
            {
                EndMatchVM.Sets[i].StartTime = matchDTO.Sets[i].StartTime;
                EndMatchVM.Sets[i].EndTime = matchDTO.Sets[i].EndTime;
            }
            return Page();
        }
    }
}
