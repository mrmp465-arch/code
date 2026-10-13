using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IZISoft.Entity
{
    public class TopupRequest
    {
        public int telcoId { get; set; }
        public int denId { get; set; }
        public string code { get; set; }
        public string serial { get; set; }
        public string scratchCallbackUrl { get; set; }
    }
}