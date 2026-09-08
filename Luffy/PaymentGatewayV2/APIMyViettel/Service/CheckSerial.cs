using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;
using Libs.API;
using Libs.Utils;

namespace APIMyViettel.Service
{
    public class CheckSerial
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string GetToken(bool refresh = false)
        {

            var userName = "c89cce73f89529309145c9563377d904";
            var token = String.Empty;

            if (!refresh)
            {
                token = Utils.GetTokenCache(userName);

                if (!string.IsNullOrEmpty(token))
                {
                    return token;
                }
            }

            long curtime = (DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000;
            var url = "https://171.255.192.120:8115/BCCSGatewayWS/BCCSGatewayWS?wsdl";
            var sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
            sb.Append("<soapenv:Header />");
            sb.Append("<soapenv:Body>");
            sb.Append("<web:gwOperation>");
            sb.Append("<Input>");
            sb.Append("<!--Validate BCCSGateway:-->");
            sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
            sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
            sb.Append("<wscode>mbccs_loginBccs2</wscode>");
            sb.Append("<!--Zero or more repetitions:-->");
            sb.Append("<rawData><![CDATA[");
            sb.Append("<ws:login>");
            sb.Append("<mbccsRequestCode>MBCCS1</mbccsRequestCode>");
            sb.AppendFormat("<requestId>mbccs_loginBccs2;{0};375105705genClientKey</requestId>", curtime.ToString());
            sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
            sb.Append("<osType>Android</osType>");
            sb.Append("<networkType>PUBLIC</networkType>");
            sb.Append("<userName>1dd54a3a8a09bd77c380604c3d4a06d4</userName>");
            sb.Append("<passWord>b594a563bcd54c28ede52753392ea35e</passWord>");
            sb.Append("<addInfo>14fc22840211ab1ed617be90f58390ef</addInfo>");
            sb.AppendFormat("<clientTime>{0}</clientTime>", curtime.ToString());
            sb.Append("<version>3.3.9</version>");
            sb.Append("<serialSim>49389178951284723342</serialSim><osType>Android</osType><networkType>PUBLIC</networkType>");
            sb.Append("</ws:login>");
            sb.Append("]]></rawData>");
            sb.Append("</Input>");
            sb.Append("</web:gwOperation>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");
            sb.Append("");

            var res = Task.Run(() => Utils.PostTaskXml(url, sb.ToString())).Result;

            //Console.WriteLine(res);

            token = Regex.Match(res, @"(?<=token&gt;).*?(?=\&lt;)").Value;
            Utils.SetTokenCache(userName, token);
            return token;

        }
        public static APIResponse CheckCard(string serial)
        {
            NLogLogger.Info(new string[] { "CheckSerial", "CheckCard", "Request", serial });
            try
            {
                var token = GetToken();
                var captcha = "1234567";

                long curtime = (DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000;
                var url = "https://171.255.192.120:8115/BCCSGatewayWS/BCCSGatewayWS?wsdl";
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
                sb.Append("<soapenv:Header />");
                sb.Append("<soapenv:Body>");
                sb.Append("<web:gwOperation>");
                sb.Append("<Input>");
                sb.Append("<!--Validate BCCSGateway:-->");
                sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
                sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
                sb.Append("<wscode>mbccs_getInforCardNumber</wscode>");
                sb.Append("<!--Zero or more repetitions:-->");
                sb.Append("<rawData><![CDATA[<ws:getInforCardNumber><mbccsRequestCode>MBCCS1</mbccsRequestCode>");
                sb.AppendFormat("<input><requestId>mbccs_getInforCardNumber;{0};994864615</requestId>", curtime.ToString());
                sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
                sb.Append("<osType>Android</osType><vsaMenu>;2Gto3G.mbccs2;</vsaMenu><networkType>PUBLIC</networkType><version>3.3.9</version>");
                sb.AppendFormat("<token>{0}</token><serial>{1}</serial><captchaCode>{2}</captchaCode><regType>0</regType></input></ws:getInforCardNumber>]]></rawData>", token, serial, captcha);
                sb.Append("</Input>");
                sb.Append("</web:gwOperation>");
                sb.Append("</soapenv:Body>");
                sb.Append("</soapenv:Envelope>");
                sb.Append("");

                var resCheck = Task.Run(() => Utils.PostTaskXml(url, sb.ToString())).Result;
                var errorCode = Regex.Match(resCheck, @"(?<=errorCode&gt;).*?(?=\&lt;)").Value;
                var responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value;

                while (errorCode == "TOKEN_INVALID")
                {
                    token = GetToken(true);
                    sb.Clear();
                    sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
                    sb.Append("<soapenv:Header />");
                    sb.Append("<soapenv:Body>");
                    sb.Append("<web:gwOperation>");
                    sb.Append("<Input>");
                    sb.Append("<!--Validate BCCSGateway:-->");
                    sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
                    sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
                    sb.Append("<wscode>mbccs_getInforCardNumber</wscode>");
                    sb.Append("<!--Zero or more repetitions:-->");
                    sb.Append("<rawData><![CDATA[<ws:getInforCardNumber><mbccsRequestCode>MBCCS1</mbccsRequestCode>");
                    sb.AppendFormat("<input><requestId>mbccs_getInforCardNumber;{0};994864615</requestId>", curtime.ToString());
                    sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
                    sb.Append("<osType>Android</osType><vsaMenu>;2Gto3G.mbccs2;</vsaMenu><networkType>PUBLIC</networkType><version>3.3.9</version>");
                    sb.AppendFormat("<token>{0}</token><serial>{1}</serial><regType>0</regType></input></ws:getInforCardNumber>]]></rawData>", token, serial);
                    sb.Append("</Input>");
                    sb.Append("</web:gwOperation>");
                    sb.Append("</soapenv:Body>");
                    sb.Append("</soapenv:Envelope>");
                    sb.Append("");

                    resCheck = Task.Run(() => Utils.PostTaskXml(url, sb.ToString())).Result;
                    errorCode = Regex.Match(resCheck, @"(?<=errorCode&gt;).*?(?=\&lt;)").Value;
                    responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value;

                }

                //Console.WriteLine(resCheck);

                if (errorCode == "0")
                {

                    if (responeCode != "0")
                    {
                        return new APIResponse((int)ResponseCode.CardSerialInvalid);
                    }

                    var cardInfor = new CheckSerialResponse()
                    {
                        cardSerial = serial,
                        cardExpired = Regex.Match(resCheck, @"(?<=cardExpired&gt;).*?(?=\&lt;)").Value,
                        cardValue = Regex.Match(resCheck, @"(?<=cardValue&gt;).*?(?=\&lt;)").Value,
                        isdn = Regex.Match(resCheck, @"(?<=isdn&gt;).*?(?=\&lt;)").Value,
                        ownerName = Regex.Match(resCheck, @"(?<=ownerName&gt;).*?(?=\&lt;)").Value,
                        dateUsed = Regex.Match(resCheck, @"(?<=dateUsed&gt;).*?(?=\&lt;)").Value,

                    };

                    if (string.IsNullOrEmpty(cardInfor.isdn))
                    {
                        return new APIResponse()
                        {
                            ResponseCode = (int)ResponseCode.TransactionSuccessful,
                            ResponseContent = serializer.Serialize(cardInfor),
                            Description = "Thẻ chưa sử dụng"
                        };
                    }
                    else
                    {
                        return new APIResponse()
                        {
                            ResponseCode = (int)ResponseCode.CardUsed,
                            ResponseContent = serializer.Serialize(cardInfor),
                            Description = "Thẻ đã sử dụng"
                        };
                    }

                }

                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "CheckSerial", "CheckCard", "Error", e.Message, e.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }

        }

        public static APIResponse GetCaptcha(string serial)
        {
            NLogLogger.Info(new string[] { "CheckSerial", "CheckCard", "Request", serial });
            try
            {
                var token = GetToken();
                var captcha = "1234567";

                long curtime = (DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks) / 10000;
                var url = "https://171.255.192.120:8115/BCCSGatewayWS/BCCSGatewayWS?wsdl";
                var sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
                sb.Append("<soapenv:Header />");
                sb.Append("<soapenv:Body>");
                sb.Append("<web:gwOperation>");
                sb.Append("<Input>");
                sb.Append("<!--Validate BCCSGateway:-->");
                sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
                sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
                sb.Append("<wscode>mbccs_getCaptchaImagePublic</wscode>");
                sb.Append("<!--Zero or more repetitions:-->");
                sb.Append("<rawData><![CDATA[<ws:getInforCardNumber><mbccsRequestCode>MBCCS1</mbccsRequestCode>");
                sb.AppendFormat("<input><requestId>mbccs_getCaptchaImagePublic;;1538473798423;239852736</requestId>", curtime.ToString());
                sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
                sb.Append("<osType>Android</osType><vsaMenu>;2Gto3G.mbccs2;</vsaMenu><networkType>PUBLIC</networkType><version>3.3.9</version>");
                sb.AppendFormat("<token>{0}</token><regType>0</regType></input></ws:getCaptchaImage>]]></rawData>", token, serial, captcha);
                sb.Append("</Input>");
                sb.Append("</web:gwOperation>");
                sb.Append("</soapenv:Body>");
                sb.Append("</soapenv:Envelope>");
                sb.Append("");

                var resCheck = Task.Run(() => Utils.PostTaskXml(url, sb.ToString())).Result;
                var errorCode = Regex.Match(resCheck, @"(?<=errorCode&gt;).*?(?=\&lt;)").Value;
                var responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value;

                while (errorCode == "TOKEN_INVALID")
                {
                    token = GetToken(true);
                    sb.Clear();
                    sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:web=\"http://webservice.bccsgw.viettel.com/\">");
                    sb.Append("<soapenv:Header />");
                    sb.Append("<soapenv:Body>");
                    sb.Append("<web:gwOperation>");
                    sb.Append("<Input>");
                    sb.Append("<!--Validate BCCSGateway:-->");
                    sb.Append("<username>1dd54a3a8a09bd77c380604c3d4a06d4</username>");
                    sb.Append("<password>00d1fb1ab0c4d66af7d5f797cbcb600d</password>");
                    sb.Append("<wscode>mbccs_getInforCardNumber</wscode>");
                    sb.Append("<!--Zero or more repetitions:-->");
                    sb.Append("<rawData><![CDATA[<ws:getInforCardNumber><mbccsRequestCode>MBCCS1</mbccsRequestCode>");
                    sb.AppendFormat("<input><requestId>mbccs_getInforCardNumber;{0};994864615</requestId>", curtime.ToString());
                    sb.Append("<userName>c89cce73f89529309145c9563377d904</userName>");
                    sb.Append("<osType>Android</osType><vsaMenu>;2Gto3G.mbccs2;</vsaMenu><networkType>PUBLIC</networkType><version>3.3.9</version>");
                    sb.AppendFormat("<token>{0}</token><serial>{1}</serial><regType>0</regType></input></ws:getInforCardNumber>]]></rawData>", token, serial);
                    sb.Append("</Input>");
                    sb.Append("</web:gwOperation>");
                    sb.Append("</soapenv:Body>");
                    sb.Append("</soapenv:Envelope>");
                    sb.Append("");

                    resCheck = Task.Run(() => Utils.PostTaskXml(url, sb.ToString())).Result;
                    errorCode = Regex.Match(resCheck, @"(?<=errorCode&gt;).*?(?=\&lt;)").Value;
                    responeCode = Regex.Match(resCheck, @"(?<=responeCode&gt;).*?(?=\&lt;)").Value;

                }

                //Console.WriteLine(resCheck);

                if (errorCode == "0")
                {

                    if (responeCode != "0")
                    {
                        return new APIResponse((int)ResponseCode.CardSerialInvalid);
                    }

                    var cardInfor = new CheckSerialResponse()
                    {
                        cardSerial = serial,
                        cardExpired = Regex.Match(resCheck, @"(?<=cardExpired&gt;).*?(?=\&lt;)").Value,
                        cardValue = Regex.Match(resCheck, @"(?<=cardValue&gt;).*?(?=\&lt;)").Value,
                        isdn = Regex.Match(resCheck, @"(?<=isdn&gt;).*?(?=\&lt;)").Value,
                        ownerName = Regex.Match(resCheck, @"(?<=ownerName&gt;).*?(?=\&lt;)").Value,
                        dateUsed = Regex.Match(resCheck, @"(?<=dateUsed&gt;).*?(?=\&lt;)").Value,

                    };

                    if (string.IsNullOrEmpty(cardInfor.isdn))
                    {
                        return new APIResponse()
                        {
                            ResponseCode = (int)ResponseCode.TransactionSuccessful,
                            ResponseContent = cardInfor.cardValue,
                            Description = serializer.Serialize(cardInfor)
                        };
                    }
                    else
                    {
                        return new APIResponse()
                        {
                            ResponseCode = (int)ResponseCode.CardUsed,
                            ResponseContent = cardInfor.cardValue,
                            Description = serializer.Serialize(cardInfor)
                        };
                    }

                }

                return new APIResponse((int)ResponseCode.TransactionFailed);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "CheckSerial", "CheckCard", "Error", e.Message, e.StackTrace });
                return new APIResponse((int)ResponseCode.TransactionFailed);
            }

        }
    }
}