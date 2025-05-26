using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Services.Infrastructure;


namespace Services.Interfaces
{
    public interface IMatchService
    {
        Task<List<PlayerDTO>> GetAllPlayersAsync();
        Task<PlayerDTO?> GetPlayerByIdAsync(int playerId);
        Task<int> CreateMatchAsync(MatchDTO match);
        Task<MatchDTO?> GetMatchByIdAsync(int matchId);
        Task<bool> IsMatchWonAsync(int matchId);
        Task CompleteMatchAsync(int matchId);
        Task<EndMatchDTO> GetMatchForEndGameByIdAsync(int matchId);
        Task UpdateMatchAsync(MatchDTO matchDTO);
        Task<MatchDetailsDTO> GetMatchDetailsAsync(int id);
        Task<PagedResult<MatchListDTO>> GetFilteredMatchesAsync(MatchQueryParameters parameters);
        //ActiveMatchDTO GetMatchById(int matchId);
    }
}
