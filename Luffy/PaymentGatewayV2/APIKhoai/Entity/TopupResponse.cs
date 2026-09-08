using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Khoai.Entity
{

    public class Callback
    {
        public string transaction_id { get; set; }
        public string status { get; set; }
        public string value { get; set; }
        public string real_value { get; set; }
        public string received_value { get; set; }
        public string card_seri { get; set; }
        public string card_code { get; set; }
        public string refcode { get; set; }
        public string sign { get; set; }
       // public string status_code { get; set; }
    }
    public class ZCallback
    {
        public string transaction_id { get; set; }
        public string status { get; set; }
        public string value { get; set; }
        public string real_value { get; set; }
        public string received_value { get; set; }
        public string card_seri { get; set; }
        public string card_code { get; set; }
        public string refcode { get; set; }
        public string sign { get; set; }
        public string status_code { get; set; }
    }

    public class TopupResponse
    {
        public int status { get; set; }
        public string status_code { get; set; }
        public string msg { get; set; }
        public long transaction_id { get; set; }
    }


}