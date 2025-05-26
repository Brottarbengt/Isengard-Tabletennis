using DataAccessLayer.DTOs;


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
        
        Task<PagedResult<MatchListDTO>> GetFilteredMatchesAsync(MatchQueryParameters parameters);
        Task<MatchDetailsDTO?> GetMatchDetailsAsync(int matchId);

        //ActiveMatchDTO GetMatchById(int matchId);
    }
}
