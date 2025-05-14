using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Tabletennis.Pages.Matches
{
    public class ActiveMatchModel : PageModel
    {
        // TODO: Set MatcDate only once, protected?
        public DateTime MatchDate { get; set; }
        public int NumberOfSets { get; set; }
        public int SetNumber { get; set; }
        public Set CurrentSet { get; set; }

        public void OnGet()
        {
            if (CurrentSet == null)
            {
                StartNewSet();
            }
        }

        private void StartNewSet()
        {
            // Rensa ModelState f�r att undvika att gamla v�rden �terkommer
            ModelState.Clear();

            // Skapa ett nytt set och l�gg till det i databasen
            CurrentSet = new Set();
            //TODO: L�gga till relevant Service 
            //_matchService.Sets.Add(CurrentSet);
            //_matchService.SaveChanges();

            // �terst�ll Winner till null f�r n�sta omg�ng
            //Winner = null;
        }
    }
}