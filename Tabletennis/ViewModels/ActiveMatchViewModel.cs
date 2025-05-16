namespace Tabletennis.ViewModels
{
    public class ActiveMatchViewModel
    {
        public int SetId { get; set; }
        public int MatchId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public string Player1 { get; set; } = string.Empty;
        public string Player2 { get; set; } = string.Empty;
        public string SetWinner { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int MatchType { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int SetNumber { get; set; }

    }
}
