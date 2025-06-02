using System.Collections.Generic;

namespace DataAccessLayer.DTOs
{
    public class EndMatchDTO
    {
        public int MatchId { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public string WinnerName { get; set; }
        public int WinnerId { get; set; }
        public List<SetResultDTO> Sets { get; set; } = new();
        public int? DurationSeconds { get; set; }

        public class SetResultDTO
        {
            public int SetNumber { get; set; }
            public int Team1Score { get; set; }
            public int Team2Score { get; set; }
            public int SetWinner { get; set; }
        }
    }
} 