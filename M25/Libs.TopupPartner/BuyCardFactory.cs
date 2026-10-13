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
            Register("pl", new PayPlus.PayPlusService());
            Register("m32cash", new M32.M32Service());
            Register("pporder", new PayPlusOrder.PayPlusOrderBizBuyService());
            Register("b2bout", new PayPlusOrder.PayPlusOrderBizBuyService());
            Register("imedia", new Imedia.IMediaService());
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
                case "pl":
                {
                    dicts.Add("pl", hander);
                    break;
                }
                case "m32cash":
                    {
                        dicts.Add("m32cash", hander);
                        break;
                    }
                case "imedia":
                    {
                        dicts.Add("imedia", hander);
                        break;
                    }
                case "pporder":
                {
                    dicts.Add("pporder", hander);
                    break;
                }
                case "b2bout":
                {
                    dicts.Add("b2bout", hander);
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
                    case "pl":
                        return dicts["pl"];
                    case "m32cash":
                        return dicts["m32cash"];
                    case "imedia":
                        return dicts["imedia"];
                    case "pporder":
                        return dicts["pporder"];
                    case "b2bout":
                        return dicts["b2bout"];
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
