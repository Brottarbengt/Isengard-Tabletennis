using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public OpponentStatsViewModel? BestOpponent { get; set; }
        public OpponentStatsViewModel? WorstOpponent { get; set; }
        public TimeSpan LongestMatchDuration { get; set; }
        public TimeSpan ShortestMatchDuration { get; set; }
        public string LongestMatchOpponent { get; set; }
        public string ShortestMatchOpponent { get; set; }
    }

    public class OpponentStatsViewModel
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal WinRatio { get; set; }
        public int TotalMatches { get; set; }
    }
} 