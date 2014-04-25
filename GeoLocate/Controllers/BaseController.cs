using GeoLocate.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace GeoLocate.Controllers
{
    [AuthorizeUser]
    public class BaseController : Controller
    {
        public IPrincipal AuthUser
        {
            get
            {
                return User;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserInfo = this.AuthUser;
            }
            else
            {
                RedirectToAction("Login", "Account");
            }
        }
    }
}