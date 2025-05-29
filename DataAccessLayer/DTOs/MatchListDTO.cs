using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTOs
{
    public class MatchListDTO
    {
        public int MatchId { get; set; }
        public List<Set> Sets { get; set; } = new(); // or int SetsCount
        public string? Player1FullName { get; set; }
        public string? Player2FullName { get; set; }
        public string? Winner { get; set; }
        public DateTime StartDate { get; set; }
        public int SetCount => Sets?.Count ?? 0;

    }
}
