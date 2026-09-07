using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;


namespace Libs.BankCash.Unknown
{
    public class UnknownBankV2 : IBankCashHandler
    {
        
        public APIResponse Cash(APITransaction transaction)
        {
            throw new NotImplementedException();
        }
        public APIResponse Check(APITransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
