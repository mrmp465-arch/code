using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Data.Api
{
    internal class SMSParam
    {
    }
    public class SMSResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }
    public class Callback
    {
        public string TransId { get; set; }
        //public string MomoId { get; set; }
        //public string MomoName { get; set; }
        //public string MomoTransId { get; set; }
        //public int Amount { get; set; }
        ////public DateTime TimeMomoSuccess { get; set; }
        //public string Note { get; set; }
        //public string BankTransId { get; set; }
        //public string Comment { get; set; }

        //public string PartnerBankCode { get; set; }
        //public string PartnerBankId { get; set; }
        //public string PartnerBankName { get; set; }

        //public string MomoPartnerId { get; set; } // NG Chuyển
        //public string MomoPartnerName { get; set; }

    }
    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }
    }
    public class SMSRequest
    {

        public string TransId { get; set; }
        public string MomoId { get; set; }
        public string Content { get; set; }
        public string CallbackUrl { get; set; }
    }
    public class SMSRespone
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }

    }
}
