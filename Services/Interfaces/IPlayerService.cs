using DataAccessLayer.DTOs;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        Task NewPlayer(PlayerCreateDTO newPlayer);
    }
}