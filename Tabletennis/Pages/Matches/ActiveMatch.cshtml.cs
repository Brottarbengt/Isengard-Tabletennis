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
                var currentSetInfo = await _setService.GetSetInfoBySetIdAsync(currentSet.SetId);

                ActiveMatchVM.SetId = currentSet.SetId;
                ActiveMatchVM.Team1Score = currentSet.Team1Score;
                ActiveMatchVM.Team2Score = currentSet.Team2Score;
                ActiveMatchVM.SetNumber = currentSet.SetNumber;
                ActiveMatchVM.InfoMessage = currentSetInfo.InfoMessage;
                ActiveMatchVM.IsPlayer1Serve = currentSetInfo.IsPlayer1Serve;

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
            
            var currentSetInfo = await _setService.GetSetInfoBySetIdAsync(currentSet.SetId);
            

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

            
            await CheckInfoAsync(currentSet.Team1Score, currentSet.Team2Score, currentSetInfo,  currentSet, matchId);
            SetServer(currentSetInfo);

            if (await _setService.IsSetWonAsync(currentSet))
            {
                currentSet.SetWinner = currentSet.Team1Score > currentSet.Team2Score ? 1 : 2;
                await _setService.UpdateSetAsync(currentSet);

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

        
        public async Task CheckInfoAsync(int team1score, int team2score, SetInfo currentSetInfo, Set currentSet, int matchId)
        {
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null) return;

            int setsToWin = (match.MatchType / 2) + 1;
            int team1SetsWon = await _setService.GetSetsWonByTeamAsync(matchId, 1);
            int team2SetsWon = await _setService.GetSetsWonByTeamAsync(matchId, 2);


            if (team1score == team2score && team1score >= 10)
            {
                currentSetInfo.InfoMessage = "Deuce";
            }
            else if (
                (team1SetsWon == setsToWin - 1 && team1score >= 10 && team1score > team2score) ||
                (team2SetsWon == setsToWin - 1 && team2score >= 10 && team2score > team1score)
            )
            {
                currentSetInfo.InfoMessage = "Match Point";
            }
            else if ((team1score >= 10 || team2score >= 10) && Math.Abs(team1score - team2score) >= 1)
            {
                currentSetInfo.InfoMessage = "Set Point";
            }
        }

        private void SetServer(SetInfo currentSetInfo)
        {
            currentSetInfo.ServeCounter++;
            if (currentSetInfo.InfoMessage == "Deuce")
            {
                currentSetInfo.IsPlayer1Serve = !currentSetInfo.IsPlayer1Serve;
                currentSetInfo.ServeCounter = 1;
            }
            else if (currentSetInfo.ServeCounter == 2)
            {
                currentSetInfo.IsPlayer1Serve = !currentSetInfo.IsPlayer1Serve;
                currentSetInfo.ServeCounter = 0;
            }

        }
    }
}