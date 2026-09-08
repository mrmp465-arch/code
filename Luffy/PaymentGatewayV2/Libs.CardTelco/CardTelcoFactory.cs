using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Libs.Utils;

namespace Libs.CardTelco
{
    public static class CardTelcoFactory
    {
        static Dictionary<string, ICardTelcoHandler> dicts = new Dictionary<string, ICardTelcoHandler>();

        static CardTelcoFactory()
        {

            //Unknown class
            Register("unknown", new Unknow.UnknownCard());

            
            Register("ppussd", new PayPlusUSSD.PayPlusCard());
            Register("bigussd", new PayPlusUSSD.PayPlusCard());
            Register("glbussd", new PayPlusUSSD.PayPlusCard());
            Register("mrtussd", new PayPlusUSSD.PayPlusCard());

            //PayPlus APP VTT
           

            //PayPlus Web
            Register("ppweb", new PayPlusWeb.PayPlusCard());
            Register("ppappvtt", new PayPlusAppV1VTT.PayPlusCard());
            Register("ppappvttv2", new PayPlusAppV2VTT.PayPlusCard());


           



            

          

            //Gate
            Register("mobo", new MoBo.MoBoCard());
            Register("onepice", new OnePice.OnePiceCard());

            //Khoai
            Register("khoaivms", new Khoai.KhoaiCard());
            Register("khoaivtt", new Khoai.KhoaiCard());
            Register("khoaivnp", new Khoai.KhoaiCard());
            Register("khoaizing", new Khoai.KhoaiCard());
            Register("nemvtt", new Khoai.KhoaiCard());
            //tiger
            Register("tigervms", new Tiger.TigerCard());
            Register("tigervtt", new Tiger.TigerCard());
            Register("tigervnp", new Tiger.TigerCard());
            Register("tigerzing", new Tiger.TigerCard());
            Register("tigervnm", new Tiger.TigerCard());

            Register("bb2dvtt", new BB2DPay.BB2DCard());
            Register("bb2dvnp", new BB2DPay.BB2DCard());
            Register("bb2dvms", new BB2DPay.BB2DCard());

            Register("bb2dgate", new BB2DGate.BB2DGateCard());

            Register("shipvms", new VinaPay.VinaPayCard());
            Register("shipvtt", new VinaPay.VinaPayCard());
            Register("shipvnp", new VinaPay.VinaPayCard());

            Register("shipzing", new VinaPay.VinaPayCard());
            Register("shipvcoin", new VinaPay.VinaPayCard());
            Register("shipgarena", new VinaPay.VinaPayCard());

            Register("alovms", new AloPay.AloPayCard());
            Register("alovtt", new AloPay.AloPayCard());
            Register("alovnp", new AloPay.AloPayCard());
        }


        public static void Register(string providerCode, ICardTelcoHandler hander)
        {

            switch (providerCode)
            {
                
                case "ppussd":
                    {
                        dicts.Add("ppussd", hander);
                        break;
                    }
                case "bigussd":
                    {
                        dicts.Add("bigussd", hander);
                        break;
                    }
                case "glbussd":
                    {
                        dicts.Add("glbussd", hander);
                        break;
                    }
                case "mrtussd":
                    {
                        dicts.Add("mrtussd", hander);
                        break;
                    }

                case "glbxhr":
                    {
                        dicts.Add("glbxhr", hander);
                        break;
                    }
                case "mrxxhr":
                    {
                        dicts.Add("mrxxhr", hander);
                        break;
                    }
                case "mrdxhr":
                    {
                        dicts.Add("mrdxhr", hander);
                        break;
                    }
                case "mraxhr":
                    {
                        dicts.Add("mraxhr", hander);
                        break;
                    }
               
                case "ppweb":
                    {
                        dicts.Add("ppweb", hander);
                        break;
                    }
                case "ppxhrv2":
                    {
                        dicts.Add("ppxhrv2", hander);
                        break;
                    }
                case "ppvnpgsm":
                    {
                        dicts.Add("ppvnpgsm", hander);
                        break;
                    }

                case "ppappvtt":
                    {
                        dicts.Add("ppappvtt", hander);
                        break;
                    }
                case "glbappvtt":
                    {
                        dicts.Add("glbappvtt", hander);
                        break;
                    }
               

                case "ppappvttv2":
                    {
                        dicts.Add("ppappvttv2", hander);
                        break;
                    }

                case "ppappmobi":
                    {
                        dicts.Add("ppappmobi", hander);
                        break;
                    }
                
                case "ppvmsgsm":
                    {
                        dicts.Add("ppvmsgsm", hander);
                        break;
                    }

                case "global":
                    {
                        dicts.Add("global", hander);
                        break;
                    }

                case "mobo":
                    {
                        dicts.Add("mobo", hander);
                        break;
                    }
                case "onepice":
                    {
                        dicts.Add("onepice", hander);
                        break;
                    }

                case "tigerzing":
                    {
                        dicts.Add("tigerzing", hander);
                        break;
                    }
                case "tigervnm":
                    {
                        dicts.Add("tigervnm", hander);
                        break;
                    }
                case "tigervms":
                {
                    dicts.Add("tigervms", hander);
                    break;
                }
                case "tigervtt":
                {
                    dicts.Add("tigervtt", hander);
                    break;
                }
                case "tigervnp":
                {
                    dicts.Add("tigervnp", hander);
                    break;
                }

                case "khoaizing":
                    {
                        dicts.Add("khoaizing", hander);
                        break;
                    }

                case "khoaivms":
                    {
                        dicts.Add("khoaivms", hander);
                        break;
                    }
                case "khoaivtt":
                    {
                        dicts.Add("khoaivtt", hander);
                        break;
                    }
                case "khoaivnp":
                    {
                        dicts.Add("khoaivnp", hander);
                        break;
                    }

                case "bb2dvtt":
                    {
                        dicts.Add("bb2dvtt", hander);
                        break;
                    }
                case "bb2dvnp":
                    {
                        dicts.Add("bb2dvnp", hander);
                        break;
                    }
                case "bb2dvms":
                    {
                        dicts.Add("bb2dvms", hander);
                        break;
                    }
                case "bb2dgate":
                    {
                        dicts.Add("bb2dgate", hander);
                        break;
                    }
                case "nemvtt":
                    {
                        dicts.Add("nemvtt", hander);
                        break;
                    }
                case "shipvms":
                    {
                        dicts.Add("shipvms", hander);
                        break;
                    }
                case "shipvtt":
                    {
                        dicts.Add("shipvtt", hander);
                        break;
                    }
                case "shipvnp":
                    {
                        dicts.Add("shipvnp", hander);
                        break;
                    }

                case "alovms":
                    {
                        dicts.Add("alovms", hander);
                        break;
                    }
                case "alovtt":
                    {
                        dicts.Add("alovtt", hander);
                        break;
                    }
                case "alovnp":
                    {
                        dicts.Add("alovnp", hander);
                        break;
                    }

                case "shipzing":
                    {
                        dicts.Add("shipzing", hander);
                        break;
                    }
                case "shipvcoin":
                    {
                        dicts.Add("shipvcoin", hander);
                        break;
                    }
                case "shipgarena":
                    {
                        dicts.Add("shipgarena", hander);
                        break;
                    }
                default:
                    {
                        dicts.Add("unknown".ToLower(), hander);
                        break;
                    }
            }

        }

        public static ICardTelcoHandler GetHandler(string providerCode)
        {
            try
            {
                switch (providerCode)
                {
                   
                    case "ppussd":
                        return dicts["ppussd"];
                    case "bigussd":
                        return dicts["bigussd"];
                    case "glbussd":
                        return dicts["glbussd"];
                    case "mrtussd":
                        return dicts["mrtussd"];

                    case "glbxhr":
                        return dicts["glbxhr"];
                    case "mrxxhr":
                        return dicts["mrxxhr"];
                    case "mrdxhr":
                        return dicts["mrdxhr"];
                  
                    case "mobo":
                        return dicts["mobo"];

                    case "onepice":
                        return dicts["onepice"];

                    case "tigervnm":
                        return dicts["tigervnm"];
                    case "tigervms":
                        return dicts["tigervms"];
                    case "tigervtt":
                        return dicts["tigervtt"];
                    case "tigervnp":
                        return dicts["tigervnp"];
                    case "tigerzing":
                        return dicts["tigerzing"];

                    case "nemvtt":
                        return dicts["nemvtt"];
                    case "khoaivms":
                        return dicts["khoaivms"];
                    case "khoaivtt":
                        return dicts["khoaivtt"];
                    case "khoaivnp":
                        return dicts["khoaivnp"];
                    case "khoaizing":
                        return dicts["khoaizing"];

                    case "bb2dvtt":
                        return dicts["bb2dvtt"];
                    case "bb2dvnp":
                        return dicts["bb2dvnp"];
                    case "bb2dvms":
                        return dicts["bb2dvms"];
                    case "bb2dgate":
                        return dicts["bb2dgate"];
                    case "shipvms":
                        return dicts["shipvms"];
                    case "shipvtt":
                        return dicts["shipvtt"];
                    case "shipvnp":
                        return dicts["shipvnp"];

                    case "alovms":
                        return dicts["alovms"];
                    case "alovtt":
                        return dicts["alovtt"];
                    case "alovnp":
                        return dicts["alovnp"];

                    case "shipzing":
                        return dicts["shipzing"];
                    case "shipvcoin":
                        return dicts["shipvcoin"];
                    case "shipgarena":
                        return dicts["shipgarena"];
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
