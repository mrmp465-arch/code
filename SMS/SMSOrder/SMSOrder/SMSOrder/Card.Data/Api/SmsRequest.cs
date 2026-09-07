using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Data.Api
{
    public class SmsParam
    {
        public string number { get; set; }
        public List<string> text_param { get; set; }
        public long user_id { get; set; }

    }

    public class SmsRequest
    {
        public string text { get; set; }
        public List<SmsParam> param { get; set; }
        public List<int> port { get; set; }
        public string sn { get; set; }
        public SmsRequest(List<int> ports, List<SmsParam> smsParams,string sns)
        {
            param = smsParams;
            port = ports;
            text = "#param#";
            sn = sns;
        }
    }
    public class UssdRequest
    {
        public List<int> port { get; set; }
        public string command { get; set; }
        public string text { get; set; }
        public string sn { get; set; }
        public UssdRequest(List<int> ports, string texts, string sns)
        {
            command = "";
            port = ports;
            text = texts;
            sn = sns;
        }
    }
}