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
        public int MatchType { get; set; } 
        public int MatchWinner { get; set; } // 1 or 2
    }
}
