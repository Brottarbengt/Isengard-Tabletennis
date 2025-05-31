using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IPlayerService _playerService;

        public IndexModel(ILogger<IndexModel> logger, IPlayerService playerService)
        {
            _logger = logger;
            _playerService = playerService;
        }

        public List<Top10PlayersViewModel> Top10Players { get; set; }

        public async Task OnGetAsync()
        {
            var players = await _playerService.GetTop10Players();

            Top10Players = players.Select(p => new Top10PlayersViewModel
            {
                PlayerId = p.PlayerId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                PlayerWinRatio = p.PlayerWinRatio,
                MatchesPlayed = p.MatchesPlayed,
                NumberOfWins = p.NumberOfWins,
                FullName = p.BirthYear.HasValue
                    ? $"{p.FirstName} {p.LastName} ({p.BirthYear})"
                    : $"{p.FirstName} {p.LastName}"
            }).ToList();
        }
    }
}
