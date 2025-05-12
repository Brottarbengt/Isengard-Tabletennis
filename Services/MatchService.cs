using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
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
        public async Task<int> CreateMatchAsync(CreateMatchDTO dto)
        {
            //Create Player 1 
            var player1 = new Player
            {
                FirstName = dto.Player1FirstName,
                LastName = dto.Player1LastName,
                Email = "", //Optional: populate later
                PhoneNumber = "",
                Birthday = new DateOnly(2000, 1, 1) //placeholder date
            };
            //Create Player 2 
            var player2 = new Player
            {
                FirstName = dto.Player2FirstName,
                LastName = dto.Player2LastName,
                Email = "",
                PhoneNumber = "",
                Birthday = new DateOnly(2000, 1, 1)
            };
            // Create Match 
            var match = new Match
            {
                MatchDate = dto.MatchDate,
                IsSingle = dto.IsSingle,
                IsCompleted = false,
                PlayerMatches = new List<PlayerMatch>
                {
                    new PlayerMatch {Player = player1, TeamNumber = 1},
                    new PlayerMatch {Player = player2, TeamNumber = 2}
                },
                Sets = new List<Set>()
            };
            // Add placeholder sets (based on BestOfSets)
            for (int i = 1; i <= dto.BestOfSets; i++)
            {
                match.Sets.Add(new Set
                {
                    SetNumber = i,
                    Team1Score = 0,
                    Team2Score = 0,
                    IsDecidingSet = false
                    // WinnerId will be set after playing
                });

            }
            // Save everything to database
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return match.MatchId;
        }
    }
}

