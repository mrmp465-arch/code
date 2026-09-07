using CMS.Data.DTO;
using CMS.Data.Factory;
using CMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Header()
        {
            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];
            return PartialView(userinfo);
        }
        public ActionResult HeaderMobile()
        {

            return PartialView();
        }
        public ActionResult Menu()
        {
            var userinfo = (UserSession)Session[SessionsManager.SESSION_USER];

            var functions = (List<Functions>)Session[SessionsManager.SESSION_FUNCTIONS];
            if (userinfo == null)
            {
                return PartialView(null);
            }
            if (functions == null)
            {
                if (userinfo.Type == 1)
                {
                    Session[SessionsManager.SESSION_FUNCTIONS] = AbstractDAOFactory.Instance().FunctionsService().GetListFunctionBySystemID(0);
                    Session[SessionsManager.SESSION_USERFUNCTIONS] = new List<UserFunction>();
                }
                else
                {
                    functions = AbstractDAOFactory.Instance().FunctionsService().GetListFunctionByUserID(userinfo.UserID);
                    Session[SessionsManager.SESSION_USERFUNCTIONS] = AbstractDAOFactory.Instance().UserRoleService().GroupFunction_GetByID(userinfo.Type);
                }
            }
            Session[SessionsManager.SESSION_FUNCTIONS] = functions;
            return PartialView(functions);
        }
    }
}