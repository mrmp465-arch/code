using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.TopupPartner.Unknown
{
    public class UnknownBuyCard : IBuyCardHandler
    {
        public int checkStore(string provider, int amount)
        {
            return 0;
        }

        public APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, string partnerCode, string providerCode, ref string providerResponse)
        {
            return new APIResponse((int)ResponseCode.ProviderNotFound);
        }

    }

    public class UnknownTopup : ITopupHandler
    {

        public APIResponse TranferBalance(string requestId, string provider, int amount, string simTarget, string partnerCode, string providerCode)
        {
            throw new NotImplementedException();
        }
    }
}
