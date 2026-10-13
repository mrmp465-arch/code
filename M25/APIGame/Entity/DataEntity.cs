using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Libs.Report;

namespace APIGame.Entity
{
    public class DataCallback
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }

    }

    public class DataCallbackOrder
    {
        public long OrderId { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public string Signature { get; set; }
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public decimal? BidRate { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime? CreatTime { get; set; }
        public string Description { get; set; }

    }

    public class DataResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string content { get; set; }
    }
}

