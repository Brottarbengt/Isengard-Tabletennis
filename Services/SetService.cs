using DataAccessLayer.Data;
using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    

    public class SetService : ISetService
    {
        private readonly ApplicationDbContext _dbContext;
        public SetService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Set?> GetSetByMatchAndNumberAsync(int matchId, int setNumber)
        {
            return await _dbContext.Sets
                .Include(s => s.Match)
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.SetNumber == setNumber);
        }
        public Set? GetSetById(int setId)
        {
            return _dbContext.Sets
                .Include(s => s.Match)
                .FirstOrDefault(s => s.SetId == setId);
        }

        public void CreateSet(Set CurrentSet)
        {
            _dbContext.Sets.Add(CurrentSet);
            _dbContext.SaveChanges();
        }

        public async Task<Set?> GetCurrentSetAsync(int matchId)
        {
            return await _dbContext.Sets
                .Include(s => s.Match)
                .Where(s => s.MatchId == matchId && s.SetWinner == 0)
                .OrderByDescending(s => s.SetNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<Set> CreateNewSetAsync(int matchId)
        {
            bool setServer;
            var lastSet = await _dbContext.Sets
                .Where(s => s.MatchId == matchId)
                .OrderByDescending(s => s.SetNumber)
                .FirstOrDefaultAsync();
            var lastSetSetInfo = await _dbContext.SetInfos
                .Where(s => s.SetId == lastSet.SetId)
                .OrderByDescending(s => s.SetInfoId)
                .FirstOrDefaultAsync();

            setServer = lastSetSetInfo.IsPlayer1StartServer ? false : true;
                        
            var newSetNumber = lastSet?.SetNumber + 1 ?? 1;
            var newSet = new Set
            {
                MatchId = matchId,
                SetNumber = newSetNumber,
                Team1Score = 0,
                Team2Score = 0,
                SetWinner = 0,
                IsSetCompleted = true
            };

            _dbContext.Sets.Add(newSet);
            await _dbContext.SaveChangesAsync();
            
            var newSetInfo = new SetInfo
            {
                SetId = newSet.SetId,
                InfoMessage = string.Empty,
                IsPlayer1Serve = setServer,
                IsPlayer1StartServer = setServer,
                ServeCounter = 0
            };

            _dbContext.SetInfos.Add(newSetInfo);            
            await _dbContext.SaveChangesAsync();
            return newSet;
        }

        public async Task<bool> IsSetWonAsync(Set set)
        {
            if (set.Team1Score < 11 && set.Team2Score < 11) return false;
            
            var scoreDiff = Math.Abs(set.Team1Score - set.Team2Score);
            return scoreDiff >= 2;
        }

        public async Task UpdateSetAsync(Set set)
        {
            _dbContext.Sets.Update(set);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetSetsWonByTeamAsync(int matchId, int teamNumber)
        {
            return await _dbContext.Sets
                .Where(s => s.MatchId == matchId && s.SetWinner == teamNumber)
                .CountAsync();
        }

        public async Task<SetInfo?> GetSetInfoBySetIdAsync(int setId)
        {
            return await _dbContext.SetInfos
                .Include(s => s.Set)
                .FirstOrDefaultAsync(s => s.SetId == setId);
        }

        public async Task<int> GetPreviousSetWinnerAsync(int setId)
        {
            var previousSetId = setId - 1;
            return await _dbContext.Sets
                .Where(s => s.SetId == previousSetId)
                .Select(s => s.SetWinner)
                .FirstOrDefaultAsync();
        }

        public Task UpdateSetInfoAsync(SetInfo setInfo)
        {
            _dbContext.SetInfos.Update(setInfo);
            return _dbContext.SaveChangesAsync();
        }
    }
}
