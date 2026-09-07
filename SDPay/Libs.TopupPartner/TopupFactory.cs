using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.TopupPartner
{
    public static class TopupFactory
    {
        static Dictionary<string, ITopupHandler> dicts = new Dictionary<string, ITopupHandler>();

        static TopupFactory()
        {
            Register("unknown", new Unknown.UnknownTopup());
            Register("b2btranfer", new PayPlusOrder.PayPlusOrderBizTopupService());
        }


        public static void Register(string providerCode, ITopupHandler hander)
        {

            switch (providerCode)
            {

                case "b2btranfer":
                    {
                        dicts.Add("b2btranfer", hander);
                        break;
                    }
                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static ITopupHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {

                    case "b2btranfer":
                        return dicts["b2btranfer"];
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
