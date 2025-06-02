using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Interfaces;

namespace Tabletennis.Pages.Matches
{
    public class UpdateModel : PageModel
    {
        private readonly IMatchService _matchService;

        public UpdateModel(IMatchService matchService)
        {
            _matchService = matchService;
        }
        [BindProperty]
        public MatchUpdateDTO MatchDto { get; set; }

        public List<SelectListItem> PlayerList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MatchDto = await _matchService.GetMatchForUpdateAsync(id);
            if (MatchDto == null)
            {
                return NotFound();
            }

            // Call directly on matchService
            PlayerList = await ((MatchService)_matchService).GetPlayerSelectListItemsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                PlayerList = await ((MatchService)_matchService).GetPlayerSelectListItemsAsync();
                return Page();
            }

            await _matchService.UpdateMatchAsync(MatchDto);
            return RedirectToPage("/Matches/MatchHistory");
        }
    }
}

