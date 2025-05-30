using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class SetUpdateDTO
    {
        public int? SetId { get; set; } // Nullable for new sets
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
    }
}
