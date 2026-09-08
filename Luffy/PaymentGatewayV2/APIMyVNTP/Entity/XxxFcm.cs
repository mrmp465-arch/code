using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMyVNTP.Entity
{
    public class Payload
    {
        public string token { get; set; }
        public string otp { get; set; }
    }

    public class FcmResponse
    {
        public bool isSuccess { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public object errorDetail { get; set; }
        public Payload payload { get; set; }
    }
}