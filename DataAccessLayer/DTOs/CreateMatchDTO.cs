using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class CreateMatchDTO
    {
        public DateTime MatchDate { get; set; }
        public bool IsSingle { get; set; }
        public int BestOfSets { get; set; } // e.g., 3, 5, or 7

        public string Player1FirstName { get; set; }
        public string Player1LastName { get; set; }

        public string Player2FirstName { get; set; }
        public string Player2LastName { get; set; }
    }
}
