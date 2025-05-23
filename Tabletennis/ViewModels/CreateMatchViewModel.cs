using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Tabletennis.ViewModels
{
    public class CreateMatchViewModel
    {
        public int Player1Id { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public string Player1FirstName { get; set; } = string.Empty;

        public int Player2Id { get; set; }
        public string Player2Name { get; set; } = string.Empty;
        public string Player2FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select number of sets")]
        public int SelectedSetCount { get; set; }
        public List<SelectListItem> SetOptions { get; set; } = new();
        public List<SelectListItem> PlayerList { get; set; } = new();

        // Add full player list for extra info like birth year
        public List<PlayerDTO> AllPlayers { get; set; } = new();
    }
}
