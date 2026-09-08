using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.TopupPartner
{
    public static class BuyCardFactory
    {
        static Dictionary<string, IBuyCardHandler> dicts = new Dictionary<string, IBuyCardHandler>();

        static BuyCardFactory()
        {
            Register("unknown", new Unknown.UnknownBuyCard());

            Register("paydirect", new PayDirect.PayDirectService());
            Register("gate", new Gate.GateService());
            Register("xbom", new XBom.VPGService());
            Register("pnow", new PaymentNow.PNowService());
            Register("1pay", new _1Pay._1PayService());
            Register("pushcard", new PayPlus.PayPlusService());
            Register("bpay", new M32.M32Service());
        }


        public static void Register(string providerCode, IBuyCardHandler hander)
        {

            switch (providerCode)
            {

                case "paydirect":
                    {
                        dicts.Add("paydirect", hander);
                        break;
                    }
                case "gate":
                    {
                        dicts.Add("gate", hander);
                        break;
                    }
                case "xbom":
                    {
                        dicts.Add("xbom", hander);
                        break;
                    }
                case "pnow":
                    {
                        dicts.Add("pnow", hander);
                        break;
                    }
                case "1pay":
                    {
                        dicts.Add("1pay", hander);
                        break;
                    }
                case "pushcard":
                {
                    dicts.Add("pushcard", hander);
                    break;
                }
                case "bpay":
                    {
                        dicts.Add("bpay", hander);
                        break;
                    }
                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IBuyCardHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {

                    case "paydirect":
                        return dicts["paydirect"];
                    case "gate":
                        return dicts["gate"];
                    case "xbom":
                        return dicts["xbom"];
                    case "pnow":
                        return dicts["pnow"];
                    case "1pay":
                        return dicts["1pay"];
                    case "pushcard":
                        return dicts["pushcard"];
                    case "bpay":
                        return dicts["bpay"];
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
