using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Huy.Entity
{

    public class Callback
    {
        public string timestamp { get; set; }
        public string status { get; set; }
        public string value { get; set; }
        public string real_value { get; set; }
     
        public string message { get; set; }
      
        public string refcode { get; set; }
        public string tran { get; set; }
        public string sign { get; set; }
    }

    public class TopupResponse
    {
        public int rc { get; set; }
        public string rd { get; set; }
        public string tran_id { get; set; }
    }


}