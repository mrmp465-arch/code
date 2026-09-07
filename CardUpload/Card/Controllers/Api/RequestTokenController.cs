using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Card.CMS.Auth;
using Card.CMS.Models;

using Card.Utility;
using SMS.Data.Factory;

namespace Card.CMS.Controllers.Api
{
    public class RequestTokenController : ApiController
    {
        [HttpPost]
        public ApiResponse<string> Post(UserInfo user)
        {
            var result = new ApiResponse<string>();
            result.Code = -99;
            result.Message = "Có lỗi trong quá trình xử lý";
            var password = Encrypt.MD5(user.password.Trim() + Config.GetAppsetting("Salt"));
            if (AbstractDAOFactory.Instance().UsersService().Authentication(user.username.Trim(), password) > 0)
            {
                var m_Users = AbstractDAOFactory.Instance().UsersService().GetByUsername(user.username);
                result.Code = m_Users.Balance;
                result.Data = JwtAuthManager.GenerateJWTToken(user.username.Trim(), 24 * 60 * 1);
                result.Message = "Success";

            }
            else
            {

                result.Code = -11;
                result.Message = "Invalid Request";

            }
            return result;
        }
    }
}
