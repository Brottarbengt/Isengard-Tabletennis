using DataAccessLayer.DTOs;
using DataAccessLayer.Enums;
using DataAccessLayer.Migrations;
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
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public List<SelectListItem> GenderOptions { get; set; }
        [BindProperty]
        public string MaxDate { get; set; }
        [BindProperty]
        public PlayerCreateViewModel player {  get; set; }

        public async Task OnGet(int playerId)
        {

            ////l�gga en samling ?
            //TypeAdapterConfig<PlayerCreateViewModel, PlayerDTO>.NewConfig()
            //    .Map(dest => dest.Birthday, src => DateOnly.FromDateTime(src.Birthday));

            //TypeAdapterConfig<PlayerDTO, PlayerCreateViewModel>.NewConfig()
            //    .Map(dest => dest.Birthday, src => src.Birthday.HasValue
            //        ? src.Birthday.Value.ToDateTime(TimeOnly.MinValue)
            //        : default);

            SetMaxDate();
            LoadGenderOptions();

            var playerDTO = await _playerService.GetOneAsync(playerId);
            player = playerDTO.Adapt<PlayerCreateViewModel>();
            FullName = player.FirstName + player.LastName;
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
            


            //kanske en tempdata, successully updaterad!
            //kom ih�g modelstate
            //och check.enums
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

        public void SetMaxDate()
        {
            MaxDate = DateTime.Today.ToString("yyyy-MM-dd");
        }



    }
}
