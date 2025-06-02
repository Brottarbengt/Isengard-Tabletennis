using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Infrastructure;


namespace Services.Interfaces
{
    public interface IMatchService
    {
        
        Task<int> CreateMatchAsync(MatchDTO match);
        Task<MatchDTO?> GetMatchByIdAsync(int matchId);
        Task<bool> IsMatchWonAsync(int matchId);
        Task CompleteMatchAsync(int matchId);
        Task<EndMatchDTO> GetMatchForEndGameByIdAsync(int matchId);
        Task UpdateMatchStartTimeAsync(MatchDTO matchDTO);
        Task<MatchDetailsDTO> GetMatchDetailsAsync(int id);
        // Match History //
        Task<PagedResult<MatchListDTO>> GetFilteredMatchesAsync(MatchQueryParameters parameters);
        
        // Match  Delete //
        Task<MatchDeleteDTO?> GetMatchDeleteDtoAsync(int matchId);
        Task<bool> DeleteMatchAsync(int id);

        // Match Update //
        Task<MatchUpdateDTO> GetMatchForUpdateAsync(int matchId);
        Task UpdateMatchAsync(MatchUpdateDTO matchDto);
        Task<List<SelectListItem>> GetPlayerSelectListItemsAsync();


        
    }
}
