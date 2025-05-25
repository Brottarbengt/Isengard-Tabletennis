using DataAccessLayer.DTOs;
using Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<PagedResult<MatchListDTO>> GetFilteredMatchesAsync(MatchQueryParameters parameters);

        //ActiveMatchDTO GetMatchById(int matchId);
    }
}
