using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Check> CreatePlayer(PlayerDTO newPlayer);
        Task<List<PlayerSmallInfoDTO>> GetAllSmallAsync();
        Task<PlayerDTO> GetPlayerByIdAsync(int playerId);
        Task<Check> Update(PlayerDTO updatePlayer);
        Task<Check> SoftDelete(int playerId);
        Task<List<PlayerDTO>> GetAllPlayersAsync();
        Task<decimal> SetPlayerWinRatioAsync(int playerId);

    }
}