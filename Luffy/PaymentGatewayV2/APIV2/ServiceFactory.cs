using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.API;
using Libs.CardTelco;
using Libs.TopupPartner;
using Libs.Utils;

namespace APIV2
{
    public static class ServiceFactory
    {
        static Dictionary<string, IServiceHandler> dicts = new Dictionary<string, IServiceHandler>();

        static ServiceFactory()
        {


            //Register("unknown", new UnknownService());
            Register("cardtelco", new CardTelcoService());
            Register("buycard", new BuyCardService());

        }


        public static void Register(string providerCode, IServiceHandler hander)
        {

            switch (providerCode)
            {
                case "cardtelco":
                    {
                        dicts.Add("cardtelco", hander);
                        break;
                    }
                case "buycard":
                    {
                        dicts.Add("buycard", hander);
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
                    case "cardtelco":
                        return dicts["cardtelco"];
                    case "buycard":
                        return dicts["buycard"];
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
