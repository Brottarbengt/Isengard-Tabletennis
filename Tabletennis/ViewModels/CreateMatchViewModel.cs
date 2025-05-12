using System.ComponentModel.DataAnnotations;

namespace Tabletennis.ViewModels
{
    public class CreateMatchViewModel
    {
        [Required]
        [Display(Name = "Match Date")]
        public DateTime MatchDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Is Singles Match?")]
        public bool IsSingle { get; set; }

        [Required]
        [Range(3, 7)]
        [Display(Name = "Best of (3, 5 or 7 sets)")]
        public int BestOfSets { get; set; }

        [Required]
        [Display(Name = "Player 1  First Name")]
        public string Player1FirstName { get; set; }

        [Required]
        [Display(Name = "Player 1 Last Name")]
        public string Player1LastName { get; set; }

        [Required]
        [Display(Name = "Player 2  First Name")]
        public string Player2FirstName { get; set; }

        [Required]
        [Display(Name = "Player 2 Last Name")]
        public string Player2LastName { get; set; }
    }
}
