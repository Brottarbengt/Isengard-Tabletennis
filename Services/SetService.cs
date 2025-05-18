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
            var lastSet = await _dbContext.Sets
                .Where(s => s.MatchId == matchId)
                .OrderByDescending(s => s.SetNumber)
                .FirstOrDefaultAsync();

            var newSetNumber = lastSet?.SetNumber + 1 ?? 1;
            var newSet = new Set
            {
                MatchId = matchId,
                SetNumber = newSetNumber,
                Team1Score = 0,
                Team2Score = 0,
                SetWinner = 0
            };

            _dbContext.Sets.Add(newSet);
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
    }
}
