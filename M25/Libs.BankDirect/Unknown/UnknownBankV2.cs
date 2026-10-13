using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.BankGate;

namespace Libs.BankDirect.Unknown
{
    public class UnknownBankV2 : IBankDirectV2Handler
    {
        public APIResponse CheckTrans(APITransaction transaction)
        {
            throw new NotImplementedException();
        }
        public APIResponse GetBanksV2(string ParnerId, string AccoutName)
        {
            throw new NotImplementedException();
        }
        public APIResponse GetBanksV3(string ParnerId, string AccoutName)
        {
            throw new NotImplementedException();
        }
        public APIResponse GetBanks(string partnercode)
        {
            throw new NotImplementedException();
        }

        public APIResponse Order(APITransaction transaction)
        {
            throw new NotImplementedException();
        }
        public APIResponse OrderV2(APITransaction transaction)
        {
            return new APIResponse((int)ResponseCode.TransactionFailed);
        }
    }
}
