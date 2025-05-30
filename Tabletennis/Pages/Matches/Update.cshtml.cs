using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public MatchUpdateDTO MatchDto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var dto = await _matchService.GetMatchForUpdateAsync(id);
            if (dto == null) return NotFound();

            MatchDto = dto;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var success = await _matchService.UpdateMatchAsync(MatchDto);
            if (!success) return NotFound();

            return RedirectToPage("/Matches/MatchHistory");
        }
    }
}

