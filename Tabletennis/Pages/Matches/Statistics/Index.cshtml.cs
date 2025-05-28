using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches.Statistics
{
    public class IndexModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly ISetService _setService;
        private readonly IPlayerService _playerService;


        public IndexModel(IMatchService matchService, ISetService setService, IPlayerService playerService)
        {
            _matchService = matchService;
            _setService = setService;  //TODO: remember to Delete if not needed
            _playerService = playerService;
        }

        
        public List<Top10PlayersViewModel> Top10PlayersVM { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var allPlayers = await _playerService.GetAllPlayerDTOsAsync();
            Top10PlayersVM = allPlayers
                .OrderByDescending(p => p.PlayerWinRatio)
                .ThenByDescending(p => p.MatchesPlayed)
                .Take(10)
                .Select(p => new Top10PlayersViewModel
                {
                    PlayerId = p.PlayerId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    NumberOfWins = p.NumberOfWins,                    
                    PlayerWinRatio = p.PlayerWinRatio,
                    MatchesPlayed = p.MatchesPlayed
                })
                .ToList();

            return Page();
        }

       
    }
}
