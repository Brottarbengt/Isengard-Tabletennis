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

namespace Services
{
    public class PlayerService : IPlayerService
    {
        public readonly ApplicationDbContext _dbContext;
        public PlayerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task NewPlayer(PlayerCreateDTO newPlayer)
        {


            //lägga till mapping
            //return check efter modelstate isvalid

            var player = new Player
            {
                FirstName = newPlayer.FirstName,
                LastName = newPlayer.LastName,
                Email = newPlayer.Email,
                PhoneNumber = newPlayer.PhoneNumber,
                Gender = newPlayer.Gender,
                Birthday = newPlayer.Birthday
            };

            _dbContext.Players.Add(player);
            await _dbContext.SaveChangesAsync();
        }


    }
}
