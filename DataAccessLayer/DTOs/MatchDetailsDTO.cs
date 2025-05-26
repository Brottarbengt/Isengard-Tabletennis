using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class MatchDetailsDTO
    {
        public int MatchId { get; set; }

        // A list of players involved in the match
        public List<PlayerDTO> Players { get; set; } = new();

        // A list of sets played in the match
        public List<SetDTO> Sets { get; set; } = new();

        // When the match took place
        public DateTime MatchDate { get; set; }

        // Who won the match
        public string Winner { get; set; } = string.Empty;
    }
}
