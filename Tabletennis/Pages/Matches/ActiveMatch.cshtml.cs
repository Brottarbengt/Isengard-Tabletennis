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
    //TODO: Refactor to clean out unplayed sets when match is completed
    //TODO: Add logic to set IsDecidingSet on save
    //TODO: Show serve owner
    //TODO: Button reveal on match complete, presenting winner and links back to CreateMatches
    //TODO: Add prompt if press Tillbaka during match, if discard match?
    public class ActiveMatchModel : PageModel
    {
        private readonly IMatchService _matchService;
        private readonly ISetService _setService;

        public ActiveMatchModel(IMatchService matchService, ISetService setService)
        {
            _matchService = matchService;
            _setService = setService;
        }

        [BindProperty]
        public ActiveMatchViewModel ActiveMatchVM { get; set; } = new();
       
        public async Task<IActionResult> OnGetAsync(int matchId)
        {
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null)
            {
                return NotFound();
            }

            ActiveMatchVM = match.Adapt<ActiveMatchViewModel>();            
            
            var currentSet = await _setService.GetCurrentSetAsync(matchId);
            if (currentSet != null)
            {
                ActiveMatchVM.SetId = currentSet.SetId;
                ActiveMatchVM.Team1Score = currentSet.Team1Score;
                ActiveMatchVM.Team2Score = currentSet.Team2Score;
                ActiveMatchVM.SetNumber = currentSet.SetNumber;
                ActiveMatchVM.InfoMessage = currentSet.InfoMessage;
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

            CheckInfo(currentSet.Team1Score, currentSet.Team2Score, currentSet);

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

        public void CheckInfo(int team1score, int team2score, Set currentSet)
        {
            int setsToWin = (ActiveMatchVM.MatchType / 2) + 1;
            int team1SetsWon = ActiveMatchVM.Team1WonSets;
            int team2SetsWon = ActiveMatchVM.Team2WonSets;

            if (team1score == team2score && team1score >= 10)
            {
                currentSet.InfoMessage = "Deuce";
            }


            //if (
            //       (team1SetsWon == setsToWin - 1 && team1score > 9 && team1score > team2score) ||
            //       (team2SetsWon == setsToWin - 1 && team2score > 9 && team2score > team1score)
            //   )
            //{
            //    currentSet.InfoMessage = "Match Point";
            //}

            else if ((team1score > 9 || team2score > 9) && (team1score > team2score || team2score > team1score))
            {
                currentSet.InfoMessage = "Set Point";
            }
            
        }
    }
}