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


        public void OnGet()
        {
            LoadGenderOptions();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                LoadGenderOptions();
                return Page();
            }

            TypeAdapterConfig<PlayerCreateViewModel, PlayerCreateDTO>.NewConfig()
            .Map(dest => dest.Birthday, src => DateOnly.FromDateTime(src.Birthday));

            var newPlayer = NewPlayer.Adapt<PlayerCreateDTO>();
            var result = await _playerService.CreatePlayer(newPlayer);

            if (result == Check.Failed)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong when creating the player.");
                LoadGenderOptions();
                return Page();
            }

            TempData["SuccessMessage"] = "Player was successfully created!";
            return Page();
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
