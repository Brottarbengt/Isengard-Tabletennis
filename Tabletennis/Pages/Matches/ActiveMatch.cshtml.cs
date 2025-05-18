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
using System.Security.Cryptography;
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
        public int CurrentSetId { get; set; }
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

            await StartNewSetAsync(ActiveMatchVM.MatchId);
        }

        private async Task StartNewSetAsync(int matchId)
        {
            if (CurrentSet == null || CurrentSet.Match == null)
            {
                SetNumber = 1;
                CurrentSet = await _setService.GetSetByMatchAndNumberAsync(matchId, SetNumber);
            }
            else
            {
                CurrentSet.SetNumber++;
                SetNumber = CurrentSet.SetNumber;
                CurrentSet = await _setService.GetSetByMatchAndNumberAsync(CurrentSet.MatchId, CurrentSet.SetNumber);
            }

            CurrentSetId = CurrentSet.SetId;
            LiveScore = liveScores.GetValueOrDefault(CurrentSet.SetId) ?? new LiveScore { SetId = CurrentSet.SetId, CurrentSetNumber = CurrentSet.SetNumber, MatchId = CurrentSet.MatchId };
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
                bool isEndOfSet = false;
                int newSetId = 0;
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
                    CurrentSet = _setService.GetSetById(setId);
                    await StartNewSetAsync(CurrentSet.MatchId);
                    isEndOfSet = true;
                    newSetId = CurrentSet.SetId;
                }

                return new JsonResult(new
                {
                    team1Points = score.Team1Points,
                    team2Points = score.Team2Points,
                    currentSetNumber = score.CurrentSetNumber,
                    isEndOfSet,
                    newSetId
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}