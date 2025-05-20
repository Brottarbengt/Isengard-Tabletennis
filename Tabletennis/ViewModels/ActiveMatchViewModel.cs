namespace Tabletennis.ViewModels
{
    public class ActiveMatchViewModel
    {
        public int SetId { get; set; }
        public int MatchId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public string Player1FirstName { get; set; } = string.Empty;
        public string Player2Name { get; set; } = string.Empty;
        public string Player2FirstName { get; set; } = string.Empty;
        public int SetWinner { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchType { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int SetNumber { get; set; }
        public bool IsCompleted { get; set; }
        public int MatchWinner { get; set; }
        public int Team1WonSets { get; set; }
        public int Team2WonSets { get; set; }
        public string InfoMessage { get; set; } = string.Empty;
    }
}
