using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIVinaPay.Entity
{

    public class DataTopup
    {
        public int id { get; set; }
        public string cardSerial { get; set; }
        public int cardPrice { get; set; }
        public string cardType { get; set; }
        public string createDate { get; set; }
        public string status { get; set; }
        public int menhGiaThat { get; set; }
        public int menhGiaNhapVao { get; set; }
        public string systemtranid { get; set; }
        public string tranid { get; set; }
        public string statuscode { get; set; }
    }

    public class TopupResponse
    {
        //public int status { get; set; }
        public int errorCode { get; set; }
        public string msg { get; set; }
        // public string signature { get; set; }
        //public DataTopup data { get; set; }
    }
    public class TokenResponse
    {

        public string access_token { get; set; }

    }
    public class Callback
    {
        public string TransID { get; set; }

        public int Amount { get; set; }
        public int ReadAmount { get; set; }

        public int Status { get; set; }

        public string Signature { get; set; }
        public string CardCode { get; set; }
        public string CardSeri { get; set; }

    }
}