namespace Tabletennis.ViewModels
{
    public class PlayerStatisticsViewModel
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int NumberOfWins { get; set; }
        public int NumberOfLosses { get; set; }
        public decimal PlayerWinRatio { get; set; }
        public int MatchesPlayed { get; set; }
        public string FullName { get; set; } = string.Empty;


    }
}
