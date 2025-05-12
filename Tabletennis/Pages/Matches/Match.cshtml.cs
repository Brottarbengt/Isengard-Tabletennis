using DataAccessLayer.DTOs;
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
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var dto = new CreateMatchDTO
            {
                MatchDate = MatchVM.MatchDate,
                IsSingle = MatchVM.IsSingle,
                Player1FirstName = MatchVM.Player1FirstName,
                Player1LastName = MatchVM.Player1LastName,
                Player2FirstName = MatchVM.Player2FirstName,
                Player2LastName = MatchVM.Player2LastName,
                BestOfSets = MatchVM.BestOfSets

            };
            int matchId = await _matchService.CreateMatchAsync(dto);
            return RedirectToPage("/Index", new { id = matchId });

        }
    }
}
