using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Check> CreatePlayer(PlayerDTO newPlayer);
        Task<List<PlayerSmallInfoDTO>> GetAllSmallAsync();
        Task<PlayerDTO> GetOneAsync(int playerId);
        Task<Check> Update(PlayerDTO updatePlayer);
        Task<Check> SoftDelete(int playerId);
        Task<List<PlayerDTO>> GetAllPlayerDTOsAsync();
                
    }
}