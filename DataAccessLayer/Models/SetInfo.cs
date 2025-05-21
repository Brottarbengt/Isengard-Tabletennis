using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class SetInfo
    {
        public int SetInfoId { get; set; }
        public string InfoMessage { get; set; } = string.Empty;
        public bool IsPlayer1Serve { get; set; }
        public bool IsPlayer1StartServer { get; set; }
        public int ServeCounter { get; set; }
        public int SetId { get; set; }
        public virtual Set Set { get; set; }
        
    }
}
