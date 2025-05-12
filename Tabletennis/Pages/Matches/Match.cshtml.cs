using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Match
{
    public class MatchModel : PageModel
    {
        private readonly IMatchService _matchService;
        public MatchModel(IMatchService matchService)
        {
            _matchService = matchService;
        }
        [BindProperty]
        public CreateMatchViewModel MatchVM { get; set; }
        public void OnGet()
        {
            MatchVM = new CreateMatchViewModel();
        }
        
    }
}
