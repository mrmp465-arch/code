using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    public class SMSLogs
    {
        public long Id { get; set; }
        public int CampaignId { get; set; }
        public int Telco { get; set; }
        public int CountSMS { get; set; }
        public string Number { get; set; }
        public string Contents { get; set; }
        public int Status { get; set; }
        public int Confirm { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public int ResponseStatus { get; set; }
        public int Port { get; set; }
        public string Sender { get; set; }
        public string ResponseMessage { get; set; }

        public string CreatedUser { get; set; }
    }
    public class SMSReport
    {
        public long Total { get; set; }
        public int Telco { get; set; }
        public string Data { get; set; }

        public string CreatedUser { get; set; }

      
    }
}
