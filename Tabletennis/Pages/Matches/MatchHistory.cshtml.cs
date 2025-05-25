using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Infrastructure;
using Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tabletennis.Pages.Matches
{
    public class MatchHistoryModel : PageModel
    {
        private readonly IMatchService _matchService;

        public MatchHistoryModel(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 3;

        public PagedResult<MatchListDTO> Matches { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var parameters = new MatchQueryParameters
            {
                Query = Query,
                Date = SelectedDate,
                PageNumber = PageNumber,
                PageSize = PageSize
            };

            Matches = await _matchService.GetFilteredMatchesAsync(parameters);
            return Page();
        }
    }
}
