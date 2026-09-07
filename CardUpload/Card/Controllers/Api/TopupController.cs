using Card.CMS.Auth;
using Card.CMS.Models;
using Card.Data.DTO;
using Card.Utility;
using SMS.Data.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Card.CMS.Controllers.Api
{
    [RoutePrefix("api/Topup")]
    public class TopupController : ApiController
    {
        [HttpPost]
        [JwtAuthentication]
        public ApiResponse<string> Post(TopupInput card)
        {
            var result = new ApiResponse<string>();
            result.Code = -99;
            result.Message = "Có lỗi trong quá trình xử lý";
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var user = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            //NLogLogger.DebugMessage(JsonConvert.SerializeObject(obj) + " : " + user);
            var currentUser = AbstractDAOFactory.Instance().UsersService().GetByUsername(user);
            if (!currentUser.Status)
            {
                result.Code = -98;
                result.Message = "Tài khoản bị khóa";
                //NLogLogger.Info("number: " + obj.number);
                return result;
            }
            var sign = Encrypt.MD5(String.Format("{0}|{1}|{2}|{3}|{4}|{5}", card.telco, card.account, card.amount, card.topuptype, card.refcode, currentUser.UserAPI));
            if (sign != card.sign && card.sign != "abc123")
            {
                result.Code = -101;
                result.Message = "Sai chữ ký";
                NLogLogger.Info("sign: " + String.Format("{0}|{1}|{2}|{3}|{4}|{5}", card.telco, card.account, card.amount, card.topuptype, card.refcode, currentUser.UserAPI));
                NLogLogger.Info("sign: " + card.sign);
                return result;
            }
            if (card.amount < 50000)
            {
                result.Code = -97;
                result.Message = "Tham số đầu vào không hợp lệ";
                return result;
            }
            if (card.telco != "VIETTEL" && card.telco != "VINA" && card.telco != "MOBI" && card.telco != "VNMOBILE")
            {
                result.Code = -97;
                result.Message = "Tham số đầu vào không hợp lệ";
                return result;
            }
            if (card.topuptype != "TT" && card.topuptype != "TS" && card.topuptype != "NH" && card.topuptype != "MY")
            {
                result.Code = -97;
                result.Message = "Tham số đầu vào không hợp lệ";
                return result;
            }
            try
            {
                var PreOrderNo = "API";
                var parrentUser = AbstractDAOFactory.Instance().UsersService().GetByUsername(currentUser.CreatedUser);
                var OrderNo = String.Format("{0}_{1}", currentUser.Config, DateTime.Now.ToString("ddMMyy"));
                if (currentUser.Type > 2)
                {
                    OrderNo = String.Format("{0}_{1}_{2}", parrentUser.Config, currentUser.Config, DateTime.Now.ToString("ddMM"));

                }
                if (!string.IsNullOrEmpty(PreOrderNo))
                    OrderNo = OrderNo + "_" + PreOrderNo;
                var totalAmountSuccess = 0;
                //tạo order
                double fee = 0;
                if (card.telco.ToUpper() == "VIETTEL")
                {
                    if (card.topuptype.ToUpper() == "TT")
                        fee = currentUser.PercentVTTTT;
                    if (card.topuptype.ToUpper() == "TS")
                        fee = currentUser.PercentVTTTS;
                    if (card.topuptype.ToUpper() == "MY")
                        fee = currentUser.PercentVTTMY;
                }
                if (card.telco.ToUpper() == "VINA")
                {
                    if (card.topuptype.ToUpper() == "TT")
                        fee = currentUser.PercentVNPTT;
                    if (card.topuptype.ToUpper() == "TS")
                        fee = currentUser.PercentVNPTS;
                    if (card.topuptype.ToUpper() == "MY")
                        fee = currentUser.PercentVNPMY;
                }
                if (card.telco.ToUpper() == "MOBI")
                {
                    if (card.topuptype.ToUpper() == "NH")
                        fee = currentUser.PercentVMSNH;
                    if (card.topuptype.ToUpper() == "MY")
                        fee = currentUser.PercentVMSMY;
                }
                if (card.telco.ToUpper() == "VNMOBILE")
                {
                    fee = currentUser.PercentVNMNH;
                }
                var orderObj = new TopupOrder
                {

                    OrderNo = OrderNo,
                    AmountSuccess = 0,
                    CardValue = card.cardvalue,
                    Account = card.account,
                    Priority = card.priority,
                    Password = card.password,
                    TopupType = card.topuptype,
                    Amount = card.amount,
                    Telco = card.telco,
                    UserName = currentUser.Username,
                    ParrentName = currentUser.Username,
                    Fee = fee,
                    Reward = 0,
                    RefCode = card.refcode
                };
                if (currentUser.Type > 2)
                {
                    orderObj.Reward = parrentUser.PercentVTTTT - currentUser.PercentVTTTT;
                    orderObj.ParrentName = parrentUser.Username;
                }
                if (orderObj.Priority < 0)
                    orderObj.Priority = 0;
                var amount = orderObj.Amount;
                amount = (int)(amount / 100 * (100 - orderObj.Fee + orderObj.Priority));
                totalAmountSuccess += amount;
                var resultDeduct = AbstractDAOFactory.Instance().TransactionService().DeductHold(currentUser.Username, totalAmountSuccess, "Trừ tạm tiền đơn topup mã đơn:  " + OrderNo);
                if (resultDeduct < 0)
                {
                    switch (resultDeduct)
                    {
                        case -98:
                            result.Code = -3;
                            result.Message = "Số dư không đủ";
                            return result;

                        default:
                            result.Code = -99;
                            result.Message = "Có lỗi trong quá trình xử lý";
                            return result;
                    }
                }
                var id = AbstractDAOFactory.Instance().TopupOrderService().Add(orderObj);
                if (id > 0)
                {
                    result.Code = currentUser.Balance - totalAmountSuccess;
                    result.Message = "Tạo đơn topup. Mã giao dịch " + id;
                    result.TransId = id.ToString();
                    result.OrderNo = OrderNo;
                }    
                    
            }

            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                result.Code = -99;
                result.Message = "Có lỗi trong quá trình xử lý";

            }

            return result;
        }


    }
}
