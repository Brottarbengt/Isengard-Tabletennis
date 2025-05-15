using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
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
        public async Task<List<PlayerCreateDTO>> GetAllPlayersAsync()
        {
            return await _context.Players
                .Select(p => new PlayerCreateDTO
                {
                    PlayerId = p.PlayerId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .ToListAsync();
        }

        public async Task<PlayerCreateDTO?> GetPlayerByIdAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            return player == null ? null : new PlayerCreateDTO
            {
                PlayerId = player.PlayerId,
                FullName = player.FirstName + " " + player.LastName
            };
        }

        public async Task<int> CreateMatchAsync(CreateMatchDTO match)
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
            },
                // Sets = Enumerable.Range(1, match.MatchType)
                //     .Select(i => new Set { SetNumber = i })
                //     .ToList()
            };

            _context.Matches.Add(newMatch);
            await _context.SaveChangesAsync();
            return newMatch.MatchId;
        }
    }
}

