using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.BankDirect
{
   public interface IBankDirectV2Handler
    {
       APIResponse Order(APITransaction transaction);
        APIResponse OrderV2(APITransaction transaction);
        APIResponse CheckTrans(APITransaction transaction);
        APIResponse GetBanks(string partnercode);
        APIResponse GetBanksV2(string ParnerId, string AccoutName);
        APIResponse GetBanksV3(string ParnerId, string AccoutName);
    }
}
