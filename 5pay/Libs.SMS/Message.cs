using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Libs.SMS
{
    public class SmsMt
    {
        public int status { get; set; }
        public string sms { get; set; }
        public string type { get; set; }
    }
    public enum MessageInStatus
    {
        WaitProcess = 0, //Cho xu ly
        Done = 1, //Ok
        Fail = -1, //Forwarded qua Partner
        FailForward = -2,
        FailAddLog = -3

    }

    public enum MessageInType
    {
        ChatSms = 1,
        SendSms = 2
    }

    public class ProcessResult
    {
        public bool IsSuccess { get; set; }

        public bool IsCharge { get; set; }

        public string Message { get; set; }

        public string ResultMT { get; set; }
    }

}
