using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.BankGate;

namespace Libs.BankGate.Unknown
{
   public class UnknownBank : IBankGateHandler
    {
       public ResponseConfirmData Confirm(ReceiveConfirmData receive)
       {
            return new ResponseConfirmData{ResposeCode = (int)ResponseCode.ProviderNotFound };
       }
       
    }
}
