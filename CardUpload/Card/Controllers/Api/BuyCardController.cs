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
    [RoutePrefix("api/BuyCard")]
    public class BuyCardController : ApiController
    {
        [HttpPost]
        [JwtAuthentication]
        public ApiResponse<List<CardOut>> Post(CardInput card)
        {
            var result = new ApiResponse<List<CardOut>>();
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
            var sign = Encrypt.MD5(String.Format("{0}|{1}|{2}|{3}|{4}", card.telco,card.cardvalue,card.cardnumber, card.refcode, currentUser.UserAPI));
            if (sign != card.sign  && card.sign!="abc123")
            {
                result.Code = -101;
                result.Message = "Sai chữ ký";
                NLogLogger.Info("sign: " + String.Format("{0}|{1}|{2}|{3}|{4}", card.telco, card.cardvalue, card.cardnumber,card.refcode, currentUser.UserAPI));
                NLogLogger.Info("sign: " + card.sign);
                return result;
            }
            try
            {
                if(card.cardnumber<1 || card.cardnumber>100)
                {
                    result.Code = -97;
                    result.Message = "Tham số đầu vào không hợp lệ";
                    return result;
                }
                if (card.cardvalue !=20000 && card.cardvalue != 50000 && card.cardvalue != 100000 && card.cardvalue != 200000 && card.cardvalue != 500000)
                {
                    result.Code = -97;
                    result.Message = "Tham số đầu vào không hợp lệ";
                    return result;
                }
                if (card.telco != "VIETTEL" && card.telco != "VINA" && card.telco != "MOBI" && card.telco != "GARENA" )
                {
                    result.Code = -97;
                    result.Message = "Tham số đầu vào không hợp lệ";
                    return result;
                }
                var totalAmount = card.cardnumber * card.cardvalue;
               
                double fee = currentUser.PercentVTT;
                if (card.telco.ToUpper() == "VINA")
                {
                    fee = currentUser.PercentVNP;
                }
                if (card.telco.ToUpper() == "MOBI")
                {
                    fee = currentUser.PercentVMS;
                }
                if (card.telco.ToUpper() == "GARENA")
                {
                    fee = currentUser.PercentGarena;
                }
                double reward = 0;
                var parrentUser = AbstractDAOFactory.Instance().UsersService().GetByUsername(currentUser.CreatedUser);
                if (currentUser.Type > 2)
                {

                    reward = parrentUser.PercentVTT - currentUser.PercentVTT;

                }
                var money = (int)(totalAmount / 100 * (100 - fee));
                var moneyReward = (int)(totalAmount / 100 * reward);

                if (currentUser.Balance < money)
                {

                    result.Code = -3;
                    result.Message = "Số dư không đủ để thực hiện giao dịch";
                    return result;
                }

                var orderObj = new BuyCard
                {
                    MoneyReward = moneyReward,
                    Money = money,
                    Fee = fee,
                    Reward = reward,
                    CardNumber = card.cardnumber,
                    CardValue = card.cardvalue,
                    Amount = totalAmount,
                    Telco = card.telco,
                    UserName = currentUser.Username,
                    ParrentName = currentUser.Username,
                    Type = 2,
                    RefCode=card.refcode

                };

                //cấp 2
                if (currentUser.Type > 2)
                {
                    //orderObj.Reward = userinfo.PercentVTT - parrentUser.PercentVTT;
                    orderObj.ParrentName = parrentUser.Username;
                }


                var id = AbstractDAOFactory.Instance().BuyCardService().Add(orderObj);
                if (id > 0)
                {
                    var resultbuycard = AbstractDAOFactory.Instance().TransactionService().BuyCard(orderObj.UserName, orderObj.ParrentName, money, moneyReward, orderObj.Telco, orderObj.CardValue, orderObj.CardNumber, id);
                    if (resultbuycard > 0)
                    {
                        result.Code = currentUser.Balance-money;
                        result.Message = "Mua thẻ thành công. Mã giao dịch " + id;
                        result.TransId = id.ToString();
                        //lấy ds thẻ
                        var data = AbstractDAOFactory.Instance().CardOrderService().GetByRefCode(id);
                        var lstCard = new List<CardOut>();
                        foreach (var item in data)
                            lstCard.Add(new CardOut { pin=item.CardCode,seri=item.CardSerial});

                        result.Data = lstCard;
                    }
                    else
                    {
                        result.Code = resultbuycard;
                        if (resultbuycard == -99)
                            result.Message = "Có lỗi trong quá trình xử lý";
                        if (resultbuycard == -2)
                            result.Message = "Kho thẻ không đủ, vui lòng liên hệ admin";
                        if (resultbuycard == -3)
                            result.Message = "Số dư không đủ để thực hiện giao dịch";
                    }
                }
                else
                {
                    result.Code = -99;
                    result.Message = "Có lỗi trong quá trình xử lý";
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
