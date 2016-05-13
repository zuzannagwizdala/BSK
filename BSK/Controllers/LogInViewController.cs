using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSK.Controllers
{
    public class LogInViewController : Controller
    {
        // GET: LogInView
        public ActionResult LogIn()
        {
            return View();
        }
    }
}