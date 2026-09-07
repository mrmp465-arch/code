using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    [Serializable]
    public class Sims
    {
        public long Id { get; set; }
       
        public string Number { get; set; }
        public DateTime ExpriteDate { get; set; }
   
        public int RealBalance { get; set; }
        public int Balance { get; set; }
        public int Group { get; set; }
        public string Password { get; set; }
        public int Status { get; set; }
    
        public string Telco { get; set; }

        public string Note { get; set; }
    }
    public class SimsForm
    {
        public int Id { get; set; }
      
        public string Number { get; set; }
        public string ExpriteDate { get; set; }
        public int RealBalance { get; set; }
        public int Balance { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        
        public string GroupName { get; set; }
        public string Telco { get; set; }

        public string Note { get; set; }

    }
    
}
