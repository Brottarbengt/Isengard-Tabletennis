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
            if (player1Id.HasValue && player2Id.HasValue && player1Id.Value == player2Id.Value)
                player2Id = null;

            ViewModel.SelectedPlayer1Id = player1Id;
            ViewModel.SelectedPlayer2Id = player2Id;
            ViewModel.SearchQuery = searchQuery ?? string.Empty;

            await LoadAllPlayersAndSelectList();

            if (player1Id.HasValue && player2Id.HasValue)
            {
                await LoadHeadToHeadStats(player1Id.Value, player2Id.Value);
            }
            else if (!player1Id.HasValue && !player2Id.HasValue)
            {
                LoadTop10Players();
            }
            else
            {
                await LoadSinglePlayerStats(player1Id ?? player2Id);
            }

            return Page();
        }

        private async Task LoadAllPlayersAndSelectList()
        {
            var allPlayers = await _playerService.GetAllPlayersAsync();
            ViewModel.AllPlayers = allPlayers;
            ViewModel.PlayerSelectList = allPlayers
                .Select(p => new SelectListItem
                {
                    Value = p.PlayerId.ToString(),
                    Text = p.BirthYear.HasValue ? $"{p.FullName} ({p.BirthYear})" : p.FullName
                })
                .ToList();
        }

        private async Task LoadHeadToHeadStats(int player1Id, int player2Id)
        {
            var player1 = await _playerService.GetPlayerByIdAsync(player1Id);
            var player2 = await _playerService.GetPlayerByIdAsync(player2Id);

            var matches = await _matchService.GetFilteredMatchesAsync(new Services.Infrastructure.MatchQueryParameters
            {
                PageSize = 1000
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

            var matchDurations = headToHeadMatches
                .Where(m => m.DurationSeconds.HasValue && m.DurationSeconds.Value > 0)
                .Select(m => new
                {
                    Duration = TimeSpan.FromSeconds(m.DurationSeconds.Value),
                    MatchDate = m.MatchDate
                })
                .ToList();

            var longestMatch = matchDurations.OrderByDescending(m => m.Duration).FirstOrDefault();
            var shortestMatch = matchDurations.OrderBy(m => m.Duration).FirstOrDefault();

            ViewModel.VsStats = new VsPlayerStatsViewModel
            {
                Player1 = player1.Adapt<PlayerStatisticsViewModel>(),
                Player2 = player2.Adapt<PlayerStatisticsViewModel>(),
                Player1Wins = player1Wins,
                Player2Wins = player2Wins,
                TotalMatches = totalMatches,
                Player1WinRatio = totalMatches > 0 ? (decimal)player1Wins / totalMatches * 100 : 0,
                Player2WinRatio = totalMatches > 0 ? (decimal)player2Wins / totalMatches * 100 : 0,
                LongestMatchDuration = longestMatch?.Duration ?? TimeSpan.Zero,
                ShortestMatchDuration = shortestMatch?.Duration ?? TimeSpan.Zero,
                LongestMatchDate = longestMatch?.MatchDate ?? DateTime.MinValue,
                ShortestMatchDate = shortestMatch?.MatchDate ?? DateTime.MinValue
            };
        }

        private void LoadTop10Players()
        {
            var allPlayers = ViewModel.AllPlayers;
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

        private async Task LoadSinglePlayerStats(int? playerId)
        {
            if (!playerId.HasValue) return;

            var selectedPlayer = await _playerService.GetPlayerByIdAsync(playerId.Value);
            var allPlayers = ViewModel.AllPlayers;

            var matches = await _matchService.GetFilteredMatchesAsync(new Services.Infrastructure.MatchQueryParameters
            {
                PageSize = 1000
            });

            var playerMatches = matches.Results
                .Select(m => _matchService.GetMatchByIdAsync(m.MatchId).Result)
                .Where(m => m != null && (m.Player1Id == playerId || m.Player2Id == playerId))
                .ToList();

            var matchDurations = playerMatches
                .Where(m => m.DurationSeconds.HasValue && m.DurationSeconds.Value > 0)
                .Select(m => new
                {
                    Duration = TimeSpan.FromSeconds(m.DurationSeconds.Value),
                    Opponent = allPlayers.First(p => p.PlayerId == (m.Player1Id == playerId ? m.Player2Id : m.Player1Id))
                })
                .ToList();

            var longestMatch = matchDurations.OrderByDescending(m => m.Duration).FirstOrDefault();
            var shortestMatch = matchDurations.OrderBy(m => m.Duration).FirstOrDefault();

            var opponentStats = new Dictionary<int, (int wins, int total)>();
            foreach (var match in playerMatches)
            {
                var opponentId = match.Player1Id == playerId ? match.Player2Id : match.Player1Id;
                if (!opponentStats.ContainsKey(opponentId))
                {
                    opponentStats[opponentId] = (0, 0);
                }

                var stats = opponentStats[opponentId];
                stats.total++;
                if ((match.Player1Id == playerId && match.Team1WonSets > match.Team2WonSets) ||
                    (match.Player2Id == playerId && match.Team2WonSets > match.Team1WonSets))
                {
                    stats.wins++;
                }
                opponentStats[opponentId] = stats;
            }

            var opponentRatios = opponentStats
                .Select(kvp => new
                {
                    PlayerId = kvp.Key,
                    WinRatio = kvp.Value.total > 0 ? (decimal)kvp.Value.wins / kvp.Value.total * 100 : 0,
                    TotalMatches = kvp.Value.total
                })
                .Where(x => x.TotalMatches >= 3)
                .OrderByDescending(x => x.WinRatio)
                .ToList();

            var bestOpponent = opponentRatios.FirstOrDefault();
            var worstOpponent = opponentRatios.LastOrDefault();

            var statsViewModel = new PlayerStatisticsViewModel
            {
                PlayerId = selectedPlayer.PlayerId,
                FirstName = selectedPlayer.FirstName,
                LastName = selectedPlayer.LastName,
                NumberOfWins = selectedPlayer.NumberOfWins,
                NumberOfLosses = selectedPlayer.NumberOfLosses,
                PlayerWinRatio = selectedPlayer.PlayerWinRatio,
                MatchesPlayed = selectedPlayer.MatchesPlayed,
                FullName = selectedPlayer.BirthYear.HasValue ? $"{selectedPlayer.FullName} ({selectedPlayer.BirthYear})" : selectedPlayer.FullName,
                LongestMatchDuration = longestMatch?.Duration ?? TimeSpan.Zero,
                ShortestMatchDuration = shortestMatch?.Duration ?? TimeSpan.Zero,
                LongestMatchOpponent = longestMatch?.Opponent.BirthYear.HasValue == true
                    ? $"{longestMatch.Opponent.FullName} ({longestMatch.Opponent.BirthYear})"
                    : longestMatch?.Opponent.FullName,
                ShortestMatchOpponent = shortestMatch?.Opponent.BirthYear.HasValue == true
                    ? $"{shortestMatch.Opponent.FullName} ({shortestMatch.Opponent.BirthYear})"
                    : shortestMatch?.Opponent.FullName,
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

            if (ViewModel.SelectedPlayer1Id.HasValue)
                ViewModel.Player1Stats = statsViewModel;
            if (ViewModel.SelectedPlayer2Id.HasValue)
                ViewModel.Player2Stats = statsViewModel;
        }
    }
}
