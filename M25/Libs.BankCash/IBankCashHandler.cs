using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.BankCash
{
   public interface IBankCashHandler
    {
       APIResponse Cash(APITransaction transaction);
        APIResponse Check(APITransaction transaction);
    }
}
