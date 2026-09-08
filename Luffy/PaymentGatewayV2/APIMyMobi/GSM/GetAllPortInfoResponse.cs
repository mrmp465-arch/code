using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyMobi.GSM
{
    public class Position
    {
        public int Port { get; set; }
        public int Slot { get; set; }
    }

    public class ResponseContent
    {
        public Position Position { get; set; }
        public string Number { get; set; }
        public int Balance { get; set; }
        public int State { get; set; }
        public int Count { get; set; }
    }

    public class GetAllPortInfoResponse
    {
        public List<ResponseContent> ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }
    }
}