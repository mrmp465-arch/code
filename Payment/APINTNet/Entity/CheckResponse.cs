using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APINTNet.Entity
{
    public class CheckResponse
    {
        public string tranid { get; set; }
        public string final_status { get; set; }
        public string response_amount { get; set; }
        public string provider_message { get; set; }
    }
}