using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IZISoft.Entity
{

  public class DataTopup
    {
        public string id { get; set; }
        public int telcoId { get; set; }
        public int denId { get; set; }
        public string code { get; set; }
        public string serial { get; set; }
        public string scratchCallbackUrl { get; set; }
        public int status { get; set; }
    }

    public class TopupResponse
{
        public string code { get; set; }
        public string message { get; set; }
        public DataTopup data { get; set; }
    }


}