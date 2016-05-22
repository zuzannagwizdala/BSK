using BSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSK.Controllers
{
    public class PoZalogowaniuController : Controller
    {
        // GET: PoZalogowaniu
        public ActionResult PoZalogowaniu()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }
    }
}