using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyViettel.Entity
{
    public class Data
    {
        public string dateUse { get; set; }
        public string amount { get; set; }
        public string datExp { get; set; }
        public string isdn { get; set; }
    }

    public class CheckCardResponse
    {
        public int errorCode { get; set; }
        public string status { get; set; }
        public Data data { get; set; }
        public string message { get; set; }
    }

    public class ResponseContent
    {
        public object stt { get; set; }
        public string trangthai { get; set; }
        public string sothuebao { get; set; }
        public int menhgia { get; set; }
        public string ngaynap { get; set; }
    }

    public class CheckCardApiResponse
    {
        public ResponseContent ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }
    }
}