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

namespace Tabletennis.Pages.Match
{
    //TODO: Refactor to clean out unplayed sets when match is completed
    //TODO: Add logic to set IsDecidingSet on save
    //TODO: Show serve owner
    //TODO: Button reveal on match complete
    public class ActiveMatchModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly ISetService _setService;

        public ActiveMatchModel(IMatchService matchService, ISetService setService)
        {
            _matchService = matchService;
            _setService = setService;
        }

        
        public LiveScore LiveScore { get; set; }
        public int SetNumber { get; set; }
        public int Team1SetsWon { get; set; }
        public int Team2SetsWon { get; set; }
        public int CurrentSetId { get; set; }
        public Set CurrentSet { get; set; }
        public ActiveMatchViewModel ActiveMatchVM { get; set; } = new();

        //TODO: Refactor OnGet to get data from DB using matchId
        //TODO: Use DTOs
        //TODO: Single source of truth!!! Needs refactoring

        // OnGet incoming survives page reload, build around getting all data from DB using matchId for Single source of truth
        // saving to DB after each point. 
        // Current design vulnarable to insertion attacks in page URL
        public async Task OnGetAsync(int matchId, string player1, string player2, int player1Id, int player2Id, DateTime matchDate, int matchType)
        {
            ActiveMatchVM.Player1 = player1;
            ActiveMatchVM.Player2 = player2;
            ActiveMatchVM.MatchId = matchId;
            ActiveMatchVM.Player1Id = player1Id;
            ActiveMatchVM.Player2Id = player2Id;
            ActiveMatchVM.MatchDate = matchDate;
            ActiveMatchVM.MatchType = matchType;

            MatchVM = match.Adapt<ActiveMatchViewModel>();            
            
            var currentSet = await _setService.GetCurrentSetAsync(matchId);
            if (currentSet != null)
            {
                MatchVM.SetId = currentSet.SetId;
                MatchVM.Team1Score = currentSet.Team1Score;
                MatchVM.Team2Score = currentSet.Team2Score;
                MatchVM.SetNumber = currentSet.SetNumber;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateScoreAsync(int matchId, int teamNumber, bool isIncrement)
        {
            var currentSet = await _setService.GetCurrentSetAsync(matchId);
            if (currentSet == null)
            {                
                currentSet = await _setService.CreateNewSetAsync(matchId);
            }
            
            if (teamNumber == 1)
            {
                if (isIncrement || currentSet.Team1Score > 0)
                {
                    currentSet.Team1Score += isIncrement ? 1 : -1;
                }
            }
            else
            {
                if (isIncrement || currentSet.Team2Score > 0)
                {
                    currentSet.Team2Score += isIncrement ? 1 : -1;
                }
            }
            
            if (await _setService.IsSetWonAsync(currentSet))
            {
                currentSet.SetWinner = currentSet.Team1Score > currentSet.Team2Score ? 1 : 2;
                await _setService.UpdateSetAsync(currentSet);

                // Kontrollera om matchen är vunnen
                if (await _matchService.IsMatchWonAsync(matchId))
                {
                    await _matchService.CompleteMatchAsync(matchId);
                    TempData["SuccessMessage"] = "Match completed!";
                    return RedirectToPage("/Matches/CreateMatch");
                }
                
                await _setService.CreateNewSetAsync(matchId);
            }
            else
            {
                await _setService.UpdateSetAsync(currentSet);
            }

            return RedirectToPage(new { matchId });
        }
    }
}