using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Check> CreatePlayer(PlayerDTO newPlayer);
        Task<List<PlayerSmallInfoDTO>> GetAllSmallAsync();
        Task<PlayerDTO> GetOneAsync(int playerId);
    }
}