using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyMobi.Entity
{

    public class PaymentHistory
    {
        public List<Datum> data { get; set; }
        public object errors { get; set; }
    }

    public class Datum
    {
        public string phone { get; set; }
        public int type { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public int amount { get; set; }
    }

    
}