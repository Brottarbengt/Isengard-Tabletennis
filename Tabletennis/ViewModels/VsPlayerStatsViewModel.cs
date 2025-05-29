namespace Tabletennis.ViewModels
{
    public class VsPlayerStatsViewModel
    {
        public PlayerStatisticsViewModel Player1 { get; set; } = new();
        public PlayerStatisticsViewModel Player2 { get; set; } = new();
        public int Player1Wins { get; set; }
        public int Player2Wins { get; set; }
        public decimal Player1WinRatio { get; set; }
        public decimal Player2WinRatio { get; set; }
        public int TotalMatches { get; set; }
        public TimeSpan LongestMatchDuration { get; set; }
        public TimeSpan ShortestMatchDuration { get; set; }
        public DateTime LongestMatchDate { get; set; }
        public DateTime ShortestMatchDate { get; set; }
    }
} 