using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class SetDTO
    {
        public int SetId { get; set; }
        public int MatchId { get; set; }
        public int SetNumber { get; set; }
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public int WinnerId { get; set; }
        public bool IsDecidingSet { get; set; }
        public int SetInfoId { get; set; }

    }
}
