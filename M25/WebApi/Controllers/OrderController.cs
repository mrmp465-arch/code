using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using APIMyMobi;
using Libs.Report;
using Libs.Utils;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using WebApi.Models;
using APIMyViettel;
namespace WebApi.Controllers
{


    [Authorize]
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {
        // GET api/Order/GroupActive?top=100&telco=vms&orderno=abc
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GroupActive")]
        public DataTable GetGroupActive(int top,int ussd=-1,string telco = "", string orderNo = "", string subUser = "")
        {
            if (top > 10000 || top <= 0) top = 10000;
            var userId = User.Identity.GetUserId();
            long totalSuccess = 0;
            long totalRequest = 0;
            long totalWaiting = 0;
            int? Ussd=null;
            if(ussd>=0)
            {
                Ussd = ussd;
            }
            var res = new TopupMobileLog().GetTableOrder(top, userId, telco, orderNo, DateTime.Now.AddDays(1), null, subUser, Ussd, ref totalSuccess, ref totalRequest, ref totalWaiting);

            return res;
        }

        // GET api/Order/GroupFinish?top=100&telco=vms&orderno=abc
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GroupFinish")]
        public DataTable GetGroupFinish(int top, string telco = "", string orderNo = "", string subUser = "")
        {
            if (top > 10000 || top <= 0) top = 10000;
            var userId = User.Identity.GetUserId();
            var res = new TopupMobileLog().GetTableOrderConfirm(top, userId, telco, orderNo, DateTime.Now.AddDays(1), null, subUser);
            return res;
        }

        // GET api/Order/List?top=100&telco=vms&status=1&oderno=AT01&isconfirm=0
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("List")]
        public DataTable GetGroupFinish(int top, string telco = "", int? status = null, string orderNo = "", int? isConfirm = null, int? ussd = null, string subUser = "", string mobile = "")
        {
            if (top > 10000 || top <= 0) top = 10000;
            var userId = User.Identity.GetUserId();
            var res = new TopupMobileLog().GetTable(top, userId, telco, String.Empty, mobile, 0, DateTime.Now.AddDays(1), status, null, orderNo, null, isConfirm, String.Empty, ussd, subUser);
            return res;
        }
        // GET api/Order/CountWaiting?userIds=123
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("CountWaiting")]
        
        public DataTable CountWaiting(string userIds)
        {
            //var userId = User.Identity.GetUserId();
            var res = new TopupMobileLog().CountWaiting(userIds);
            return res;
        }

        // GET api/Order/History?transactionid=1&status=1
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("history")]
        public DataTable GetHistory(int transactionid, int? status = null)
        {
            var userId = User.Identity.GetUserId();
            var res = new TopupMobileLog().GetTableTransactionExport(userId, string.Empty, transactionid, null, null);
            return res;
        }
        // POST api/Order/AddVTTAcount
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("AddVTTAccount")]
        public IHttpActionResult AddVTTAcount([FromBody] List<VTTAcountBindingModel> data)
        {

            if (!ModelState.IsValid)

            {
                return BadRequest(ModelState);
            }
            //var providers = string.Join(",", new Libs.API.Providers().GetListByUserId(Convert.ToInt32(User.Identity.GetUserId())).Select(x => x.ProviderCode).ToArray());
            //var lstTrans = new List<long>();
            foreach (var model in data)
            {
                var obj = new APIMyViettel.Account();
               
                obj.Source = User.Identity.Name;
                obj.AccountName = model.AccountName;
                obj.Password = model.Password;
                obj.Type = model.Type;
                obj.Status = -99;
                obj.InsertReview();
                
            }

            return Ok(1);

        }
        // POST api/Order/AddMulti
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("AddMulti")]
        public IHttpActionResult AddOrder([FromBody] List<AddOrderBindingModel> data)
        {

            if (!ModelState.IsValid)

            {
                return Ok("-99");
            }
            var providers = string.Join(",", new Libs.API.Providers().GetListByUserId(Convert.ToInt32(User.Identity.GetUserId())).Select(x => x.ProviderCode).ToArray());
            var lstTrans = new List<long>();
            foreach (var model in data)
            {
                var obj = new TopupMobileLog();
                obj.UserId = Convert.ToInt32(User.Identity.GetUserId());
                obj.UserName = User.Identity.Name;
                obj.Mobile = model.Mobile;
                obj.FullName = model.FullName;
                obj.OrderNo = model.OrderNo;
                obj.Telco = model.Telco;
                obj.TopupType = model.TopupType;
                obj.Amount = model.Amount;
                obj.AmountMin = model.AmountMin;
                obj.AmountMinAll = model.AmountMinAll;
                obj.Priority = model.Priority;
                obj.RequestNo = GenOrderCode();
                obj.AmountPending = 0;
                obj.AmountTopupSuccess = 0;
                obj.LogContent = "Add";

                switch (model.Status)
                {
                    case -9:
                        obj.Status = -9;
                        break;
                    case 0:
                        obj.Status = model.Telco.ToLower() == "vms" && string.IsNullOrEmpty(model.Password) ? -3 : 1;
                        break;
                    default:
                        return BadRequest(ModelState);
                }

                obj.Partners = string.Empty;
                obj.Providers = providers;
                obj.AccountName = model.AccountName;
                obj.Password = model.Password;
                obj.Ussd = model.Ussd;
                obj.CallbackUrl = model.CallbackUrl;
                obj.SubUser = model.SubUser;
                obj.ExtData = model.ExtData;
                //NLogLogger.Info(new string[] { "WebApi", "AddOrder Request", JsonConvert.SerializeObject(obj) });
                var result = obj.Add();
                lstTrans.Add(result);

            }

            return Ok(string.Join(",", lstTrans));

        }
        // POST api/Order/Add
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Add")]
        public IHttpActionResult AddOrder(AddOrderBindingModel model)
        {

            if (!ModelState.IsValid)

            {
                return BadRequest(ModelState);
            }
            var providers = string.Join(",", new Libs.API.Providers().GetListByUserId(Convert.ToInt32(User.Identity.GetUserId())).Select(x => x.ProviderCode).ToArray());
            var obj = new TopupMobileLog();
            obj.UserId = Convert.ToInt32(User.Identity.GetUserId());
            obj.UserName = User.Identity.Name;
            obj.Mobile = model.Mobile;
            obj.FullName = model.FullName;
            obj.OrderNo = model.OrderNo;
            obj.Telco = model.Telco;
            obj.TopupType = model.TopupType;
            obj.Amount = model.Amount;
            obj.AmountMin = model.AmountMin;
            obj.AmountMinAll = model.AmountMinAll;
            obj.Priority = model.Priority;
            obj.RequestNo = GenOrderCode();
            obj.AmountPending = 0;
            obj.AmountTopupSuccess = 0;
            obj.LogContent = "Add";
            switch (model.Status)
            {
                case -9:
                    obj.Status = -9;
                    break;
                case 0:
                    obj.Status = model.Telco.ToLower() == "vms" && string.IsNullOrEmpty(model.Password) ? -3 : 1;
                    break;
                default:
                    return BadRequest(ModelState);
            }
            obj.Partners = string.Empty;
            obj.Providers = providers;
            obj.AccountName = model.AccountName;
            obj.Password = model.Password;
            obj.Ussd = model.Ussd;
            obj.CallbackUrl = model.CallbackUrl;
            obj.SubUser = model.SubUser;
            obj.ExtData = model.ExtData;
            //NLogLogger.Info(new string[] { "WebApi", "AddOrder Request", JsonConvert.SerializeObject(obj) });
            var result = obj.Add();

            if (result <= 0)
            {
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Lock")]
        public IHttpActionResult LockOrder(string orderNo)
        {

            var topupOrder = new TopupMobileLog();

            try
            {
                var result = topupOrder.UpdateOrder(orderNo, null, -3, Convert.ToInt32(User.Identity.GetUserId()), null, 0);

                if (result != 0)
                {
                    if (result == -2)
                        return BadRequest("không thể chốt vì còn tồn tại giao dịch nghi vấn (-2), Liên hệ cấp cao hơn để review lại đơn hàng");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "WebAPI", "Edit", exp.Message });
            }

            return Ok();
        }
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UpdatePriority")]
        public IHttpActionResult UpdatePriority(string orderNo,int priority)
        {

            var topupOrder = new TopupMobileLog();

            try
            {
                var result = topupOrder.UpdateOrder(orderNo, priority, null, Convert.ToInt32(User.Identity.GetUserId()), 0, 0);

                if (result != 0)
                {
                    if (result == -2)
                        return BadRequest("không thể chốt vì còn tồn tại giao dịch nghi vấn (-2), Liên hệ cấp cao hơn để review lại đơn hàng");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "WebAPI", "Edit", exp.Message });
            }

            return Ok();
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Confirm")]
        public IHttpActionResult ConfirmOrder(string orderNo)
        {

            var topupOrder = new TopupMobileLog();

            try
            {
                var result = topupOrder.UpdateOrder(orderNo, null, null, Convert.ToInt32(User.Identity.GetUserId()), 1, 0);

                if (result != 0)
                {
                    if (result == -2)
                        return BadRequest("không thể chốt vì còn tồn tại giao dịch nghi vấn (-2), Liên hệ cấp cao hơn để review lại đơn hàng");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "WebAPI", "Edit", exp.Message });
            }

            return Ok();
        }
        // POST api/Order/ChangeStatus
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Edit")]
        public IHttpActionResult EditOrder(EditOrderBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.IsConfirm != null)
            {
                if (model.IsConfirm != 1)
                {
                    return BadRequest(ModelState);
                }

            }

            var obj = new TopupMobileLog();
            obj.TransactionID = model.TransactionID;
            obj.AmountMin = model.AmountMin;
            obj.AmountMinAll = model.AmountMinAll;
            obj.Status = model.Status;
            obj.UserId = Convert.ToInt32(User.Identity.GetUserId());
            obj.IsConfirm = model.IsConfirm;
            obj.Priority = model.Priority;
            obj.Password = model.Password;
            obj.BidRate = model.BidRate;
            obj.BidFee = model.BidFee;
            obj.Ussd = model.Ussd;
            obj.SubUser = model.SubUser;
           
            try
            {
                var result = obj.Update();

                if (result != 0)
                {
                    if (result == -2)
                        return BadRequest("không thể chốt vì còn tồn tại giao dịch nghi vấn (-2), Liên hệ cấp cao hơn để review lại đơn hàng");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "WebAPI", "Edit", exp.Message });
            }


            return Ok();
        }

        // POST api/Order/StartPause
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("StartPause")]
        public IHttpActionResult StartPauseOrder(StartPauseOrderBindingModel model)
        {

            // NLogLogger.Info(new string[] { "WebAPI", "StartPause", model.Status.ToString() });
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            switch (model.Status)
            {
                case 0:
                    model.Status = -9;
                    break;
                case 1:
                    model.Status = 1;
                    break;
                default:
                    return BadRequest(ModelState);
            }

            var obj = new TopupMobileLog();
            try
            {
                var result = obj.UpdateOrderByUser(Convert.ToInt32(User.Identity.GetUserId()), User.Identity.Name, model.Status);

                if (result != 0)
                {
                    NLogLogger.Info(new string[] { "WebAPI", "StartPause", result.ToString() });
                    return BadRequest(ModelState);
                }
            }
            catch (Exception exp)
            {
                NLogLogger.Info(new string[] { "WebAPI", "StartPause", exp.Message });
            }


            return Ok();
        }

        // GET api/Order/GroupActive?accountName=0936999961
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GetAccountVMS")]
        public APIMyMobi.Account GetAccountVMS(string accountName)
        {
            var res = new APIMyMobi.Account().GetAccount(accountName);
            return res;
        }

        // GET api/Order/Get?orderId=0936999961
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("Get")]
        public TopupMobileLog GetOrder(long orderId)
        {
            var res = new TopupMobileLog().Get(orderId);
            return res;
        }

        private string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m,0,1,2,3,4,5,6,7,8,9").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 17; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            //var timeSpan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
            return tmp.ToUpper(); //+ timeSpan.ToString();
        }

    }




}
