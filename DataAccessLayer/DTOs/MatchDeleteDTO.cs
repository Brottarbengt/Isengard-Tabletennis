using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class MatchDeleteDTO
    {
        public int MatchId { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public string Player2Name { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public string Winner { get; set; } = string.Empty;
    }

}
