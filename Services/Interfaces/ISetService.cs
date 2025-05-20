using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISetService
    {
        Set? GetSetById(int setId);
        void CreateSet(Set currentSet);
        Task<Set?> GetSetByMatchAndNumberAsync(int matchId, int setNumber);
        Task<Set?> GetCurrentSetAsync(int matchId);
        Task<Set> CreateNewSetAsync(int matchId);
        Task<bool> IsSetWonAsync(Set set);
        Task UpdateSetAsync(Set set);
        Task<int> GetSetsWonByTeamAsync(int matchId, int teamNumber);
    }
}
