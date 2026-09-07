using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.CardTelco.XBomCardLib
{
    public class CardRequest
    {
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
    }

    public class CardResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    public class CardDataRequest
    {
        public string partnerCode { get; set; }
        public string serviceCode { get; set; }
        public string commandCode { get; set; }
        public string requestContent { get; set; }
        public string signature { get; set; }
    }



}

