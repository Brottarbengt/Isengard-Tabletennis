using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class MatchDTO
    {
        public int MatchId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public string Player1FirstName { get; set; } = string.Empty;
        public string Player2Name { get; set; } = string.Empty;
        public string Player2FirstName { get; set; } = string.Empty;
        public int MatchType { get; set; }
        public DateTime MatchDate { get; set; }
        public int Team1WonSets { get; set; }
        public int Team2WonSets { get; set; }
    }
}
