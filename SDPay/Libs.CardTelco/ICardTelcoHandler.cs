using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.CardTelco
{
   public interface ICardTelcoHandler
   {
       APIResponse UseCard(APITransaction transaction);
       APIResponse ReCheck(string transactionId);
   }
}
