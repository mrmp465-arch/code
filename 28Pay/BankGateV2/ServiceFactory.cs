using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;

using Libs.BankDirect;
using Libs.BankGate;
using Libs.BankCash;
using Libs.Utils;

namespace BankGateV2
{
    public static class ServiceFactory
    {
        static Dictionary<string, IServiceHandler> dicts = new Dictionary<string, IServiceHandler>();

        static ServiceFactory()
        {
            
            //Register("unknown", new UnknownService());
            Register("bankdirect", new BankDirectV2Service());
            Register("bankcash", new BankCashService());
        }


        public static void Register(string providerCode, IServiceHandler hander)
        {

            switch (providerCode)
            {
                case "bankdirect":
                    {
                        dicts.Add("bankdirect", hander);
                        break;
                    }
                case "bankcash":
                    {
                        dicts.Add("bankcash", hander);
                        break;
                    }

                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IServiceHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                    case "bankdirect":
                        return dicts["bankdirect"];
                    case "bankcash":
                        return dicts["bankcash"];
                    default:
                        return dicts["unknown"];
                }


            }

            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "Error", ex.Message.Replace("\n", " ") });
                return null;
            }

        }
    }
}
