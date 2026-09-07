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
        public int status { get; set; }
        public int error_code { get; set; }
        public string message { get; set; }
        public string signature { get; set; }
        //public DataTopup data { get; set; }
    }
    public class TokenResponse
    {
       
        public string access_token { get; set; }
        
    }
    public class Callback
    {
        public string transaction_id { get; set; }
        public string type_transaction { get; set; }
        public int value { get; set; }
        
        public int status { get; set; }
       
        public string carrier_return_value { get; set; }
        public string message { get; set; }
        public string signature { get; set; }
    }
}