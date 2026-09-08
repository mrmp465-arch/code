using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.BankCash
{
    public static class BankCashFactory
    {
        static Dictionary<string, IBankCashHandler> dicts = new Dictionary<string, IBankCashHandler>();

        static BankCashFactory()
        {

            //Unknown class
            Register("unknown", new Unknown.UnknownBankV2());
            Register("imobankcash", new Imo.ImoBank());
            Register("imomomocash", new Imo.ImoBank());
            //Register("bankvnpaycash", new Jav.JavBank());
            //Register("momovnpaycash", new Jav.JavBank());
            Register("bpaybankcash", new Tox.ToxBank());
            //Register("toxmomocash", new Tox.ToxBank());

            Register("kenbankcash", new Ken.KenBank());
            Register("kenmomocash", new Ken.KenBank());
            Register("hynmomocash", new Jav.JavBank());
            Register("pushbankcash", new Jav.JavBank());
            Register("pushmomocash", new Jav.JavBank());
            //---------------------------

        }


        public static void Register(string providerCode, IBankCashHandler hander)
        {

            switch (providerCode)
            {
                case "hynmomocash":
                    {
                        dicts.Add("hynmomocash", hander);
                        break;
                    }
                case "imobankcash":
                    {
                        dicts.Add("imobankcash", hander);
                        break;
                    }

                case "imomomocash":
                    {
                        dicts.Add("imomomocash", hander);
                        break;
                    }
                case "bpaybankcash":
                    {
                        dicts.Add("bpaybankcash", hander);
                        break;
                    }

                //case "momovnpaycash":
                //    {
                //        dicts.Add("momovnpaycash", hander);
                //        break;
                //    }
                case "pushbankcash":
                    {
                        dicts.Add("pushbankcash", hander);
                        break;
                    }

                case "pushmomocash":
                    {
                        dicts.Add("pushmomocash", hander);
                        break;
                    }
                case "kenbankcash":
                    {
                        dicts.Add("kenbankcash", hander);
                        break;
                    }

                case "kenmomocash":
                    {
                        dicts.Add("kenmomocash", hander);
                        break;
                    }
                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IBankCashHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                    case "imobankcash":
                        return dicts["imobankcash"];
                    case "imomomocash":
                        return dicts["imomomocash"];

                    case "bpaybankcash":
                        return dicts["bpaybankcash"];
                    //case "momovnpaycash":
                    //    return dicts["momovnpaycash"];
                    case "pushmomocash":
                        return dicts["pushmomocash"];
                    case "pushbankcash":
                        return dicts["pushbankcash"];

                    case "kenbankcash":
                        return dicts["kenbankcash"];
                    case "kenmomocash":
                        return dicts["kenmomocash"];

                    case "hynmomocash":
                        return dicts["hynmomocash"];
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
