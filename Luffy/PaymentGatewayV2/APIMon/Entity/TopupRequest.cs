using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMon.Entity
{
    public class TopupRequest
    {

        public string api_key { get; set; }
        public string card_seri { get; set; }
        public string card_code { get; set; }
        public string request_id { get; set; }
        public int card_amount { get; set; }
        public string card_type { get; set; }
        public string signature { get; set; }
    }
}