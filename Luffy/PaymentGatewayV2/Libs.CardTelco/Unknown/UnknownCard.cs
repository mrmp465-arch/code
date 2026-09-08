using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

namespace Libs.CardTelco.Unknow
{
   public class UnknownCard : ICardTelcoHandler
    {
       public APIResponse UseCard(APITransaction transaction)
       {
            return new APIResponse((int)ResponseCode.ProviderNotFound);
        }

       public APIResponse ReCheck(string transactionId)
       {
           throw new NotImplementedException();
       }
    }
}
