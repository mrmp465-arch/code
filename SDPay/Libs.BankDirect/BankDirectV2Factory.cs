using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.BankDirect
{
    public static class BankDirectV2Factory
    {
        static Dictionary<string, IBankDirectV2Handler> dicts = new Dictionary<string, IBankDirectV2Handler>();

        static BankDirectV2Factory()
        {

            //Unknown class
            Register("unknown", new Unknown.UnknownBankV2());
            //Register("usdt", new TraoDoiUSDT.TraoDoiUSDTBank());
            
            Register("fastpaymomo", new FastPay.fastBank());
            Register("drumbank", new MDrumV2.MDrumV2Bank());
            Register("24hbank", new _24h._24hBank());
            //Register("gpay", new GPay.GpayBank());
            //Register("caspay", new Caspay.CaspayBank());
            //---------------------------



        }


        public static void Register(string providerCode, IBankDirectV2Handler hander)
        {

            switch (providerCode)
            {

                //case "usdt":
                //    {
                //        dicts.Add("usdt", hander);
                //        break;
                //    }
                case "fastpaymomo":
                {
                    dicts.Add("fastpaymomo", hander);
                    break;
                }
                case "24hbank":
                    {
                        dicts.Add("24hbank", hander);
                        break;
                    }
                case "drumbank":
                    {
                        dicts.Add("drumbank", hander);
                        break;
                    }

                //case "gpay":
                //    {
                //        dicts.Add("gpay", hander);
                //        break;
                //    }

                //case "caspay":
                //    {
                //        dicts.Add("caspay", hander);
                //        break;
                //    }


                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static IBankDirectV2Handler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                    //case "usdt":
                    //    return dicts["usdt"];
                    case "fastpaymomo":
                        return dicts["fastpaymomo"];
                  
                    case "drumbank":
                        return dicts["drumbank"];
                   
                    case "24hbank":
                        return dicts["24hbank"];

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
