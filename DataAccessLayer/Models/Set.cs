using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Set
    {
        public int SetId { get; set; }
        public int MatchId { get; set; }
        public int SetNumber { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int SetWinner { get; set; }
        public bool IsDecidingSet { get; set; } // Add this


        public virtual Match Match { get; set; }

    }
}
