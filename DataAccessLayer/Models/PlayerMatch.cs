using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class PlayerMatch
    {
        public int PlayerMatchId { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public int TeamNumber { get; set; }

        public virtual Player Player { get; set; }
        public virtual Match Match { get; set; }
    }
}
