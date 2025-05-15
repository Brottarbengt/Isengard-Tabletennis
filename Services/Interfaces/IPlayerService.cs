using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task<Check> CreatePlayer(PlayerDTO newPlayer);
    }
}