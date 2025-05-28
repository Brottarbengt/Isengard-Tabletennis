namespace Tabletennis.ViewModels
{
    public class Top10PlayersViewModel
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int NumberOfWins { get; set; }        
        public decimal PlayerWinRatio { get; set; }
        public int MatchesPlayed { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        
        
    }
}
