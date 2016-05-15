using BSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BSK.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            return View();
        }
        public HttpResponseMessage Post(LogOutZapytanie dane)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();
            using (DB baza = new DB())
            {
                if (baza.Sesje.Any(s => s.ID_Sesji == dane.ID_Sesji))
                {
                    Sesja sesja = baza.Sesje.FirstOrDefault(s => s.ID_Sesji == dane.ID_Sesji);
                    baza.Sesje.Remove(sesja);
                    baza.SaveChanges();
                }
                odpowiedz.StatusCode = HttpStatusCode.OK;
            }
            return odpowiedz;
        }
    }
}