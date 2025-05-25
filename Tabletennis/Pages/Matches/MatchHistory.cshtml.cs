using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;

namespace Tabletennis.Pages.Matches
{
    public class MatchHistoryModel : PageModel
    {
        private readonly IMatchService _matchService;

        public MatchHistoryModel(IMatchService matchService)
        {
            _matchService = matchService;
        }

        public List<MatchListDTO> Matches { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        public async Task OnGetAsync()
        {
            Matches = await _matchService.GetFilteredMatchesAsync(Query);
        }
    }
}
