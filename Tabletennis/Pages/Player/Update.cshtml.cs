using DataAccessLayer.DTOs;
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
        
        public string FullName { get; set; }
        [BindProperty]
        public List<SelectListItem> GenderOptions { get; set; }
        
        public string MaxDate { get; set; }
        [BindProperty]
        public PlayerUpdateViewModel player {  get; set; }

        public async Task OnGet(int playerId)
        {

            SetMaxDate();
            LoadGenderOptions();

            var playerDTO = await _playerService.GetPlayerByIdAsync(playerId);
            player = playerDTO.Adapt<PlayerUpdateViewModel>();
            FullName = player.FirstName + " " + player.LastName;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                SetMaxDate();
                LoadGenderOptions();
                return Page();
            }

            var updatePlayer = player.Adapt<PlayerDTO>();
            var result = await _playerService.Update(updatePlayer);

            if (result == Check.Failed)
            {
                SetMaxDate();
                LoadGenderOptions();
                return Page();
            }

            TempData["SuccessMessage"] = "Spelaren uppdaterad!";
            return RedirectToPage("/player/Index");
        }

        public async Task<IActionResult> OnPostDelete(int playerId)
        {
            var result = await _playerService.SoftDelete(playerId);

            if (result == Check.Failed)
            {
                TempData["ErrorMessage"] = "Kunde inte ta bort spelaren.";
                SetMaxDate();
                LoadGenderOptions();
                return Page();
            }

            TempData["SuccessMessage"] = "Spelaren togs bort!";
            return RedirectToPage("/Player/Index");
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

        public void SetMaxDate()
        {
            MaxDate = DateTime.Today.ToString("yyyy-MM-dd");
        }



    }
}
