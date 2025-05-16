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
        public readonly ApplicationDbContext _dbContext;
        public SetService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateSet(Set CurrentSet)
        {
            _dbContext.Sets.Add(CurrentSet);
            _dbContext.SaveChanges();
        }

        public void SaveSet(int setId, LiveScore score, int player1Id, int player2Id)
        {
            int winnerId;

            if (score.Team1Points > score.Team2Points)
            {
                winnerId = player1Id;
            }
            else 
            {
                winnerId = player2Id;
            }

            var existingSet = _dbContext.Sets.Find(setId);
            if (existingSet != null)
            {
                existingSet.WinnerId = winnerId;
                existingSet.Team1Score = score.Team1Points;
                existingSet.Team2Score = score.Team2Points;

                _dbContext.SaveChanges();
            }
        }

        //public SetDTO? GetCurrentSetForMatch(int matchId)
        //{
        //    // Hämta det set för matchen som inte har en vinnare (pågående set)
        //    var currentSet = _dbContext.Sets
        //        .Where(s => s.MatchId == matchId && s.WinnerId == 0)
        //        .OrderByDescending(s => s.SetNumber)
        //        .FirstOrDefaultAsync();

        //    // Om alla set är avslutade, ta det senaste setet
        //    if (currentSet == null)
        //    {
        //        currentSet = _dbContext.Sets
        //            .Where(s => s.MatchId == matchId)
        //            .OrderByDescending(s => s.SetNumber)
        //            .FirstOrDefaultAsync();
        //    }

        //    return currentSet?.Adapt<SetDTO>();
        //}





    }
}
