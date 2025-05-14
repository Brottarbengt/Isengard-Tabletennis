using DataAccessLayer.Data;
using DataAccessLayer.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    

    public class SetService : ISetService
    {
        public readonly ApplicationDbContext _dbContext;
        public SetService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
    }
}
