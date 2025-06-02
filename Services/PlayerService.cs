using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Humanizer;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class PlayerService : IPlayerService
    {
        public readonly ApplicationDbContext _dbContext;
        public PlayerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Check> CreatePlayer(PlayerDTO newPlayer)
        {

            if (newPlayer == null)
            {
                return Check.Failed;
            }

            var player = newPlayer.Adapt<Player>();

            if (player == null)
            {
                return Check.Failed;
            }


            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync();
            return Check.Success;
        }

        public async Task<List<PlayerSmallInfoDTO>> GetAllSmallAsync()
        {
            var players = await _dbContext.Players.Where(c => c.IsActive == true).ToListAsync();
            return players.Adapt<List<PlayerSmallInfoDTO>>();
        }

        public async Task<PlayerDTO> GetPlayerByIdAsync(int playerId)
        {
            var playerDb = await _dbContext.Players.FindAsync(playerId);
            var playerDTO = playerDb.Adapt<PlayerDTO>();
            return playerDTO;
        }

        public async Task<Check> Update(PlayerDTO updatePlayer)
        {
            var playerToUpdate = await _dbContext.Players.FindAsync(updatePlayer.PlayerId);
            if (playerToUpdate == null)
            { return Check.Failed; }

            playerToUpdate.FirstName = updatePlayer.FirstName;
            playerToUpdate.LastName = updatePlayer.LastName;
            playerToUpdate.Email = updatePlayer.Email;
            playerToUpdate.PhoneNumber = updatePlayer.PhoneNumber;
            playerToUpdate.Gender = updatePlayer.Gender;
            playerToUpdate.Birthday = updatePlayer.Birthday;

            await _dbContext.SaveChangesAsync();
            return Check.Success;


        }

        public async Task<Check> SoftDelete(int playerId)
        {
            var playerToDelete = await _dbContext.Players.FindAsync(playerId);
            if (playerToDelete == null)
            { return Check.Failed; }

            playerToDelete.IsActive = false;

            await _dbContext.SaveChangesAsync();
            return Check.Success;

        }

        public async Task<decimal> SetPlayerWinRatioAsync(int playerId)
        {
            var player = await _dbContext.Players
                .FirstOrDefaultAsync(p => p.PlayerId == playerId);


            decimal winRatio = (decimal)player.NumberOfWins / (decimal)player.MatchesPlayed;
            return Math.Round(winRatio * 100 , 2);

        }

        public Task<List<PlayerDTO>> GetAllPlayersAsync()
        {
            var players = _dbContext.Players
                .Where(p => p.IsActive)
                .Select(p => p.Adapt<PlayerDTO>())
                .ToListAsync();
            return players;

        }

        public async Task<List<PlayerDTO>> GetTop10Players()
        {
            var top10 = await _dbContext.Players
                .OrderByDescending(p => p.PlayerWinRatio)
                .Take(10)
                .Select(p => new PlayerDTO
                    {
                        PlayerId = p.PlayerId,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        PlayerWinRatio = p.PlayerWinRatio,
                        MatchesPlayed = p.MatchesPlayed,
                        NumberOfWins = p.NumberOfWins,
            
                    }).ToListAsync();

            return top10;
        }
    }
}
