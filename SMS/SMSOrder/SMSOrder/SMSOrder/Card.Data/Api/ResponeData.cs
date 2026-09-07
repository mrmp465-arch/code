using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    [Serializable]
    public class ResponeData
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public PortInfo ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    [Serializable]
    public class ResponePortData
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public List<PortInfo> ResponseContent { get; set; }
        public string Signature { get; set; }
    }
    [Serializable]
    public class ResponeSendSMSData
    {
        public ResponseSMSContent ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public object Description { get; set; }
        public object Signature { get; set; }
    }
    [Serializable]
    public class ResponeSendSMSApiData
    {
        public int code { get; set; }
        public string des { get; set; }
   
    }


    [Serializable]
    public class ResponeSendSMSApiDataV2
    {
        public long messageId { get; set; }
       

    }
    public class ResponseSMSContent
    {
        public int Code { get; set; }
        public int SmsInQueue { get; set; }
        public int TaskId { get; set; }
    }
    public class PortUse
    {
        public int port { get; set; }
        public DateTime lastime { get; set; }

    }
    public class NumberUse
    {
        public string number { get; set; }
        public DateTime lastime { get; set; }



    }
    public class NumberLock
    {
        public string number { get; set; }
        public DateTime lastime { get; set; }

        public int port { get; set; }
        public int? type { get; set; }
    }

    public class USSDContent
    {
        public int port { get; set; }
        public int status { get; set; }
        public string text { get; set; }
    }

    public class USSDResponseContent
    {
        public List<USSDContent> ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public object Description { get; set; }
        public object Signature { get; set; }
    }
    public class SMSInboxResponseContent
    {
        public List<SMSInbox> ResponseContent { get; set; }
        public int ResponseCode { get; set; }
        public object Description { get; set; }
        public object Signature { get; set; }
    }
    public class SMSInbox
    {
        public int port { get; set; }
        public string number { get; set; }
        public string text { get; set; }
        public string timestamp { get; set; }
    }
}
