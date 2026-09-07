using CMS.Data.DTO;
using CMS.Utility;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMS.Filter
{
    public class PermissionFilter : ActionFilterAttribute, IActionFilter
    {
        //Your Properties in Action Filter
        public string FunctionCode { get; set; }
        public int FunctionType { get; set; }
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {

            filterContext.Controller.ViewBag.FunctionCode = FunctionCode;
            filterContext.Controller.ViewBag.IsAdmin = false;
            var functions = (List<Functions>)filterContext.HttpContext.Session[SessionsManager.SESSION_FUNCTIONS];
            var userinfo = (UserSession)filterContext.HttpContext.Session[SessionsManager.SESSION_USER];
            //filterContext.Controller.ViewBag.UserType = userinfo.Type;
            var userfunctions = (List<UserFunction>)filterContext.HttpContext.Session[SessionsManager.SESSION_USERFUNCTIONS];
            if (userinfo == null|| functions==null|| userfunctions==null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var viewResult = new JsonResult();
                    viewResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    viewResult.Data = (new { ResponseCode = -101, Description = "Bạn không có quyền sử dụng chức năng này" });
                    filterContext.Result = viewResult;
                    return;
                }
                else
                {
                    if (!filterContext.IsChildAction)
                    {


                        filterContext.Result = new RedirectToRouteResult(
                         new RouteValueDictionary
                         {
                                    { "controller", "Account" },
                                    { "action", "Login" } ,
                                    { "url", filterContext.HttpContext.Request.RawUrl} ,
                         });
                        return;
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                        return;
                    }
                }
            }

           
            if (!functions.Exists(c => c.FunctionCode == FunctionCode))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var viewResult = new JsonResult();
                    viewResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    viewResult.Data = (new { ResponseCode = -101, Description = "Bạn không có quyền sử dụng chức năng này" });
                    filterContext.Result = viewResult;
                    return;
                }
                else
                {
                    if (!filterContext.IsChildAction)
                    {


                        filterContext.Result = new RedirectToRouteResult(
                         new RouteValueDictionary
                         {
                                    { "controller", "Home" },
                                    { "action", "ErrorPermission" } 
                         });
                        return;
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                        return;
                    }
                }
                
            }
            var control = functions.Find(c => c.FunctionCode == FunctionCode);
            //var historyid = Session[SessionsManager.SESSION_HISTORY] != null
            //                    ? int.Parse(Session[SessionsManager.SESSION_HISTORY].ToString())
            //                    : 0;

            var Permission = new UserFunction { FunctionCode = FunctionCode };
            if (userinfo.Type == 1)
            {
                Permission.FunctionID = control.FunctionID;
                Permission.FatherName = control.FatherName;
                Permission.FatherID = control.FatherID;
                Permission.FunctionName = control.FunctionName;
                Permission.IsDelete = true;
                Permission.IsFullControl = true;
                Permission.IsInsert = true;
                Permission.IsUpdate = true;
                filterContext.Controller.ViewBag.IsAdmin = true;
            }
            else
            {
                
                Permission = userfunctions.Find(c => c.FunctionCode == FunctionCode);
                //permission = _userroleservice.
                //   CheckPermission(userinfo.UserID, control.FunctionID);
            }


            filterContext.HttpContext.Session[SessionsManager.SESSION_PERMISSION] = Permission;
            filterContext.HttpContext.Session[SessionsManager.SESSION_HISTORY] = control.FunctionCode;

            
            if (Permission != null)
            {
                filterContext.Controller.ViewBag.IsInsert = Permission.IsInsert || Permission.IsFullControl || Permission.IsUpdate;
                filterContext.Controller.ViewBag.IsUpdate = Permission.IsUpdate || Permission.IsFullControl;
                filterContext.Controller.ViewBag.IsFullControl = Permission.IsFullControl;
                filterContext.Controller.ViewBag.IsDelete = Permission.IsDelete || Permission.IsFullControl;
            }
            else
            {
                filterContext.Controller.ViewBag.IsInsert = false;
                filterContext.Controller.ViewBag.IsUpdate = false;
                filterContext.Controller.ViewBag.IsFullControl = false;
                filterContext.Controller.ViewBag.IsDelete = false;
            }
            if (!CheckPermison(Permission, FunctionCode, FunctionType))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {

                    var viewResult = new JsonResult();
                    viewResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    viewResult.Data = (new { ResponseCode = -101, Description = "Bạn không có quyền sử dụng chức năng này" });
                    filterContext.Result = viewResult;
                    return;
                }
                else
                {
                    if (!filterContext.IsChildAction)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                            { "controller", "Home" },
                            { "action", "ErrorPermission" } ,

                            });
                        return;
                    }
                    else
                    {
                        filterContext.Result = new EmptyResult();
                        return;
                    }
                }

            }
        }
        private bool CheckPermison(UserFunction permission, string functionCode, int functionType)
        {
            if (permission == null)
                return false;
            if (permission.FunctionCode != functionCode)
                return false;
            if (!permission.IsFullControl)
            {
                switch (functionType)
                {
                    case (int)Enums.FunctionType.IsView:
                        return true;
                    case (int)Enums.FunctionType.IsInsert:
                        if (!permission.IsInsert && !permission.IsUpdate)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    case (int)Enums.FunctionType.IsUpdate:
                        if (!permission.IsUpdate)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    case (int)Enums.FunctionType.IsDelete:
                        if (!permission.IsDelete)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    default:
                        return false;

                }
            }
            return true;
        }

    }
}