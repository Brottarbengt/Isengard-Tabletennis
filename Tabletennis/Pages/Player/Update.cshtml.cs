using DataAccessLayer.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using Tabletennis.ViewModels;

namespace Tabletennis.Pages.Player
{
    public class UpdateModel : PageModel
    {
        private readonly IPlayerService _playerService;

        public UpdateModel(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public List<SelectListItem> GenderOptions { get; set; }
        public PlayerCreateViewModel player {  get; set; }

        public async Task OnGet(int playerId)
        {
            LoadGenderOptions();
            var playerDTO = await _playerService.GetOneAsync(playerId);
            player = playerDTO.Adapt<PlayerCreateViewModel>();
        }

        public async Task<IActionResult> OnPost()
        {

            return RedirectToPage("/player/Index");
        }





        public void LoadGenderOptions()
        {
            GenderOptions = Enum.GetValues(typeof(Gender))
                .Cast<Gender>()
                .Select(g => new SelectListItem
                {
                    Value = g.ToString(),
                    Text = g.ToString()
                })
                .ToList();
        }



    }
}
