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

        //TODO: Fixa seedning av players s� att top10 f�r spelare
        public IndexModel(IMatchService matchService, ISetService setService, IPlayerService playerService)
        {
            _matchService = matchService;
            _setService = setService;  //TODO: remember to Delete if not needed
            _playerService = playerService;
        }

        
        public List<Top10PlayersViewModel> Top10PlayersVMList { get; set; } = new();
        public Top10PlayersViewModel Top10PlayerVM { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var allPlayers = await _playerService.GetAllPlayerDTOsAsync();
            Top10PlayersVMList = new List<Top10PlayersViewModel>();

            foreach (var player in allPlayers) 
            { 
                var top10Player = new Top10PlayersViewModel
                {
                    PlayerId = player.PlayerId,
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    NumberOfWins = player.NumberOfWins,
                    MatchesPlayed = player.MatchesPlayed,
                    PlayerWinRatio = player.PlayerWinRatio
                };
                Top10PlayersVMList.Add(top10Player);
            }

            Top10PlayersVMList = Top10PlayersVMList
                .Where(p => p.MatchesPlayed >= 10)
                .OrderByDescending(p => p.PlayerWinRatio)
                .ThenByDescending(p => p.MatchesPlayed)
                .Take(10)
                .ToList();

            return Page();
        }
       
    }
}
