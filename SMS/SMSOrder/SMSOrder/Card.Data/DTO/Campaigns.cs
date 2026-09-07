using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.DTO
{
    public class Campaigns
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }      
        public DateTime StartTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public string Group { get; set; }
        public int Total { get; set; }
        public int TotalMoney { get; set; }
        public int TotalSucces { get; set; }
        public int TotalFail { get; set; }
        public int TotalSMS { get; set; }
        public int Confirm { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
    public class CampaignsForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public string StartTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedUser { get; set; }
        public string Group { get; set; }
        public int Total { get; set; }
        public int TotalSucces { get; set; }
        public int TotalFail { get; set; }
        public int TotalSMS { get; set; }
        public int Confirm { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
}
