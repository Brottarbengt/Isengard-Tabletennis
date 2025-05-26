using DataAccessLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Interfaces;
using Tabletennis.ViewModels;
using Mapster;
using DataAccessLayer.DTOs;
using System;

namespace Tabletennis.Pages.Player
{
    public class CreateModel : PageModel
    {

        private readonly IPlayerService _playerService;

        public CreateModel(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [BindProperty]
        public PlayerCreateViewModel NewPlayer { get; set; }
        public List<SelectListItem> GenderOptions { get; set; }
        public string MaxDate { get; set; }


        public void OnGet()
        {
            SetMaxDate();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                SetMaxDate();
                return Page();
            }

            var newPlayer = NewPlayer.Adapt<PlayerDTO>();
            var result = await _playerService.CreatePlayer(newPlayer);

            if (result == Check.Failed)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong when creating the player.");
                SetMaxDate();
                return Page();
            }

            TempData["SuccessMessage"] = "Player was successfully created!";
            return Page();
        }

        

        public void SetMaxDate()
        {
            MaxDate = DateTime.Today.ToString("yyyy-MM-dd");
        }

    }
}
