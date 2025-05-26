using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches
{
    //TODO: Refactor to clean out unplayed sets (and match?) on abort OR should they be kept for 
    //      possibility to continue the match later?
    //TODO: Add decrease button to decrease score and working logic with SetServe()
    //TODO: Button reveal on match complete, presenting winner and links back to CreateMatches
    //TODO: Add prompt if press Tillbaka during match, if discard match?
    //TODO: Need to reset team/player set winner on match complete, otherwise it will be shown with "Starta första set" in next match
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
            ActiveMatchVM.StartTime = match.StartTime;
            
            var currentSet = await _setService.GetCurrentSetAsync(matchId);
            if (currentSet != null)
            {
                var currentSetInfo = await _setService.GetSetInfoBySetIdAsync(currentSet.SetId);
                
                if (currentSet.SetNumber > 1)
                {
                    var previousSetWinner = await _setService.GetPreviousSetWinnerAsync(currentSet.SetId);
                    ActiveMatchVM.PreviousSetWinner = previousSetWinner;
                }
                
                ActiveMatchVM.SetId = currentSet.SetId;
                ActiveMatchVM.Team1Score = currentSet.Team1Score;
                ActiveMatchVM.Team2Score = currentSet.Team2Score;
                ActiveMatchVM.SetNumber = currentSet.SetNumber;
                ActiveMatchVM.InfoMessage = currentSetInfo.InfoMessage;
                ActiveMatchVM.IsPlayer1Serve = currentSetInfo.IsPlayer1Serve;
                ActiveMatchVM.IsSetCompleted = currentSet.IsSetCompleted;                

            }

            return Page();
        }


        public async Task<IActionResult> OnPostStartSetAsync(int matchId)
        {
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match != null && match.StartTime == null)
            {
                match.StartTime = DateTime.Now;
                await _matchService.UpdateMatchAsync(match.Adapt<MatchDTO>());
            }

            var currentSet = await _setService.GetCurrentSetAsync(matchId);
            if (currentSet == null)
            {                
                currentSet = await _setService.CreateNewSetAsync(matchId);
            }
            else
            {                
                currentSet.IsSetCompleted = false;
                await _setService.UpdateSetAsync(currentSet);
            }
            
            return RedirectToPage(new { matchId });
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
            await _setService.UpdateSetInfoAsync(currentSetInfo);
            SetServer(currentSetInfo, currentSet);            
            
            if (await _setService.IsSetWonAsync(currentSet))
            {
                currentSet.SetWinner = currentSet.Team1Score > currentSet.Team2Score ? 1 : 2;
                currentSet.IsSetCompleted = true;
                await _setService.UpdateSetAsync(currentSet);

                if (await _matchService.IsMatchWonAsync(matchId))
                {   

                    await _matchService.CompleteMatchAsync(matchId);
                    return RedirectToPage("/Matches/EndMatch", new { matchId });
                }
                
                await _setService.CreateNewSetAsync(matchId);
            }
            else
            {
                await _setService.UpdateSetAsync(currentSet);
            }

            return RedirectToPage(new {matchId});
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
            else
            {
                currentSetInfo.InfoMessage = string.Empty;
            }

            if ((team1SetsWon == setsToWin - 1 && team1score >= 10 && team1score > team2score) ||
                (team2SetsWon == setsToWin - 1 && team2score >= 10 && team2score > team1score))
            {
                currentSetInfo.InfoMessage = "Match Point";
            }
            else if ((team1score >= 10 || team2score >= 10) && Math.Abs(team1score - team2score) >= 1)
            {
                currentSetInfo.InfoMessage = "Set Point";
            }
        }

        private void SetServer(SetInfo currentSetInfo, Set currentSet)
        {
            int totalPoints = currentSet.Team1Score + currentSet.Team2Score;

            // Hantera deuce-situation (9-9 eller högre)( 9 för att reagera även på 10-10)
            if (currentSet.Team1Score >= 9 && currentSet.Team2Score >= 9)
            {
                // Vid deuce byts serve varje poäng
                // Använder totalPoints direkt för att beräkna serve
                currentSetInfo.IsPlayer1Serve = currentSetInfo.IsPlayer1StartServer
                    ? totalPoints % 2 == 0
                    : totalPoints % 2 != 0;
            }
            else
            {
                // Normal servbyte varannan poäng
                int serveTurn = totalPoints / 2;
                currentSetInfo.IsPlayer1Serve = currentSetInfo.IsPlayer1StartServer
                    ? serveTurn % 2 == 0
                    : serveTurn % 2 != 0;
            }
        }
    }
}