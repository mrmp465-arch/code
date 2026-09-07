using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.DTO
{
    public class UsersLog
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FunctionCode { get; set; }
        public DateTime LogTime { get; set; }
        public string Description { get; set; }
        public int LogType { get; set; }
        public string FullName { get; set; }
        public string FunctionName { get; set; }
        public string ClientIP { get; set; }
        public string PaygateName { get; set; }
    }
    public class BidHistory    {
        public long Id { get; set; }
    
        public int UserID { get; set; }
      
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
    }
}
