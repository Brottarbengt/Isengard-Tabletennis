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
        public ActiveMatchViewModel MatchVM { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int matchId)
        {
            var match = await _matchService.GetMatchByIdAsync(matchId);
            if (match == null)
            {
                return NotFound();
            }

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