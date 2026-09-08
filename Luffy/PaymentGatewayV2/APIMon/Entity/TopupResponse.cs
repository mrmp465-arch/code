using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMon.Entity
{

    public class TopupResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public long tran_id { get; set; }

    }

}