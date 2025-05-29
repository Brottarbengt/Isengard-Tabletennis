using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        //public Top10PlayersViewModel Top10PlayerVM { get; set; } = new(); Obsolete?
        public CreateMatchViewModel MatchVM { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadPlayersAsync();
            await ShowTop10PlayersAsync();


            return Page();
        }

        private async Task ShowTop10PlayersAsync()
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
        }

        private async Task LoadPlayersAsync()
        {
            var players = await _matchService.GetAllPlayersAsync();

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
            var player = await _matchService.GetPlayerByIdAsync(id);

            if (player == null)
            {
                return new JsonResult(new { error = "Player not found" });
            }

            // Optional debug
            Console.WriteLine($"DEBUG: PlayerId={player.PlayerId}, FullName={player.FullName}, BirthYear={player.BirthYear}");

            return new JsonResult(new
            {
                player.PlayerId,
                player.FirstName,
                player.LastName,
                player.FullName,
                player.Email,
                player.PhoneNumber,
                player.Gender,
                Birthday = player.Birthday?.ToString("yyyy-MM-dd") ?? "N/A", // format safely
                player.BirthYear
            });
        }
    }
}
