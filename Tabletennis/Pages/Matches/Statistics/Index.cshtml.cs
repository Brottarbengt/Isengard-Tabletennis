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
        private readonly IPlayerService _playerService;
        private readonly IMatchService _matchService;

        public IndexModel(IPlayerService playerService, IMatchService matchService)
        {
            _playerService = playerService;
            _matchService = matchService;
        }

        [BindProperty]
        public StatisticsViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? player1Id, int? player2Id, string searchQuery)
        {
            ViewModel.SelectedPlayer1Id = player1Id;
            ViewModel.SelectedPlayer2Id = player2Id;
            ViewModel.SearchQuery = searchQuery ?? string.Empty;

            // Hämta alla aktiva spelare
            var allPlayers = await _playerService.GetAllPlayersAsync();
            ViewModel.AllPlayers = allPlayers;

            // Skapa SelectList för dropdown
            ViewModel.PlayerSelectList = allPlayers
                .Select(p => new SelectListItem
                {
                    Value = p.PlayerId.ToString(),
                    Text = $"{p.FirstName} {p.LastName}"
                })
                .ToList();

            // Om båda spelare är valda, visa head-to-head statistik
            if (player1Id.HasValue && player2Id.HasValue)
            {
                var player1 = await _playerService.GetPlayerByIdAsync(player1Id.Value);
                var player2 = await _playerService.GetPlayerByIdAsync(player2Id.Value);

                // Hämta alla matcher mellan spelarna
                var matches = await _matchService.GetFilteredMatchesAsync(new Services.Infrastructure.MatchQueryParameters
                {
                    PageSize = 1000 // Hämta alla matcher
                });

                var headToHeadMatches = matches.Results
                    .Select(m => _matchService.GetMatchByIdAsync(m.MatchId).Result)
                    .Where(m => m != null && 
                           ((m.Player1Id == player1Id && m.Player2Id == player2Id) ||
                            (m.Player1Id == player2Id && m.Player2Id == player1Id)))
                    .ToList();

                var player1Wins = headToHeadMatches.Count(m => 
                    (m.Player1Id == player1Id && m.Team1WonSets > m.Team2WonSets) || 
                    (m.Player2Id == player1Id && m.Team2WonSets > m.Team1WonSets));

                var player2Wins = headToHeadMatches.Count(m => 
                    (m.Player1Id == player2Id && m.Team1WonSets > m.Team2WonSets) || 
                    (m.Player2Id == player2Id && m.Team2WonSets > m.Team1WonSets));

                var totalMatches = headToHeadMatches.Count;

                ViewModel.VsStats = new VsPlayerStatsViewModel
                {
                    Player1 = new PlayerStatisticsViewModel
                    {
                        PlayerId = player1.PlayerId,
                        FirstName = player1.FirstName,
                        LastName = player1.LastName,
                        NumberOfWins = player1.NumberOfWins,
                        NumberOfLosses = player1.NumberOfLosses,
                        PlayerWinRatio = player1.PlayerWinRatio,
                        MatchesPlayed = player1.MatchesPlayed,
                        FullName = $"{player1.FirstName} {player1.LastName}"
                    },
                    Player2 = new PlayerStatisticsViewModel
                    {
                        PlayerId = player2.PlayerId,
                        FirstName = player2.FirstName,
                        LastName = player2.LastName,
                        NumberOfWins = player2.NumberOfWins,
                        NumberOfLosses = player2.NumberOfLosses,
                        PlayerWinRatio = player2.PlayerWinRatio,
                        MatchesPlayed = player2.MatchesPlayed,
                        FullName = $"{player2.FirstName} {player2.LastName}"
                    },
                    Player1Wins = player1Wins,
                    Player2Wins = player2Wins,
                    TotalMatches = totalMatches,
                    Player1WinRatio = totalMatches > 0 ? (decimal)player1Wins / totalMatches * 100 : 0,
                    Player2WinRatio = totalMatches > 0 ? (decimal)player2Wins / totalMatches * 100 : 0
                };
            }
            // Om ingen spelare är vald, visa Top 10
            else if (!player1Id.HasValue && !player2Id.HasValue)
            {
                // Hämta Top 10 spelare baserat på WinRatio
                var top10Players = allPlayers
                    .OrderByDescending(p => p.PlayerWinRatio)
                    .Take(10)
                    .Select(p => new Top10PlayersViewModel
                    {
                        PlayerId = p.PlayerId,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        PlayerWinRatio = p.PlayerWinRatio,
                        MatchesPlayed = p.MatchesPlayed,
                        NumberOfWins = p.NumberOfWins,
                        FullName = $"{p.FirstName} {p.LastName}"
                    })
                    .ToList();

                ViewModel.Top10Players = top10Players;
            }
            // Om bara en spelare är vald, visa deras statistik
            else
            {
                if (player1Id.HasValue)
                {
                    var player1 = await _playerService.GetPlayerByIdAsync(player1Id.Value);
                    ViewModel.Player1Stats = new PlayerStatisticsViewModel
                    {
                        PlayerId = player1.PlayerId,
                        FirstName = player1.FirstName,
                        LastName = player1.LastName,
                        NumberOfWins = player1.NumberOfWins,
                        NumberOfLosses = player1.NumberOfLosses,
                        PlayerWinRatio = player1.PlayerWinRatio,
                        MatchesPlayed = player1.MatchesPlayed,
                        FullName = $"{player1.FirstName} {player1.LastName}"
                    };
                }

                if (player2Id.HasValue)
                {
                    var player2 = await _playerService.GetPlayerByIdAsync(player2Id.Value);
                    ViewModel.Player2Stats = new PlayerStatisticsViewModel
                    {
                        PlayerId = player2.PlayerId,
                        FirstName = player2.FirstName,
                        LastName = player2.LastName,
                        NumberOfWins = player2.NumberOfWins,
                        NumberOfLosses = player2.NumberOfLosses,
                        PlayerWinRatio = player2.PlayerWinRatio,
                        MatchesPlayed = player2.MatchesPlayed,
                        FullName = $"{player2.FirstName} {player2.LastName}"
                    };
                }
            }

            return Page();
        }
    }
}
