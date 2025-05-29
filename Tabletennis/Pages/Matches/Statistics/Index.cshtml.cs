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
            // Kontrollera om samma spelare är vald två gånger
            if (player1Id.HasValue && player2Id.HasValue && player1Id.Value == player2Id.Value)
            {
                // Om samma spelare är vald, återställ player2Id
                player2Id = null;
            }

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
                    Text = p.BirthYear.HasValue ? $"{p.FullName} ({p.BirthYear})" : p.FullName
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
                        FullName = player1.BirthYear.HasValue ? $"{player1.FullName} ({player1.BirthYear})" : player1.FullName
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
                        FullName = player2.BirthYear.HasValue ? $"{player2.FullName} ({player2.BirthYear})" : player2.FullName
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
                        FullName = p.BirthYear.HasValue ? $"{p.FullName} ({p.BirthYear})" : p.FullName
                    })
                    .ToList();

                ViewModel.Top10Players = top10Players;
            }
            // Om bara en spelare är vald, visa deras statistik
            else
            {
                var selectedPlayerId = player1Id ?? player2Id;
                var selectedPlayer = await _playerService.GetPlayerByIdAsync(selectedPlayerId.Value);

                // Hämta alla matcher för den valda spelaren
                var matches = await _matchService.GetFilteredMatchesAsync(new Services.Infrastructure.MatchQueryParameters
                {
                    PageSize = 1000
                });

                var playerMatches = matches.Results
                    .Select(m => _matchService.GetMatchByIdAsync(m.MatchId).Result)
                    .Where(m => m != null && (m.Player1Id == selectedPlayerId || m.Player2Id == selectedPlayerId))
                    .ToList();

                // Beräkna vinstratio mot varje motståndare
                var opponentStats = new Dictionary<int, (int wins, int total)>();
                foreach (var match in playerMatches)
                {
                    var opponentId = match.Player1Id == selectedPlayerId ? match.Player2Id : match.Player1Id;
                    if (!opponentStats.ContainsKey(opponentId))
                    {
                        opponentStats[opponentId] = (0, 0);
                    }

                    var stats = opponentStats[opponentId];
                    stats.total++;
                    if ((match.Player1Id == selectedPlayerId && match.Team1WonSets > match.Team2WonSets) ||
                        (match.Player2Id == selectedPlayerId && match.Team2WonSets > match.Team1WonSets))
                    {
                        stats.wins++;
                    }
                    opponentStats[opponentId] = stats;
                }

                // Hitta bästa och sämsta motståndare
                var opponentRatios = opponentStats
                    .Select(kvp => new
                    {
                        PlayerId = kvp.Key,
                        WinRatio = kvp.Value.total > 0 ? (decimal)kvp.Value.wins / kvp.Value.total * 100 : 0,
                        TotalMatches = kvp.Value.total
                    })
                    .Where(x => x.TotalMatches >= 3) // Minst 3 matcher för att räknas
                    .OrderByDescending(x => x.WinRatio)
                    .ToList();

                var bestOpponent = opponentRatios.FirstOrDefault();
                var worstOpponent = opponentRatios.LastOrDefault();

                if (player1Id.HasValue)
                {
                    ViewModel.Player1Stats = new PlayerStatisticsViewModel
                    {
                        PlayerId = selectedPlayer.PlayerId,
                        FirstName = selectedPlayer.FirstName,
                        LastName = selectedPlayer.LastName,
                        NumberOfWins = selectedPlayer.NumberOfWins,
                        NumberOfLosses = selectedPlayer.NumberOfLosses,
                        PlayerWinRatio = selectedPlayer.PlayerWinRatio,
                        MatchesPlayed = selectedPlayer.MatchesPlayed,
                        FullName = selectedPlayer.BirthYear.HasValue ? $"{selectedPlayer.FullName} ({selectedPlayer.BirthYear})" : selectedPlayer.FullName,
                        BestOpponent = bestOpponent != null ? new OpponentStatsViewModel
                        {
                            PlayerId = bestOpponent.PlayerId,
                            FullName = allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).BirthYear.HasValue 
                                ? $"{allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).FullName} ({allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).BirthYear})"
                                : allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).FullName,
                            WinRatio = bestOpponent.WinRatio,
                            TotalMatches = bestOpponent.TotalMatches
                        } : null,
                        WorstOpponent = worstOpponent != null ? new OpponentStatsViewModel
                        {
                            PlayerId = worstOpponent.PlayerId,
                            FullName = allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).BirthYear.HasValue 
                                ? $"{allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).FullName} ({allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).BirthYear})"
                                : allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).FullName,
                            WinRatio = worstOpponent.WinRatio,
                            TotalMatches = worstOpponent.TotalMatches
                        } : null
                    };
                }

                if (player2Id.HasValue)
                {
                    ViewModel.Player2Stats = new PlayerStatisticsViewModel
                    {
                        PlayerId = selectedPlayer.PlayerId,
                        FirstName = selectedPlayer.FirstName,
                        LastName = selectedPlayer.LastName,
                        NumberOfWins = selectedPlayer.NumberOfWins,
                        NumberOfLosses = selectedPlayer.NumberOfLosses,
                        PlayerWinRatio = selectedPlayer.PlayerWinRatio,
                        MatchesPlayed = selectedPlayer.MatchesPlayed,
                        FullName = selectedPlayer.BirthYear.HasValue ? $"{selectedPlayer.FullName} ({selectedPlayer.BirthYear})" : selectedPlayer.FullName,
                        BestOpponent = bestOpponent != null ? new OpponentStatsViewModel
                        {
                            PlayerId = bestOpponent.PlayerId,
                            FullName = allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).BirthYear.HasValue 
                                ? $"{allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).FullName} ({allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).BirthYear})"
                                : allPlayers.First(p => p.PlayerId == bestOpponent.PlayerId).FullName,
                            WinRatio = bestOpponent.WinRatio,
                            TotalMatches = bestOpponent.TotalMatches
                        } : null,
                        WorstOpponent = worstOpponent != null ? new OpponentStatsViewModel
                        {
                            PlayerId = worstOpponent.PlayerId,
                            FullName = allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).BirthYear.HasValue 
                                ? $"{allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).FullName} ({allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).BirthYear})"
                                : allPlayers.First(p => p.PlayerId == worstOpponent.PlayerId).FullName,
                            WinRatio = worstOpponent.WinRatio,
                            TotalMatches = worstOpponent.TotalMatches
                        } : null
                    };
                }
            }

            return Page();
        }
    }
}
