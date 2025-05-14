using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public CreateMatchViewModel MatchVM { get; set; } = new();

        public async Task OnGetAsync()
        {
            MatchVM.SetOptions = GetSetOptions();
            await LoadPlayersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Always repopulate dropdowns after post
            MatchVM.SetOptions = GetSetOptions();
            await LoadPlayersAsync();

            if (!ModelState.IsValid)
            {
                return Page(); // Now MatchVM is not reset — inputs are preserved
            }

            var match = new CreateMatchDTO
            {
                Player1Id = MatchVM.Player1Id,
                Player2Id = MatchVM.Player2Id,
                SetCount = MatchVM.SelectedSetCount,
                MatchDate = DateTime.Now
            };

            await _matchService.CreateMatchAsync(match);
            TempData["SuccessMessage"] = "New Match was successfully created!";
            return RedirectToPage("/Player/Index");
        }

        public async Task<JsonResult> OnGetGetPlayer(int id)
        {
            var player = await _matchService.GetPlayerByIdAsync(id);
            return new JsonResult(player);
        }

        private async Task LoadPlayersAsync()
        {
            var players = await _matchService.GetAllPlayersAsync();
            MatchVM.PlayerList = players
                .Select(p => new SelectListItem(p.FullName, p.PlayerId.ToString()))
                .ToList();
        }

        private List<SelectListItem> GetSetOptions()
        {
            return new List<SelectListItem>
            {
              new SelectListItem { Text = "3", Value = "3" },
              new SelectListItem { Text = "5", Value = "5" },
              new SelectListItem { Text = "7", Value = "7" }
            };
        }
    }

}
