using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tabletennis.Pages.Matches
{
    public class ActiveMatchModel : PageModel
    {
        // TODO: Set MatcDate only once, protected?
        public DateTime MatchDate { get; set; }
        public int NumberOfSets { get; set; }
        public int SetNumber { get; set; }
        public void OnGet()
        {
            MatchDate = DateTime.Now;
        }
    }
}