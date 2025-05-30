using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Services.Infrastructure;
using Services.Interfaces;


namespace Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _context;
        public MatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerDTO>> GetAllPlayersAsync()
        {
            return await _context.Players
              .Select(player => new PlayerDTO
              {
                 PlayerId = player.PlayerId,
                 FirstName = player.FirstName,
                 LastName = player.LastName,
                 FullName = $"{player.FirstName} {player.LastName}",
                 Birthday = player.Birthday, // 🟢 Required for BirthYear
                 Email = player.Email,
                 PhoneNumber = player.PhoneNumber,
                 Gender = player.Gender
              })
               .ToListAsync();
        }

        public async Task<PlayerDTO?> GetPlayerByIdAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player == null ? null : new PlayerDTO
            {
                PlayerId = player.PlayerId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                FullName = $"{player.FirstName} {player.LastName}",
                Birthday = player.Birthday,
                Email = player.Email,
                PhoneNumber = player.PhoneNumber,
                Gender = player.Gender
            };
        }

        public async Task<int> CreateMatchAsync(MatchDTO match)
        {
            var newMatch = new Match
            {
                MatchDate = match.MatchDate,
                IsSingle = true,
                IsCompleted = false,
                MatchType = match.MatchType,
                PlayerMatches = new List<PlayerMatch>
                    {
                        new PlayerMatch { PlayerId = match.Player1Id, TeamNumber = 1 },
                        new PlayerMatch { PlayerId = match.Player2Id, TeamNumber = 2 }
                    }
            };
            _context.Matches.Add(newMatch);
            await _context.SaveChangesAsync();

            // Skapa första set direkt
            var firstSet = new Set 
            { 
                MatchId = newMatch.MatchId,
                SetNumber = 1,
                Team1Score = 0,
                Team2Score = 0,
                SetWinner = 0,
                IsSetCompleted = true
            };
            _context.Sets.Add(firstSet);            
            await _context.SaveChangesAsync();

            
            // Skapa SetInfo för första set
            var firstSetInfo = new SetInfo
            {
                SetId = firstSet.SetId,                 
                InfoMessage = string.Empty,
                IsPlayer1Serve = true,
                IsPlayer1StartServer = true,
                ServeCounter = 0
            };
            _context.SetInfos.Add(firstSetInfo);
            await _context.SaveChangesAsync();

            return newMatch.MatchId;
        }

        public async Task<MatchDTO?> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches)
                    .ThenInclude(pm => pm.Player)
                .Include(m => m.Sets)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null) return null;

            var player1 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 1)?.Player;
            var player2 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 2)?.Player;

            var team1WonSets = match.Sets.Count(s => s.SetWinner == 1);
            var team2WonSets = match.Sets.Count(s => s.SetWinner == 2);

            return new MatchDTO
            {
                MatchId = match.MatchId,
                Player1Id = player1?.PlayerId ?? 0,
                Player2Id = player2?.PlayerId ?? 0,
                Player1Name = player1 != null ? $"{player1.FirstName} {player1.LastName}" : string.Empty,
                Player1FirstName = player1 != null ? player1.FirstName : string.Empty,
                Player2Name = player2 != null ? $"{player2.FirstName} {player2.LastName}" : string.Empty,
                Player2FirstName = player2 != null ? player2.FirstName : string.Empty,
                MatchType = match.MatchType,
                MatchDate = match.MatchDate,
                Team1WonSets = team1WonSets,
                Team2WonSets = team2WonSets
            };
        }

        public async Task<bool> IsMatchWonAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Sets)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null) return false;

            var team1WonSets = match.Sets.Count(s => s.SetWinner == 1);
            var team2WonSets = match.Sets.Count(s => s.SetWinner == 2);
            var requiredSets = (match.MatchType / 2) + 1;

            if (team1WonSets >= requiredSets || team2WonSets >= requiredSets)
            {
                if (team1WonSets > team2WonSets)
                {
                    match.MatchWinner = 1;
                }
                else
                {
                    match.MatchWinner = 2;
                }
                return true;
            }
            return false;
        }

        public async Task CompleteMatchAsync(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match != null)
            {
                match.IsCompleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<PagedResult<MatchListDTO>> GetFilteredMatchesAsync(MatchQueryParameters parameters)
        {
            var matches = _context.Matches
                .Include(m => m.PlayerMatches).ThenInclude(pm => pm.Player)
                .Include(m => m.Sets)
                .AsQueryable();

            // Filter by player name
            if (!string.IsNullOrWhiteSpace(parameters.Query))
            {
                var query = parameters.Query.ToLower();
                matches = matches.Where(m =>
                    m.PlayerMatches.Any(pm =>
                        (pm.Player.FirstName + " " + pm.Player.LastName).ToLower().Contains(query)) ||
                    m.MatchId.ToString().Contains(query)
                );
            }

            // Filter by match date
            if (parameters.Date.HasValue)
            {
                matches = matches.Where(m => m.MatchDate.Date == parameters.Date.Value.Date);
            }

            // Project to DTO before pagination
            var projected = matches.Select(m => new MatchListDTO
            {
                MatchId = m.MatchId,
                Sets = m.Sets.ToList(),
                Player1FullName = m.PlayerMatches
                 .Where(pm => pm.TeamNumber == 1)
                 .Select(pm => pm.Player.FirstName + " " + pm.Player.LastName)
                 .FirstOrDefault() ?? "Unknown Player 1",
                 Player2FullName = m.PlayerMatches
                  .Where(pm => pm.TeamNumber == 2)
                  .Select(pm => pm.Player.FirstName + " " + pm.Player.LastName)
                  .FirstOrDefault() ?? "Unknown Player 2",
                 Winner = m.PlayerMatches
                  .Where(pm => pm.TeamNumber == m.MatchWinner)
                  .Select(pm => pm.Player.FirstName + " " + pm.Player.LastName)
                  .FirstOrDefault() ?? "Unknown Winner",
                StartDate = m.MatchDate
            });

            // Apply paging using extension method
            var pagedResult = projected.GetPaged(parameters.PageNumber, parameters.PageSize);
            return pagedResult;
        }
        public async Task<MatchDetailsDTO?> GetMatchDetailsAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches).ThenInclude(pm => pm.Player)
                .Include(m => m.Sets)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null) return null;

            // Convert players to DTOs
            var playerDTOs = match.PlayerMatches.Select(pm => new PlayerDTO
            {
                PlayerId = pm.Player.PlayerId,
                FirstName = pm.Player.FirstName,
                LastName = pm.Player.LastName,
                Email = pm.Player.Email,
                PhoneNumber = pm.Player.PhoneNumber,
                Gender = pm.Player.Gender,
                Birthday = pm.Player.Birthday,
                FullName = $"{pm.Player.FirstName} {pm.Player.LastName}",
                TeamNumber = pm.TeamNumber // Mapped here
            }).ToList();

            // Convert sets to DTOs
            var setDTOs = match.Sets.Select(s => new SetDTO
            {
                SetId = s.SetId,
                MatchId = s.MatchId,
                SetNumber = s.SetNumber,
                Team1Score = s.Team1Score,
                Team2Score = s.Team2Score,
                WinnerId = s.SetWinner,
                IsSetCompleted = s.IsSetCompleted
            }).ToList();

            // Get full name(s) of the winning team
            var winningPlayers = match.PlayerMatches
                .Where(pm => pm.TeamNumber == match.MatchWinner)
                .Select(pm => pm.Player)
                .ToList();

            var winnerName = winningPlayers.Any()
                ? string.Join(" & ", winningPlayers.Select(p => $"{p.FirstName} {p.LastName}"))
                : "Unknown";

            return new MatchDetailsDTO
            {
                MatchId = match.MatchId,
                Players = playerDTOs,
                Sets = setDTOs,
                MatchDate = match.MatchDate,
                Winner = winnerName
            };
        }
        public async Task<EndMatchDTO> GetMatchForEndGameByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches)
                    .ThenInclude(pm => pm.Player)
                .Include(m => m.Sets)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
                return null;

            var player1 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 1)?.Player;
            var player2 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 2)?.Player;

            return new EndMatchDTO
            {
                MatchId = match.MatchId,
                Player1Name = player1 != null ? $"{player1.FirstName} {player1.LastName}" : string.Empty,
                Player2Name = player2 != null ? $"{player2.FirstName} {player2.LastName}" : string.Empty,
                WinnerId = match.MatchWinner,
                WinnerName = match.MatchWinner == 1 ? (player1 != null ? $"{player1.FirstName} {player1.LastName}" : "") : (player2 != null ? $"{player2.FirstName} {player2.LastName}" : ""),
                Sets = match.Sets.OrderBy(s => s.SetNumber).Select(s => new EndMatchDTO.SetResultDTO
                {
                    SetNumber = s.SetNumber,
                    Team1Score = s.Team1Score,
                    Team2Score = s.Team2Score,
                    SetWinner = s.SetWinner
                }).ToList()
            };
        }
        public async Task<MatchDeleteDTO?> GetMatchDeleteDtoAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches).ThenInclude(pm => pm.Player)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null) return null;

            var player1 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 1)?.Player;
            var player2 = match.PlayerMatches.FirstOrDefault(pm => pm.TeamNumber == 2)?.Player;

            var winner = match.MatchWinner switch
            {
                1 => player1 != null ? $"{player1.FirstName} {player1.LastName}" : "Team 1",
                2 => player2 != null ? $"{player2.FirstName} {player2.LastName}" : "Team 2",
                _ => "Unknown"
            };

            return new MatchDeleteDTO
            {
                MatchId = match.MatchId,
                Player1Name = player1 != null ? $"{player1.FirstName} {player1.LastName}" : "Unknown Player 1",
                Player2Name = player2 != null ? $"{player2.FirstName} {player2.LastName}" : "Unknown Player 2",
                MatchDate = match.MatchDate,
                Winner = winner
            };
        }
        public async Task<bool> DeleteMatchAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null) return false;

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<MatchUpdateDTO?> GetMatchForUpdateAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
                return null;

            return new MatchUpdateDTO
            {
                MatchId = match.MatchId,
                MatchDate = match.MatchDate,
                MatchWinner = match.MatchWinner,
                IsSingle = match.IsSingle,
                IsCompleted = match.IsCompleted,
                MatchType = match.MatchType,
                Players = match.PlayerMatches.Select(pm => new PlayerUpdatetDTO
                {
                    PlayerId = pm.PlayerId,
                    TeamNumber = pm.TeamNumber
                }).ToList()
            };
        }
        public async Task<bool> UpdateMatchAsync(MatchUpdateDTO dto)
        {
            var match = await _context.Matches
                .Include(m => m.PlayerMatches)
                .FirstOrDefaultAsync(m => m.MatchId == dto.MatchId);

            if (match == null)
                return false;

            match.MatchDate = dto.MatchDate;
            match.MatchWinner = dto.MatchWinner;
            match.IsSingle = dto.IsSingle;
            match.IsCompleted = dto.IsCompleted;
            match.MatchType = dto.MatchType;

            // Remove existing player assignments
            _context.PlayerMatches.RemoveRange(match.PlayerMatches);

            // Add updated player assignments
            match.PlayerMatches = dto.Players.Select(p => new PlayerMatch
            {
                PlayerId = p.PlayerId,
                TeamNumber = p.TeamNumber,
                MatchId = match.MatchId
            }).ToList();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}


