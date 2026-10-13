using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Title = "Home Page";
            var myHtmlFile = Server.MapPath("~/Index.html");
            if (!System.IO.File.Exists(myHtmlFile))
            {
                return HttpNotFound();
            }
            return Content(System.IO.File.ReadAllText(myHtmlFile), "text/html");
            //return View();
        }
    }
}
