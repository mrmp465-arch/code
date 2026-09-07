using Card.Data.Api;
using Card.Data.DTO;
using Card.Utility;
using Newtonsoft.Json;
using SMS.Data.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Card.CMS.Controllers.Api
{
    [RoutePrefix("api/Order")]
    public class OrderTranController : ApiController
    {
        [Route("update")]

        public string Send(OrderCallback obj)
        {
            try
            {
                NLogLogger.DebugMessage(JsonConvert.SerializeObject(obj));
                if (obj.CreatTime == null || obj.CreatTime.GetValueOrDefault().Year < 2000)
                    obj.CreatTime = obj.updatetime;
                if (obj.status > 0)
                {

                    if (AbstractDAOFactory.Instance().OrderReportsService().GetByCode(obj.cardcode) == null)
                    {
                        var menhgia = obj.amount;
                        var order = AbstractDAOFactory.Instance().OrderReportsService().Get(obj.orderid);
                        if (order != null && order.OrderId > 0)
                        {

                            if (order.Amount - order.AmoutSuccess <= obj.amount)
                                obj.amount = order.Amount - order.AmoutSuccess;

                            order.AmoutSuccess += obj.amount;
                            var currenUser = AbstractDAOFactory.Instance().UsersService().GetByUsername(order.UserName);
                            //trừ tiền
                            var amountSuccess = obj.amount * order.Percent / 100;
                            var amountParrent = obj.amount * (order.Percent - order.PercentParrent) / 100;
                            AbstractDAOFactory.Instance().TransactionService().DeductCard(order.UserName, amountSuccess, order.OrderId, String.Format("Trừ tiền thẻ vào  đơn {0}, mệnh giá {1}, mã thẻ {2}", order.OrderNo + " - " + order.OrderId, menhgia, obj.cardcode), order.OrderId.ToString());

                            //cộng hoa hồng
                            if (amountParrent > 0 && currenUser.Type > 2)
                            {
                                AbstractDAOFactory.Instance().TransactionService().Topup(currenUser.CreatedUser, amountParrent, String.Format("Cộng tiền hoa hồng đơn {0}, mệnh giá {1}, mã thẻ {2}", order.OrderNo + " - " + order.OrderId, menhgia, obj.cardcode), order.OrderId.ToString(), 3);
                            }
                            //cộng hoa hồng c1
                            if (currenUser.Type == 5)
                            {
                                var amountC1 = obj.amount * (order.PercentParrent - order.PercentC1) / 100;
                                if (amountC1 > 0)
                                    AbstractDAOFactory.Instance().TransactionService().Topup(currenUser.C1User, amountC1, String.Format("Cộng tiền hoa hồng đơn {0}, mệnh giá {1}, mã thẻ {2}", order.OrderNo + " - " + order.OrderId, menhgia, obj.cardcode), order.OrderId.ToString(), 3);
                            }
                            var BidRate = order.BidRate;
                            if (obj.bidrate != null)
                                BidRate = obj.bidrate.GetValueOrDefault();
                            //ghi log
                            int bidFee = obj.amount * (int)BidRate / 100;
                            var orderhistory = new OrderReportHistory
                            {
                                OrderId = order.OrderId,
                                OrderNo = order.OrderNo,
                                Amount = obj.amount,
                                Mobile = order.Mobile,
                                Telco = order.Telco,
                                CreatedDate =  obj.updatetime.AddHours(7),
                                TopupType = order.TopupType,
                                UserApi = order.UserApi,
                                ParrentName = order.ParrentName,
                                C1Name = order.C1Name,
                                UserName = order.UserName,
                                Percent = order.Percent,
                                PercentParrent = order.PercentParrent,
                                PercentC1 = order.PercentC1,
                                PercentRoot = 0,
                                Type = order.Type,
                                Ussd = order.Ussd,
                                Priority = order.Priority,
                                BidFee = bidFee,
                                BidRate = BidRate,
                                CardCode = string.IsNullOrEmpty(obj.cardcode) ? "" : obj.cardcode,
                                CardSerial = string.IsNullOrEmpty(obj.cardserial) ? "" : obj.cardserial,
                            };

                            AbstractDAOFactory.Instance().OrderReportsService().AddHistory(orderhistory);
                            //var token = ServerProcess.GetUserTokenCache(order.UserApi, order.PasswordApi);
                            //trừ phí đua giá
                            if (order.BidRate > 0)
                            {

                                //trừ phí đua giá
                                AbstractDAOFactory.Instance().TransactionService().DeductBid(order.UserName, bidFee, order.OrderId, String.Format("Trừ phí đua giá đơn {0}, mệnh giá {1}, mã thẻ {2}", order.OrderNo + " - " + order.OrderId, menhgia, obj.cardcode), order.OrderNo);
                                //update phí đua giá vào core

                                //var orderApi = ServerProcess.GetOrder(token, obj.orderid);
                                //orderApi.BidFee += bidFee;
                                //ServerProcess.OrderUpdateBid(orderApi, token);


                                //ghi log đua giá
                                var logBid = new BidHistory
                                {
                                    OrderId = order.OrderId,
                                    OrderNo = order.OrderNo,
                                    UserID = 0,
                                    Description = String.Format("Trừ phí đua giá, thẻ {0}, mệnh giá {1}, phí đua giá {2}", obj.cardserial, menhgia, bidFee)
                                };
                                AbstractDAOFactory.Instance().BidHistoryService().InsertBidHistory(logBid);
                            };

                            //set cache count
                            var count = ServerProcess.GetCountCard(order.Telco, DateTime.Now.Hour);
                            count++;
                            ServerProcess.SetCountCard(count, order.Telco);

                            //set lai amount min
                            if (order.Amount - order.AmoutSuccess < order.AmountMin && order.Amount - order.AmoutSuccess > 0)
                            {
                                if (order.Amount - order.AmoutSuccess >= 50000)
                                {
                                    order.AmountMin = 50000;
                                }
                                else
                                {
                                    if (order.Amount - order.AmoutSuccess == 40000 || order.Amount - order.AmoutSuccess == 20000 || order.Amount - order.AmoutSuccess == 30000)
                                    {
                                        order.AmountMin = 20000;
                                    }
                                    else
                                    {
                                        order.AmountMin = 10000;
                                    }
                                }
                                NLogLogger.DebugMessage("update amount min " + order.OrderNo + " - " + order.OrderId);
                                var token = ServerProcess.GetUserTokenCache(order.UserApi, order.PasswordApi);
                                var orderApi = ServerProcess.GetOrder(token, obj.orderid);
                                orderApi.AmountMinAll = order.AmountMin;
                                ServerProcess.OrderUpdate(orderApi, token);
                                AbstractDAOFactory.Instance().OrderReportsService().UpdateAmount(order.OrderId, order.AmountMin);
                            }

                        }
                        else
                        {
                            NLogLogger.DebugMessage("Exist OderId" + obj.orderid);
                            return "-99|fail";
                        }
                    }
                    else
                    {
                        NLogLogger.DebugMessage("Exist Card code" + obj.cardcode);
                        return "-99|fail";
                    }
                        
                }
                else
                {
                    AbstractDAOFactory.Instance().OrderReportsService().UpdateStatus(obj.orderid, 0, obj.Description);
                    return "1|succes";
                }

                return "1|succes";
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return "-99|" + ex.Message;
            }
        }
        public class OrderCallback
        {
            public long orderid { get; set; }
            public int status { get; set; }
            public int amount { get; set; }
            public string cardserial { get; set; }

            public string cardcode { get; set; }

            public DateTime updatetime { get; set; }

            public DateTime? CreatTime { get; set; }

            public string Description
            {
                get; set;
            }
            public decimal? bidrate { get; set; }

        }
    }
}
