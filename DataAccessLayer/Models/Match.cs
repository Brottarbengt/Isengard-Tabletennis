using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchWinner { get; set; }
        public bool IsSingle { get; set; }
        public bool IsCompleted { get; set; }
        public int MatchType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public virtual ICollection<PlayerMatch> PlayerMatches { get; set; }
        public virtual ICollection<Set> Sets { get; set; }
    }
}
