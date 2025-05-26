using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Interfaces;
using Tabletennis.ViewModels;
using static System.Collections.Specialized.BitVector32;

namespace Tabletennis.Pages.Player
{
    public class IndexModel : PageModel
    {
        private readonly IPlayerService _playerService;

        public IndexModel(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public List<PlayerSmallInfoViewModel> Players { get; set; } = new();

        public async Task OnGet()
        {
            var playersDTO = await _playerService.GetAllSmallAsync();
            Players = playersDTO.Adapt<List<PlayerSmallInfoViewModel>>();
        }
    }
}

