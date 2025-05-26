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
            var players = await _dbContext.Players.ToListAsync();
            return players.Adapt<List<PlayerSmallInfoDTO>>();
        }


    }
}
