using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tabletennis.ViewModels
{
    public class UpdateMatchViewModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public List<SelectListItem> PlayerSelectList { get; set; } = new();
        public string SelectedPlayerName { get; set; } = string.Empty;
    }
} 