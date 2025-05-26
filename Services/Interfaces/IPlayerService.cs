using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Check> CreatePlayer(PlayerCreateDTO newPlayer);
        Task<List<PlayerSmallInfoDTO>> GetAllSmallAsync();
    }
}