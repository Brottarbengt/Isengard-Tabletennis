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
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int SetCount { get; set; } // Calculated from Sets.Count
    }
}
