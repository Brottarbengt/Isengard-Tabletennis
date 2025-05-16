using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches
{
    public class ActiveMatchModel : PageModel

    {
        private readonly ISetService _setService;
        private readonly IMatchService _matchService;
        private static readonly ConcurrentDictionary<int, LiveScore> liveScores = new();

        public ActiveMatchModel(ISetService setService, IMatchService matchService)
        {
            _setService = setService;
            _matchService = matchService;
        }

        

        [BindProperty]
        public LiveScore LiveScore { get; set; }
        public int SetNumber { get; set; }
        public Set CurrentSet { get; set; }        
        public ActiveMatchViewModel ActiveMatchVM { get; set; } = new();

        public async Task OnGetAsync(int matchId, string player1, string player2, int player1Id, int player2Id, DateTime matchDate, int matchType)
        {

            ActiveMatchVM.Player1 = player1;
            ActiveMatchVM.Player2 = player2;
            ActiveMatchVM.MatchId = matchId;
            ActiveMatchVM.Player1Id = player1Id;
            ActiveMatchVM.Player2Id = player2Id;
            ActiveMatchVM.MatchDate = matchDate;
            ActiveMatchVM.MatchType = matchType;
            
            if (CurrentSet == null)
            {
                SetNumber = 1;
                await StartNewSetAsync(SetNumber, matchId);                
            }
        }

        private async Task StartNewSetAsync(int setNumber, int matchId)
        {
                        
            CurrentSet = new Set
            {
                MatchId = matchId,
                SetNumber = setNumber,
                Team1Score = 0,
                Team2Score = 0,
                WinnerId = 0,
                IsDecidingSet = false
                
            };

            _setService.CreateSet(CurrentSet);

            var setId = CurrentSet.SetId;
            ActiveMatchVM.SetId = setId;
            LiveScore = liveScores.GetValueOrDefault(setId) ?? new LiveScore { SetId = setId };
        }

        public JsonResult OnPostAddPoint(int setId, int team)
        {
            var score = liveScores.GetOrAdd(setId, _ => new LiveScore { SetId = setId });

            if (team == 1) score.Team1Points++;
            else if (team == 2) score.Team2Points++;

            if ((score.Team1Points >= 11 || score.Team2Points >= 11) && Math.Abs(score.Team1Points - score.Team2Points) >= 2)
            {
                _setService.SaveSet(setId, score, ActiveMatchVM.Player1Id, ActiveMatchVM.Player2Id);
                score.Team1Points = 0;
                score.Team2Points = 0;
                score.CurrentSetNumber++;
            }

            return new JsonResult(score);
        }
                
    }
}