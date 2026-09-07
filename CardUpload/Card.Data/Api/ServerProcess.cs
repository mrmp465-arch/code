using Card.Data.DTO;
using Card.Utility;
using Newtonsoft.Json;
using SMS.Data.Factory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Card.Data.Api
{
    public class ServerProcess
    {
        private static readonly string Url = ConfigurationManager.AppSettings["Api_url"];
        private static readonly string UrlZing = ConfigurationManager.AppSettings["Api_Zing"];
        private static readonly string Callback = ConfigurationManager.AppSettings["CallBack"];
        public static string GetTelco(string accountName, string telco, string token)
        {
            try
            {
                try
                {
                    var requestUrl = string.Format("{0}api/Order/GetAccountVMS?accountName={1}&telco={2}", Url, accountName, telco);


                    var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);

                    NLogLogger.Info("GetAccountVMS: " + apiResponsetext);

                    if (string.IsNullOrEmpty(apiResponsetext))
                        return "";
                    var apiResponse = JsonConvert.DeserializeObject<TelcoInfo>(apiResponsetext);

                    return apiResponse.Password;
                }

                catch (Exception ex)
                {
                    //NLogLogger.PublishException(ex);

                    return "";
                }
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return "";

            }
        }
        public static string GetUserToken(string username, string password)
        {
            try
            {
                string postData = string.Format("grant_type=password&username={0}&password={1}", username, password);
                var requestUrl = string.Format("{0}token", Url);
                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData);
                //NLogLogger.DebugMessage(apiResponseText);
                var apiResponse = JsonConvert.DeserializeObject<UserToken>(apiResponseText);
                if (!string.IsNullOrEmpty(apiResponse.error))
                {
                    NLogLogger.DebugMessage(apiResponse);
                    NLogLogger.DebugMessage(postData);
                }
                else
                {
                    if (apiResponse.expires_in <= 0)
                    {
                        return "";
                    }
                }
                return apiResponse.access_token;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return "";

            }
        }
        public static string GetUserLockStatusCache(string username)
        {

            var keycache = string.Format("GetUserLockStatusCache-{0}", username);
            var status = "";
            var cachedata = RedisCaching.GetData(keycache);
            if (cachedata == null)
            {
                status = "1";
                RedisCaching.Add(keycache, status);
            }
            else
            {
                status = cachedata.ToString();
            }
            // NLogLogger.DebugMessage(token);
            return status;
        }
        public static void SetUserLockStatusCache(string username, string status)
        {

            var keycache = string.Format("GetUserLockStatusCache-{0}", username);
            RedisCaching.Add(keycache, status);
        }
        public static string GetUserTokenCache(string username, string password)
        {
            try
            {
                var keycache = string.Format("GetUserTokenCache-{0}", username);
                var token = "";
                var cachedata = RedisCaching.GetData(keycache);

                if (cachedata == null)
                {
                    token = GetUserToken(username, password);
                    if (!string.IsNullOrEmpty(token))
                    {
                        RedisCaching.Add(keycache, token);
                    }
                }
                else
                {
                    token = cachedata.ToString();
                }
                // NLogLogger.DebugMessage(token);
                return token;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return "";

            }

        }
        public static long OrderAddMulti(List<OrderInput> lstorder, string token, Users currentUser)
        {
            try
            {
                foreach (var order in lstorder)
                {
                    order.Mobile = StringUtils.FormatMobile(order.Mobile);
                    order.AccountName = StringUtils.FormatMobile(order.AccountName);
                    if (order.Telco.ToLower() == "vtt" || order.Telco.ToLower() == "vnp" || order.Telco.ToLower() == "vms")
                    {
                        if (order.TopupType != "3")
                        {
                            order.Mobile = StringUtils.RemoveNonNumeric(order.Mobile);
                        }
                    }
                    if (!order.OrderNo.StartsWith("RT_"))
                        order.OrderNo = "RT_" + order.OrderNo;


                    //xử lý admount min 10000
                    order.AmountMin = 0;
                    order.Priority += 40;


                    if (order.Telco.ToLower() != "zing" || order.Telco.ToLower() != "garena")
                    {

                        if (order.AmountMinAll <= 10000)
                        {
                            if (order.AmountMinAll == 0)
                            {
                                order.AmountMinAll = 10000;
                            }
                        }

                    }
                    if (order.Amount < order.AmountMinAll)
                        order.AmountMinAll = order.Amount;
                    var status = 0;
                    var lockstatus = GetUserLockStatusCache(currentUser.UserAPI);
                    if (lockstatus == "0")
                    {
                        status = -9;
                    }
                    order.Status = status;
                    order.CallbackUrl = Callback;
                    order.SubUser = currentUser.Username;
                    order.Telco = order.Telco.ToLower();


                }


                string postData = JsonConvert.SerializeObject(lstorder);
                NLogLogger.DebugMessage(postData);
                var requestUrl = string.Format("{0}api/Order/AddMulti", Url);
                var apiResponseText = Utilities.HttpRequestPostDataJson(requestUrl, postData, token);
                apiResponseText = apiResponseText.Replace("\"", "");
                //NLogLogger.DebugMessage(apiResponseText);

                if (apiResponseText == "-99")
                    return -99;
                var lstTran = apiResponseText.Split(',');
                int i = 0;
                foreach (var order in lstorder)
                {
                    // NLogLogger.DebugMessage(lstTran[i]);
                    //long orderId = long.Parse(lstTran[i]);
                    //add orderReport
                    var orderReport = new OrderReport
                    {
                        OrderId = long.Parse(lstTran[i]),
                        OrderNo = order.OrderNo,
                        Amount = order.Amount,
                        AmountMin = order.AmountMinAll,
                        Mobile = order.Mobile,
                        Telco = order.Telco.ToLower(),
                        Priority = order.Priority,
                        TopupType = int.Parse(order.TopupType),
                        UserApi = currentUser.UserAPI,
                        PasswordApi = currentUser.PasswordAPI,
                        ParrentName = currentUser.CreatedUser,
                        C1Name = currentUser.C1User,
                        UserName = currentUser.Username,
                        Percent = order.Percent,
                        PercentParrent = order.PercentParrent,
                        PercentC1 = order.PercentC1,
                        Type = 0,
                        Ussd = order.Ussd,

                    };
                    //cấp 1 tạo đơn
                    if (currentUser.Type == 3)
                    {
                        orderReport.ParrentName = currentUser.Username;
                        orderReport.C1Name = currentUser.Username;
                        orderReport.PercentC1 = orderReport.Percent;
                        orderReport.PercentParrent = orderReport.Percent;
                    }
                    //cấp 2 tạo đơn
                    //cấp 2 tạo đơn
                    if (currentUser.Type == 4)
                    {
                        //orderReport.ParrentName = currentUser.Username;
                        orderReport.C1Name = currentUser.CreatedUser;
                        orderReport.PercentC1 = orderReport.PercentParrent;
                        //orderReport.PercentParrent = orderReport.Percent;
                    }
                    if (order.Telco.ToLower() == "vtt")
                    {
                        //if (order.Ussd >= 1 || (order.TopupType == "1" && string.IsNullOrEmpty(order.Password)))
                        if ((order.TopupType == "2" && string.IsNullOrEmpty(order.Password)))
                        {
                            orderReport.Type = 1;
                        }
                        if ((order.TopupType == "1" && string.IsNullOrEmpty(order.Password)))
                        {
                            orderReport.Type = 1;
                        }
                        if (order.TopupType == "3" && order.Ussd > 0)
                        {
                            orderReport.Type = 1;
                        }
                    }
                    AbstractDAOFactory.Instance().OrderReportsService().Add(orderReport);

                    i++;
                }

                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                //NLogLogger.DebugMessage(postData);
                //NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long OrderAdd(OrderInput order, string token, Users currentUser, int percent, int percentParrent, int percentC1)
        {

            order.Mobile = StringUtils.FormatMobile(order.Mobile);
            order.AccountName = StringUtils.FormatMobile(order.AccountName);

            if (order.Telco.ToLower() == "vtt" || order.Telco.ToLower() == "vnp" || order.Telco.ToLower() == "vms")
            {
                if (order.TopupType != "3")
                {
                    order.Mobile = StringUtils.RemoveNonNumeric(order.Mobile);
                }
            }
            order.OrderNo = "RT_" + order.OrderNo;
            if (order.Amount < 5000)
                return -1;

            //xử lý admount min 10000
            order.AmountMin = 0;
            order.Priority += 40;



            if (order.Telco.ToLower() != "zing" || order.Telco.ToLower() != "garena")
            {

                if (order.AmountMinAll <= 10000)
                {
                    if (order.AmountMinAll == 0)
                    {
                        order.AmountMinAll = 10000;
                    }

                }

            }
            if (order.Amount < order.AmountMinAll)
                order.AmountMinAll = order.Amount;

            var status = 0;
            var lockstatus = GetUserLockStatusCache(currentUser.UserAPI);
            if (lockstatus == "0")
            {
                status = -9;
            }
            string postData = string.Format("Mobile={0}&FullName={1}&OrderNo={2}&TopupType={3}&Amount={4}&AmountMin={5}&AmountMinAll={6}&Priority={7}&Password={8}&AccountName={9}&Telco={10}&CallbackUrl={11}&Ussd={13}&Status={12}&SubUser={14}&ExtData={15}", order.Mobile, order.FullName, order.OrderNo, order.TopupType, order.Amount, order.AmountMin, order.AmountMinAll, order.Priority, order.Password, order.AccountName, order.Telco.ToLower(), Callback, status, order.Ussd, currentUser.Username, order.ExtData);
            NLogLogger.DebugMessage(postData);
            var requestUrl = string.Format("{0}api/Order/Add", Url);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                //NLogLogger.DebugMessage(apiResponseText);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId <= 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }

                //add orderReport
                var orderReport = new OrderReport
                {
                    OrderId = transId,
                    OrderNo = order.OrderNo,
                    Amount = order.Amount,
                    AmountMin = order.AmountMinAll,
                    Mobile = order.Mobile,
                    Telco = order.Telco,
                    Priority = order.Priority,
                    TopupType = int.Parse(order.TopupType),
                    UserApi = currentUser.UserAPI,
                    PasswordApi = currentUser.PasswordAPI,
                    ParrentName = currentUser.CreatedUser,
                    C1Name = currentUser.C1User,
                    UserName = currentUser.Username,
                    Percent = percent,
                    PercentParrent = percentParrent,
                    PercentC1 = percentC1,
                    Type = 0,
                    Ussd = order.Ussd,

                };
                //cấp 1 tạo đơn
                if (currentUser.Type == 3)
                {
                    orderReport.ParrentName = currentUser.Username;
                    orderReport.C1Name = currentUser.Username;
                    orderReport.PercentC1 = orderReport.Percent;
                    orderReport.PercentParrent = orderReport.Percent;
                }
                //cấp 2 tạo đơn
                if (currentUser.Type == 4)
                {
                    //orderReport.ParrentName = currentUser.Username;
                    orderReport.C1Name = currentUser.CreatedUser;
                    orderReport.PercentC1 = orderReport.PercentParrent;
                    //orderReport.PercentParrent = orderReport.Percent;
                }
                if (order.Telco.ToLower() == "vtt")
                {
                    //if (order.Ussd >= 1 || ( order.TopupType == "1" && string.IsNullOrEmpty(order.Password)))
                    if ((order.TopupType == "2" && string.IsNullOrEmpty(order.Password)))
                    {
                        orderReport.Type = 1;
                    }
                    if ((order.TopupType == "1" && string.IsNullOrEmpty(order.Password)))
                    {
                        orderReport.Type = 1;
                    }
                    if (order.TopupType == "3" && order.Ussd > 0)
                    {
                        orderReport.Type = 1;
                    }
                }

                AbstractDAOFactory.Instance().OrderReportsService().Add(orderReport);
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static List<OrderGroup> CountWaitingCache(string token)
        {
            try
            {
                var keycache = "CountWaitingCache";
                var data = new List<OrderGroup>();
                var cachedata = RedisCaching.GetData(keycache);

                if (cachedata == null)
                {
                    data = CountWaiting(token);
                    RedisCaching.Add(keycache, JsonConvert.SerializeObject(data), Constants.OneMinuteExpire * 5);
                }
                else
                {
                    data = JsonConvert.DeserializeObject<List<OrderGroup>>(cachedata.ToString());
                }
                // NLogLogger.DebugMessage(token);
                return data;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return null;

            }

        }
        public static List<OrderGroup> CountWaiting(string token)
        {

            var requestUrl = string.Format("{0}api/Order/CountWaiting?userIds={1}", Url, ConfigurationManager.AppSettings["UsersDL"].ToString());

            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, "", token);
                var apiResponse = JsonConvert.DeserializeObject<List<OrderGroup>>(apiResponseText);

                //if (!string.IsNullOrEmpty(orderNo))
                //{
                //    if (apiResponse.Count() > 0)
                //        apiResponse = apiResponse.Where(x => x.OrderNo.Contains(orderNo)).ToList();
                //}
                return apiResponse;




            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                //NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);

                return null;
            }
        }
        public static int StartPause(string status, string token, string UserAPI)
        {
            string postData = string.Format("Status={0}", status);
            var requestUrl = string.Format("{0}api/Order/StartPause", Url);

            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);

                NLogLogger.DebugMessage(apiResponseText);
                SetUserLockStatusCache(UserAPI, status);


                return 1;

            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);

                return -99;
            }
        }
        public static long OrderConfirm(OrderOutput order, string token)
        {

            string postData = string.Format("TransactionID={0}&AmountMin={1}&AmountMinAll={2}&Password={3}&Status={4}&Priority={5}&IsConfirm={6}&USSD={7}", order.TransactionID, order.AmountMin, order.AmountMinAll, order.Password, order.Status, order.Priority, order.IsConfirm, order.Ussd);
            var requestUrl = string.Format("{0}api/Order/Edit", Url);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                // NLogLogger.Info(apiResponseText);
                NLogLogger.DebugMessage(postData);
                //long transId = 0;
                //Int64.TryParse(apiResponseText, out transId);
                //if (transId < 0)
                //{
                //    NLogLogger.DebugMessage(apiResponseText);

                //}
                //return transId;
                return 1;

            }
            catch (WebException exception)
            {


                var responseStream = exception.Response?.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {

                        NLogLogger.PublishException(exception);
                        NLogLogger.Info(postData);
                        NLogLogger.Info(requestUrl);
                        NLogLogger.Info(reader.ReadToEnd());

                    }
                }
                return -99;
            }

        }
        public static long OrderUpdateFull(OrderOutput order, string token)
        {
            string postData = string.Format("TransactionID={0}&AmountMin={1}&AmountMinAll={2}&Password={3}&Status={4}&Priority={5}&USSD={6}", order.TransactionID, order.AmountMin, order.AmountMinAll, order.Password, order.Status, order.Priority, order.Ussd);
            var requestUrl = string.Format("{0}api/Order/Edit", Url);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long OrderLock(string orderNo, string token)
        {
            string postData = string.Format("orderNo={0}", orderNo);
            var requestUrl = string.Format("{0}api/Order/Lock?orderNo={1}", Url, orderNo);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                //NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long UpdatePriority(string orderNo, int Priority, string token)
        {
            string postData = string.Format("orderNo={0}", orderNo);
            var requestUrl = string.Format("{0}api/Order/UpdatePriority?orderNo={1}&priority={2}", Url, orderNo, Priority);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                //NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long OrderConfirm(string orderNo, string token)
        {
            string postData = string.Format("orderNo={0}", orderNo);
            var requestUrl = string.Format("{0}api/Order/Confirm?orderNo={1}", Url, orderNo);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                //NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long OrderUpdate(OrderOutput order, string token)
        {
            string postData = string.Format("TransactionID={0}&AmountMin={1}&AmountMinAll={2}&Password={3}&Status={4}&Priority={5}&USSD={6}", order.TransactionID, order.AmountMin, order.AmountMinAll, null, order.Status, order.Priority, order.Ussd);
            var requestUrl = string.Format("{0}api/Order/Edit", Url);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static long OrderUpdateBid(OrderOutput order, string token)
        {
            string postData = string.Format("TransactionID={0}&AmountMin={1}&AmountMinAll={2}&Status={3}&Priority={4}&BidFee={5}&BidRate={6}&USSD={7}", order.TransactionID, order.AmountMin, order.AmountMinAll, order.Status, order.Priority, order.BidFee, order.BidRate, order.Ussd);

            var requestUrl = string.Format("{0}api/Order/Edit", Url);
            try
            {

                var apiResponseText = Utilities.HttpRequestPostData(requestUrl, postData, token);
                NLogLogger.DebugMessage(postData);
                long transId = 0;
                Int64.TryParse(apiResponseText, out transId);
                if (transId < 0)
                {
                    NLogLogger.DebugMessage(apiResponseText);
                    NLogLogger.DebugMessage(postData);
                }
                return transId;


            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                NLogLogger.DebugMessage(postData);
                NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }
        public static OrderOutput GetOrder(string token, long orderId)
        {
            try
            {
                var requestUrl = string.Format("{0}api/Order/Get?orderId={1}", Url, orderId);


                var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);
                //NLogLogger.DebugMessage(apiResponsetext);

                var apiResponse = JsonConvert.DeserializeObject<OrderOutput>(apiResponsetext);
                //if (!string.IsNullOrEmpty(orderNo))
                //{
                //    if (apiResponse.Count() > 0)
                //        apiResponse = apiResponse.Where(x => x.OrderNo.Equals(orderNo)).ToList();
                //}
                return apiResponse;
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return null;
            }
        }
        public static List<OrderOutput> ListTransaction(string token, int top, string telco, string orderNo, int? status, int? isconfirm, string subUser = "", string mobile = "", int ussd = -1)
        {
            try
            {
                var requestUrl = string.Format("{0}api/Order/List?top={1}&telco={2}&orderNo={3}", Url, top, telco, orderNo);

                if (status.HasValue)
                {
                    requestUrl += "&status=" + status;
                }
                if (isconfirm.HasValue)
                {
                    requestUrl += "&isconfirm=" + isconfirm;
                }
                if (!string.IsNullOrEmpty(subUser))
                {
                    requestUrl += "&subUser=" + subUser;
                }
                if (!string.IsNullOrEmpty(mobile))
                {
                    requestUrl += "&mobile=" + mobile;
                }
                if (ussd > 0)
                {
                    requestUrl += "&ussd=" + ussd;
                }
                var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);
                //NLogLogger.DebugMessage(apiResponsetext);

                var apiResponse = JsonConvert.DeserializeObject<List<OrderOutput>>(apiResponsetext);
                //if (!string.IsNullOrEmpty(orderNo))
                //{
                //    if (apiResponse.Count() > 0)
                //        apiResponse = apiResponse.Where(x => x.OrderNo.Equals(orderNo)).ToList();
                //}
                return apiResponse;
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return new List<OrderOutput>();
            }
        }
        public static List<OrderDetail> ListTransactionHistory(string token, int transactionid, int? status)
        {
            try
            {
                var requestUrl = string.Format("{0}api/Order/history?transactionid={1}", Url, transactionid);

                if (status.HasValue)
                {
                    requestUrl += "&status=" + status;
                }

                var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);
                //NLogLogger.DebugMessage(apiResponsetext);

                var apiResponse = JsonConvert.DeserializeObject<List<OrderDetail>>(apiResponsetext);

                return apiResponse;
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return null;
            }
        }
        public static List<OrderGroup> ListGroupActive(string token, int top, int ussd, string telco, string orderNo, string subUser)
        {
            try
            {
                var requestUrl = string.Format("{0}api/Order/GroupActive?top={1}&telco={2}&orderNo={3}", Url, top, telco, orderNo);
                if (!string.IsNullOrEmpty(subUser))
                {
                    requestUrl += "&subUser=" + subUser;
                }
                if (ussd > -1)
                    requestUrl += "&ussd=" + ussd;
                //NLogLogger.DebugMessage(requestUrl);
                var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);
                //NLogLogger.DebugMessage(apiResponsetext);

                var apiResponse = JsonConvert.DeserializeObject<List<OrderGroup>>(apiResponsetext);

                //if (!string.IsNullOrEmpty(orderNo))
                //{
                //    if (apiResponse.Count() > 0)
                //        apiResponse = apiResponse.Where(x => x.OrderNo.Contains(orderNo)).ToList();
                //}
                return apiResponse;
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return null;
            }
        }
        public static List<OrderGroup> ListGroupFinish(string token, int top, string telco, string orderNo, string subUser)
        {
            try
            {
                var requestUrl = string.Format("{0}api/Order/GroupFinish?top={1}&telco={2}&orderNo={3}", Url, top, telco, orderNo);
                if (!string.IsNullOrEmpty(subUser))
                {
                    requestUrl += "&subUser=" + subUser;
                }
                //NLogLogger.Info(requestUrl);
                //NLogLogger.Info(token);
                var apiResponsetext = Utilities.HttpRequestGet(requestUrl, token);
                //NLogLogger.DebugMessage(apiResponsetext);

                var apiResponse = JsonConvert.DeserializeObject<List<OrderGroup>>(apiResponsetext);
                //if (!string.IsNullOrEmpty(orderNo))
                //{
                //    if (apiResponse.Count() > 0)
                //        apiResponse = apiResponse.Where(x => x.OrderNo.Contains(orderNo)).ToList();
                //}
                return apiResponse;
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);

                return null;
            }
        }
        public static string getSearchOrder(Users user, bool isOnwer)
        {
            var searchtext = "";
            //chỉ lấy của nó
            if (isOnwer)
            {
                searchtext = String.Format("{0}_", user.Config);
                return searchtext;
            }
            //lấy của cha và con
            if (user.Type == 2)
            {
                searchtext = String.Format("{0}_", user.Config);
            }
            if (user.Type == 3)
            {
                searchtext = String.Format("{0}_", user.Config);
            }
            return searchtext;
        }
        public static void SetBidCache(string Order)
        {

            var keycache = "Bib_" + Order;

            RedisCaching.Add(keycache, "1", Constants.OneMinuteExpire * 2);

        }
        public static int GetBidCache(string Order)
        {

            var keycache = "Bib_" + Order;

            var cachedata = RedisCaching.GetData(keycache);
            if (cachedata == null)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public static void SetCountCard(int count, string Telco)
        {

            var keycache = "CountCard_" + Telco + DateTime.Now.Hour;
            //if (DateTime.Now.Minute < 30)
            //{
            //    keycache += "_0";
            //}
            //else
            //{
            //    keycache += "_1";
            //}
            RedisCaching.Add(keycache, count.ToString(), Constants.OneHourExpire * 2);

        }
        public static int GetCountCard(string Telco, int Hour)
        {

            var keycache = "CountCard_" + Telco + Hour;

            var cachedata = RedisCaching.GetData(keycache);
            if (cachedata == null)
            {
                return 0;
            }
            else
            {
                return int.Parse(cachedata.ToString());
            }
        }

        public static long AddVTTAccount(List<VTTAccount> lstorder, string token)
        {
            try
            {


                string postData = JsonConvert.SerializeObject(lstorder);
                //NLogLogger.DebugMessage(postData);
                var requestUrl = string.Format("{0}api/Order/AddVTTAccount", Url);
                var apiResponseText = Utilities.HttpRequestPostDataJson(requestUrl, postData, token);
                apiResponseText = apiResponseText.Replace("\"", "");

                if (apiResponseText == "-99")
                    return -99;


                return 1;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                //NLogLogger.DebugMessage(postData);
                //NLogLogger.DebugMessage(requestUrl);
                return -99;

            }
        }

    }

}
