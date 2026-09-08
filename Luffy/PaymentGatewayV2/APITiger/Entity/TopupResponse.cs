using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tiger.Entity
{
    public class TopupRequest
    {
        public string client_type_card { get; set; }
        public int client_value_card { get; set; }
        public string client_seri_card { get; set; }
        public string client_code_card { get; set; }
        
        public string client_transaction_id { get; set; }
        public string client_signature { get; set; }
        public string client_token_api { get; set; }
        public string client_code_request { get; set; }
    }
    public class Callback
    {
        public string card_transaction_id { get; set; }
        public string card_code { get; set; }
        public string card_series { get; set; }
        public int card_real_amount { get; set; }
        public int card_net_amount { get; set; }
        public int card_received_amount { get; set; }
        public int card_status { get; set; }
        public string card_content { get; set; }
        public string card_type { get; set; }
        public object message_code { get; set; }
        public object reason { get; set; }
    }

    public class TopupResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string reject_message { get; set; }
        
    }


}