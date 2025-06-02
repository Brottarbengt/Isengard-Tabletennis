using System.Collections.Generic;

namespace Tabletennis.ViewModels
{
    public class EndMatchViewModel
    {
        public int MatchId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public string WinnerName { get; set; }
        public int WinnerId { get; set; }
        public int? DurationSeconds { get; set; }

        // Lista med alla set och deras resultat
        public List<SetResult> Sets { get; set; } = new();

        public class SetResult
        {
            public int SetNumber { get; set; }
            public int Team1Score { get; set; }
            public int Team2Score { get; set; }
            public int SetWinner { get; set; } // 1 eller 2
        }
    }
}