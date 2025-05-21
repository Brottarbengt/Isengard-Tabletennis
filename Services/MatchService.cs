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
              .Select(player => new PlayerCreateDTO
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

        public async Task<PlayerCreateDTO?> GetPlayerByIdAsync(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return null;

            return new PlayerCreateDTO
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
        public async Task CreateMatchAsync(CreateMatchDTO match)
        {
            var newMatch = new Match
            {
                MatchDate = match.MatchDate,
                IsSingle = true,
                IsCompleted = false,
                PlayerMatches = new List<PlayerMatch>
            {
                new PlayerMatch { PlayerId = match.Player1Id, TeamNumber = 1 },
                new PlayerMatch { PlayerId = match.Player2Id, TeamNumber = 2 }
            },
                Sets = Enumerable.Range(1, match.SetCount)
                    .Select(i => new Set { SetNumber = i })
                    .ToList()
            };

            _context.Matches.Add(newMatch);
            await _context.SaveChangesAsync();
        }
    }
}

