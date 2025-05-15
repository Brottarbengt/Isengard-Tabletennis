using DataAccessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IMatchService
    {
        Task<List<PlayerCreateDTO>> GetAllPlayersAsync();
        Task<PlayerCreateDTO?> GetPlayerByIdAsync(int playerId);
        Task<int> CreateMatchAsync(CreateMatchDTO match);
    }
}
