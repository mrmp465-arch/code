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
            
            //Register("m32", new M32.M32Bank());
            Register("imobank", new ImoBank.ImoBank());

            Register("imovtp", new ImoBank.ImoBank());
            Register("imo", new ImoBank.ImoBank());
            Register("imomomo", new ImoBank.ImoBank());

            Register("hynmomo", new HynBank.HynBank());
            Register("hynbank", new Libs.BankDirect.HynBank.HynBank());
            Register("pushbank", new Libs.BankDirect.HynBank.HynBank());
            Register("pushmomo", new Libs.BankDirect.HynBank.HynBank());
            //---------------------------
            Register("kenbank", new Ken.KenBank());
            Register("kenmomo", new Ken.KenBank());

        }


        public static void Register(string providerCode, IBankDirectV2Handler hander)
        {

            switch (providerCode)
            {

             
                
                //case "khoaimomo":
                //    {
                //        dicts.Add("khoaimomo", hander);
                //        break;
                //    }
                case "imovtp":
                    {
                        dicts.Add("imovtp", hander);
                        break;
                    }
                case "imo":
                    {
                        dicts.Add("imo", hander);
                        break;
                    }
                case "imobank":
                    {
                        dicts.Add("imobank", hander);
                        break;
                    }
                case "imomomo":
                    {
                        dicts.Add("imomomo", hander);
                        break;
                    }
                case "hynmomo":
                    {
                        dicts.Add("hynmomo", hander);
                        break;
                    }
                case "hynbank":
                    {
                        dicts.Add("hynbank", hander);
                        break;
                    }
                case "bpaybank":
                    {
                        dicts.Add("bpaybank", hander);
                        break;
                    }
                //case "vnpaybank":
                //    {
                //        dicts.Add("vnpaybank", hander);
                //        break;
                //    }
                case "pushbank":
                    {
                        dicts.Add("pushbank", hander);
                        break;
                    }
                case "pushmomo":
                    {
                        dicts.Add("pushmomo", hander);
                        break;
                    }
                //    }
                case "kenmomo":
                    {
                        dicts.Add("kenmomo", hander);
                        break;
                    }
                case "kenbank":
                    {
                        dicts.Add("kenbank", hander);
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

                    case "bpaybank":
                        return dicts["bpaybank"];

                    case "imobank":
                        return dicts["imobank"];
                    case "imovtp":
                        return dicts["imovtp"];
                    case "imo":
                        return dicts["imo"];
                    case "imomomo":
                        return dicts["imomomo"];
                    case "hynmomo":
                        return dicts["hynmomo"];
                    case "hynbank":
                        return dicts["hynbank"];
                    //case "vnpaymomo":
                    //    return dicts["vnpaymomo"];
                    case "pushmomo":
                        return dicts["pushmomo"];
                    case "pushbank":
                        return dicts["pushbank"];

                    case "kenmomo":
                        return dicts["kenmomo"];
                    case "kenbank":
                        return dicts["kenbank"];
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
