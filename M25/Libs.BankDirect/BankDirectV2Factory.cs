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
            
            Register("drummomo", new MDrumV2.MDrumV2Bank());
            Register("drumbank", new MDrumV2.MDrumV2Bank());


            Register("fastpaymomo", new FastPay.fastBank());

            Register("fastpaybank", new FastPay.fastBank());

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
                case "drummomo":
                {
                    dicts.Add("drummomo", hander);
                    break;
                }
               
                case "drumbank":
                    {
                        dicts.Add("drumbank", hander);
                        break;
                    }
                case "fastpaybank":
                    {
                        dicts.Add("fastpaybank", hander);
                        break;
                    }
                case "fastpaymomo":
                    {
                        dicts.Add("fastpaymomo", hander);
                        break;
                    }

             
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
                    case "drummomo":
                        return dicts["drummomo"];
                  
                    case "drumbank":
                        return dicts["drumbank"];

                    case "fastpaymomo":
                        return dicts["fastpaymomo"];
                    case "fastpaybank":
                        return dicts["fastpaybank"];
                  
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
