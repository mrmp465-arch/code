using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{
    public class TopupRequest
    {
        public string phone { get; set; }
        public string card_id { get; set; }
    }
}