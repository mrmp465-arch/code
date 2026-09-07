using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.API.Models
{
    public class BlueSend
    {
        public string accountEmail { get; set; }
        public string content { get; set; }
        public bool is_outbound { get; set; }
        public string status { get; set; }
        public object error_code { get; set; }
        public object error_message { get; set; }
        public string message_handle { get; set; }
        public DateTime date_sent { get; set; }
        public DateTime date_updated { get; set; }
        public string from_number { get; set; }
        public string to_number { get; set; }
        public bool was_downgraded { get; set; }
        public string plan { get; set; }
    }
    public class SMSStatusParam
    {
        public int id { get; set; }
        public int status { get; set; }
        public string  message { get; set; }
        public int port { get; set; }
        public string number { get; set; }
    }
    public class SMSContent
    {
        public string number { get; set; }
        public string content { get; set; }
        public string sign { get; set; }
        public int? type { get; set; }
    }
}