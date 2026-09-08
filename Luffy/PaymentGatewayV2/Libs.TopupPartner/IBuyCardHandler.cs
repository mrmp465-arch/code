using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.TopupPartner
{
   public interface IBuyCardHandler
    {
        APIResponse downloadSoftpin(string requestId, string provider, int amount, int quantity, ref string providerResponse);
        int checkStore(string provider, int amount);
    }
}
