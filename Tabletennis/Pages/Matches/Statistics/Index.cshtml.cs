using DataAccessLayer.DTOs;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches.Statistics
{
    [BindProperties]
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

        
        public List<PlayerStatisticsViewModel> Top10PlayersVMList { get; set; } = new();
        public PlayerStatisticsViewModel PlayerStatisticsVM { get; set; } = new();
        public CreateMatchViewModel MatchVM { get; set; } = new();
        

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPlayersAsync();
            await ShowTop10PlayersAsync();
                

            return Page();
        }

        

        private async Task ShowTop10PlayersAsync()
        {
            //var allPlayers = await _playerService.GetAllPlayersAsync();
            Top10PlayersVMList = new List<PlayerStatisticsViewModel>();

            foreach (var player in MatchVM.AllPlayers)
            {
                var top10Player = new PlayerStatisticsViewModel
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
        }

        private async Task LoadPlayersAsync()
        {
            var players = await _playerService.GetAllPlayersAsync();

            MatchVM.AllPlayers = players.ToList();
            MatchVM.PlayerList = players
               .OrderBy(p => p.FullName)
             .Select(p => new SelectListItem
             {
                 Value = p.PlayerId.ToString(),
                 Text = $"{p.FullName} ({(p.BirthYear?.ToString() ?? "N/A")})"
             })
             .ToList();
        }

        public async Task<JsonResult> OnGetGetPlayer(int id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);

            if (player == null)
            {
                return new JsonResult(new { error = "Player not found" });
            }

            PlayerStatisticsVM = new PlayerStatisticsViewModel
            {
                PlayerId = player.PlayerId,                
                FullName = player.FullName,                
                NumberOfWins = player.NumberOfWins,
                MatchesPlayed = player.MatchesPlayed,
                PlayerWinRatio = player.PlayerWinRatio
            };

            return new JsonResult(new
            {
                player.PlayerId,                
                player.FullName,                
                Birthday = player.Birthday?.ToString("yyyy-MM-dd") ?? "N/A", // format safely
                player.BirthYear
            });
        }
    }
}
