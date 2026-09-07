using Card.Data.DTO;
using Card.Data.Service;
using Card.CMS.Filter;
using Card.CMS.Models;

using Card.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Card.CMS.Controllers
{
    public class CommentController : Controller
    {
        public UserFunction Permission { get { return ((UserFunction)Session[SessionsManager.SESSION_PERMISSION]); } }
        private UserSession CurrentUser { get { return ((UserSession)Session[SessionsManager.SESSION_USER]); } }
        private readonly IContentsService _contentservice;
        private readonly ICommentsService _commentservice;
        private readonly IUsersService _userservice;
        private readonly IUsersLogService _userlogservice;
        private readonly IFucntionsService _functionservice;
        private readonly IGroupsService _groupservice;
        private readonly IUserRoleService _userroleservice;
        private readonly ITransactionsService _transervice;
        public CommentController(IContentsService contentservice, ICommentsService commentservice, IUsersService userservice, IUsersLogService userlogservice, IFucntionsService functionservice, IGroupsService groupservice, IUserRoleService userroleservice, ITransactionsService transervice)
        {
            _userservice = userservice;
            _userlogservice = userlogservice;
            _userroleservice = userroleservice;
            _functionservice = functionservice;
            _groupservice = groupservice;
            _transervice = transervice;
            _contentservice = contentservice;
            _commentservice = commentservice;
        }
        public ActionResult ListComment(int Id)
        {
            var data = new List<Comments>();


            data = _commentservice.GetTop(100, Id);
            if (data.Count > 0)
                ViewBag.TotalRecord = data.Count;
            else
                ViewBag.TotalRecord = 0;
            return PartialView(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Comments data)
        {
            if (Session["sendfeedback"] == null)
            {
                Session["sendfeedback"] = "1";
            }

            var countsession = Convert.ToInt32(Session["sendfeedback"].ToString());
            if (countsession >50)
            {
                return Json(-4);
            }
            data.CreatedUser = CurrentUser.Username;
            data.Title = CurrentUser.Username;
            data.Status = 1;
            var result =  _commentservice.InsertUpdate(data);

            Session["sendfeedback"] = (countsession + 1).ToString();
            return Json(result > 0 ? 1 : result);

        }
    }
}