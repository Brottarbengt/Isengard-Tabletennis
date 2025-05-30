using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class MatchUpdateDTO
    {
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchWinner { get; set; }
        public bool IsSingle { get; set; }
        public bool IsCompleted { get; set; }
        public int MatchType { get; set; }

        // Related to PlayerMatches
        public List<PlayerUpdatedDTO> Players { get; set; } = new();
    }
    public class PlayerUpdatedDTO
    {
        public int PlayerId { get; set; }
        public int TeamNumber { get; set; }
    }
}
