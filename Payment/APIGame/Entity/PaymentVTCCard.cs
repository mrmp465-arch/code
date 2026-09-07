using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIGame.Entity
{
    public class PaymentVTCCard
    {
    }

    public class TopupResponse
    {
        public int ResponseStatus { get; set; }
        public string ErorrMess { get; set; }
        public int ResponseCode { get; set; }
        public string GiftCode { get; set; }
    }

    public class LoginResponse
    {
        public int ResponseStatus { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}