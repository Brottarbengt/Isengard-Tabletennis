using DataAccessLayer.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;
using System.Reflection;
using System.Threading.Tasks;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Matches
{
    public class ActiveMatchModel : PageModel

    {
        private readonly ISetService _setService;
        private readonly IMatchService _matchService;

        public ActiveMatchModel(ISetService setService, IMatchService matchService)
        {
            _setService = setService;
            _matchService = matchService;
        }
        
        
        public int SetNumber { get; set; }
        public Set CurrentSet { get; set; }

        public ActiveMatchViewModel ActiveMatchVM { get; set; }

        public async Task OnGetAsync(int matchId, string player1, string player2)
        {
            var activeMatch = await _matchService.GetMatchByIdAsync(matchId);
            ActiveMatchVM = activeMatch.Adapt<ActiveMatchViewModel>();

            ActiveMatchVM.Player1 = player1;
            ActiveMatchVM.Player2 = player2;
            if (CurrentSet == null)
            {
                SetNumber = 1;
                StartNewSet(SetNumber, matchId);
            }
        }

        private void StartNewSet(int setNumber, int matchId)
        {
            // Rensa ModelState för att undvika att gamla värden återkommer
            ModelState.Clear();

            // Skapa ett nytt set och lggga till det i databasen
            CurrentSet = new Set{
                MatchId = matchId, // TODO: Hmta matchId frn databasen
                SetNumber = setNumber,
                Team1Score = 0,
                Team2Score = 0,
                WinnerId = 0,
                IsDecidingSet = false
            };

            //TODO: Lggga till relevant Service 
            _setService.CreateSet(CurrentSet);            

            // rterstll Winner till null fr nsta omgng
            //Winner = null;
        }

        public string CheckEndOfSet(int player1Score, int player2Score)
        {
            if (player1Score >= 11 || player2Score >= 11)
            {
                if (Math.Abs(player1Score - player2Score) >= 2)
                {
                    // Kontrollera vem som har flest poäng fr att avgöra vinnaren
                    if (player1Score > player2Score)
                    {
                        return "Player1";
                    }
                    else
                    {
                        return "Player2";
                    }
                }
            }
            return null;
        }
    }
}