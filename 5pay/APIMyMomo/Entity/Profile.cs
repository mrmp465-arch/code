using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMomo.Entity
{

    public class DataProfile
    {
        public string phone { get; set; }
        public string title { get; set; }
        public string balance { get; set; }
        public int type { get; set; }
        public string totalDebt { get; set; }
        public string period { get; set; }
        public string expireDate { get; set; }
        public int networkAccount { get; set; }
        public int othersAccount { get; set; }
        public string paymentDate { get; set; }
        public string billCycleId { get; set; }
        public string debtStartCyCle { get; set; }
        public string payment { get; set; }
        public string fullname { get; set; }
        public int responseCode { get; set; }
        public DateTime updateTime { get; set; }
    }

    
    public class Profile

    {
        public List<DataProfile> data { get; set; }
        public object errors { get; set; }
    }

}