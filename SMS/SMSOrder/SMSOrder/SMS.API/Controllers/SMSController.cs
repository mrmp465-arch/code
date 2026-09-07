using Newtonsoft.Json;
using SMS.API.Auth;
using SMS.API.Models;
using SMS.Data.Api;
using SMS.Data.DTO;
using SMS.Data.Factory;
using SMS.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace SMS.API.Controllers
{
    [RoutePrefix("api/SMS")]
    public class SMSController : ApiController
    {

        [Route("authen")]
        [HttpPost]
        public HttpResponseMessage Authen(UserInfo user)
        {
            var password = Encrypt.MD5(user.password.Trim() + Config.GetAppsetting("Salt"));
            if (AbstractDAOFactory.Instance().UsersService().Authentication(user.username.Trim(), password) > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
             JwtAuthManager.GenerateJWTToken(user.username.Trim(), 24 * 60));
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized,
             "Invalid Request");
            }
        }

       
        [HttpPost]
        public ApiResponse<string> CallBack(SMSResponse request)
        {
            var result = new ApiResponse<string>();
            result.Code = -99;
            result.Message = "Có lỗi trong quá trình xử lý";
            NLogLogger.Info(JsonConvert.SerializeObject(request));
            var newclObj = JsonConvert.DeserializeObject<Callback>(request.ResponseContent);
            if (request.ResponseCode>0)
            {
                AbstractDAOFactory.Instance().SMSLogsService().UpdateRespone(long.Parse(newclObj.TransId), 0, 2, "");
            }
            else
            {
                AbstractDAOFactory.Instance().SMSLogsService().UpdateRespone(long.Parse(newclObj.TransId), 0, -1, request.Description);
            }

            //2 thành công 
            //-1 thất bại
            //AbstractDAOFactory.Instance().SMSLogsService().UpdateRespone(obj.id, 0, obj.status, obj.message, obj.port, obj.number);

            result.Code = 1;
            result.Message = "Success";
            return result;
        }

       

    }

}
