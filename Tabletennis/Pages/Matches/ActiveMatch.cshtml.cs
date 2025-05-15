using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;
using System.Reflection;

namespace Tabletennis.Pages.Matches
{
    public class ActiveMatchModel : PageModel

    {
        private readonly ISetService _setService;

        public ActiveMatchModel(ISetService setService)
        {
            _setService = setService;
        }


        // TODO: Set MatcDate only once, protected?
        public string Player1 { get; set; } = "Player 1";
        public string Player2 { get; set; } = "Player 2";
        public string SetWinner { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int NumberOfSets { get; set; }
        public int SetNumber { get; set; }
        public Set CurrentSet { get; set; }

        //public DataAccessLayer.Models.Match CurrentMatch { get; set; } 

        public void OnGet()
        {
            
            if (CurrentSet == null)
            {
                SetNumber = 1;
                StartNewSet(SetNumber);
            }
        }

        private void StartNewSet(int setNumber)
        {
            // Rensa ModelState f�r att undvika att gamla v�rden �terkommer
            ModelState.Clear();

            // Skapa ett nytt set och l�gg till det i databasen
            CurrentSet = new Set{
                MatchId = 2, // TODO: H�mta matchId fr�n databasen
                SetNumber = setNumber,
                Team1Score = 0,
                Team2Score = 0,
                WinnerId = 0,
                IsDecidingSet = false
            };

            //TODO: L�gga till relevant Service 
            _setService.CreateSet(CurrentSet);            

            // �terst�ll Winner till null f�r n�sta omg�ng
            //Winner = null;
        }

        public string CheckEndOfSet(int player1Score, int player2Score)
        {
            if (player1Score >= 11 || player2Score >= 11)
            {
                if (Math.Abs(player1Score - player2Score) >= 2)
                {
                    // Kontrollera vem som har flest po�ng f�r att avg�ra vinnaren
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