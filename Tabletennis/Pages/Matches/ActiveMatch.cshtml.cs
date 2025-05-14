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
            // Rensa ModelState för att undvika att gamla värden återkommer
            ModelState.Clear();

            // Skapa ett nytt set och lägg till det i databasen
            CurrentSet = new Set();
            //TODO: Lägga till relevant Service 
            //_matchService.Sets.Add(CurrentSet);
            //_matchService.SaveChanges();

            // Återställ Winner till null för nästa omgång
            //Winner = null;
        }
    }
}