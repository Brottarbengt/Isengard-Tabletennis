using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISetService
    {
        void CreateSet(Set currentSet);
        
        void SaveSet(int setId, LiveScore score);
        //SetDTO? GetCurrentSetForMatch(int matchId);
    }
}
