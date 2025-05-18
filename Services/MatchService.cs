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

            newMatch.Sets = Enumerable.Range(1, match.MatchType)
                .Select(i => new Set { SetNumber = i, Match = newMatch })
                .ToList();
            
            await _context.SaveChangesAsync();
            return newMatch.MatchId;
        }

        public async Task<ActiveMatchDTO?> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .Where(m => m.MatchId == matchId)
                .ProjectToType<MatchDTO>() // Mapster magic here
                .FirstOrDefaultAsync();

            return match?.Adapt<ActiveMatchDTO>();
        }
    }
}

