using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tabletennis.ViewModels
{
    public class StatisticsViewModel
    {
        public List<PlayerDTO> AllPlayers { get; set; } = new();
        public List<SelectListItem> PlayerSelectList { get; set; } = new();
        public int? SelectedPlayer1Id { get; set; }
        public int? SelectedPlayer2Id { get; set; }
        public string SearchQuery { get; set; } = string.Empty;
        public List<Top10PlayersViewModel> Top10Players { get; set; } = new();
        public PlayerStatisticsViewModel? Player1Stats { get; set; }
        public PlayerStatisticsViewModel? Player2Stats { get; set; }
    }
} 