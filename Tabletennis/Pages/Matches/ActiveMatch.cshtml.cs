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
    [BindProperties]
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

        
        public LiveScore LiveScore { get; set; }
        public int SetNumber { get; set; }
        public int Team1SetsWon { get; set; }
        public int Team2SetsWon { get; set; }
        public int MatchId { get; set; } //överflödig?
        public Set CurrentSet { get; set; }        
        public ActiveMatchViewModel ActiveMatchVM { get; set; } = new();

        public async Task OnGetAsync(int matchId, string player1, string player2, int player1Id, int player2Id, DateTime matchDate, int matchType)
        {
            ActiveMatchVM.Player1 = player1;
            ActiveMatchVM.Player2 = player2;
            ActiveMatchVM.MatchId = matchId;
            MatchId = matchId;
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
            // Check if set already exists
            var existingSet = await _setService.GetSetByMatchAndNumberAsync(matchId, setNumber);
            if (existingSet != null)
            {
                CurrentSet = existingSet;
            }
            else
            {
                CurrentSet = new Set
                {
                    MatchId = matchId,
                    SetNumber = setNumber,
                    Team1Score = 0,
                    Team2Score = 0,
                    SetWinner = 0,
                    IsDecidingSet = false
                };

                _setService.CreateSet(CurrentSet);
            }

            var setId = CurrentSet.SetId;
            ActiveMatchVM.SetId = setId;
            LiveScore = liveScores.GetValueOrDefault(setId) ?? new LiveScore { SetId = setId, CurrentSetNumber = setNumber, MatchId = matchId };
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostStartNewSet(int setNumber, int matchId)
        {
            await StartNewSetAsync(setNumber, matchId);
            return new JsonResult(new
            {
                setNumber = CurrentSet.SetNumber,
                team1Points = CurrentSet.Team1Score,
                team2Points = CurrentSet.Team2Score
            });
        }

        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OnPostAddPoint(int setId, int team)
        {
            try
            {
                if (CurrentSet == null || CurrentSet.SetId != setId)
                {
                    CurrentSet =  _setService.GetSetById(setId);
                    if (CurrentSet == null)
                        return new JsonResult(new { error = "Set not found." }) { StatusCode = 404 };
                }

                var score = liveScores.GetOrAdd(setId, _ => new LiveScore { SetId = setId, CurrentSetNumber = CurrentSet.SetNumber, MatchId = CurrentSet.MatchId });

                if (team == 1) score.Team1Points++;
                else if (team == 2) score.Team2Points++;

                bool endOfSet = false;
                if ((score.Team1Points >= 11 || score.Team2Points >= 11) && Math.Abs(score.Team1Points - score.Team2Points) >= 2)
                {
                    if (score.Team1Points > score.Team2Points)
                    {
                        CurrentSet.SetWinner = 1;
                        Team1SetsWon++;
                    }
                    else
                    {
                        CurrentSet.SetWinner = 2;
                        Team2SetsWon++;
                    }

                    _setService.SaveSet(setId, score, CurrentSet.SetWinner);
                    score.CurrentSetNumber++;
                    SetNumber = score.CurrentSetNumber;
                    await StartNewSetAsync(SetNumber, score.MatchId);
                    endOfSet = true;
                }

                return new JsonResult(new
                {
                    team1Points = score.Team1Points,
                    team2Points = score.Team2Points,
                    currentSetNumber = score.CurrentSetNumber,
                    endOfSet
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}