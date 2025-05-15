using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class ActiveMatchDTO
    {
        public int MatchId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int MatchType { get; set; }
        public DateTime MatchDate { get; set; }
        List<SetDTO> Sets { get; set; } = new List<SetDTO>();
    }
}
