using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .Select(p => new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .ToListAsync();
        }

        public async Task<PlayerDTO?> GetPlayerByIdAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player == null ? null : new PlayerDTO
            {
                PlayerId = player.PlayerId,
                FullName = player.FirstName + " " + player.LastName
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
                SetWinner = 0
            };
            _context.Sets.Add(firstSet);
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
                Player2Name = player2 != null ? $"{player2.FirstName} {player2.LastName}" : string.Empty,
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

            return team1WonSets >= requiredSets || team2WonSets >= requiredSets;
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
    }
}

