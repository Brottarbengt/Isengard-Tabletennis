using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace Tabletennis.Pages.Matches
{
    public class MatchDetailsModel : PageModel
    {
        private readonly IMatchService _matchService;

        public MatchDetailsModel(IMatchService matchService)
        {
            _matchService = matchService;
        }
        public MatchDetailsDTO Match { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var matchDetails = await _matchService.GetMatchDetailsAsync(id);

            if (matchDetails == null)
            {
                return NotFound();
            }

            Match = matchDetails;
            return Page();
        }
    }
}
