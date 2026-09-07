using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.TopupPartner
{
   public interface ITopupHandler
    {
       APIResponse TranferBalance(string requestId, string provider, int amount, string simTarget, string partnerCode, string providerCode);
    }
}
