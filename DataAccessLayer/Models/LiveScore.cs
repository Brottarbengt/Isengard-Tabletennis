using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class LiveScore
    {
        public int MatchId { get; set; }
        public int SetId { get; set; }
        public int CurrentSetNumber { get; set; }
        public int Team1Points { get; set; }
        public int Team2Points { get; set; }
    }
}
