using BSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
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

        [HttpPost]
        public JsonResult Post(LogOutZapytanie dane)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            using (DB baza = new DB())
            {
                if (baza.Sesje.Any(s => s.ID_Sesji == dane.ID_Sesji))
                {
                    Sesja sesja = baza.Sesje.FirstOrDefault(s => s.ID_Sesji == dane.ID_Sesji);
                    baza.Sesje.Remove(sesja);
                    baza.SaveChanges();
                }
                //odpowiedz.StatusCode = HttpStatusCode.OK;
            }
            return odpowiedz;
        }

        [HttpPost]
        public JsonResult LogOutSession(string nazwa_uzytkownika)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";

            using (DB baza = new DB())
            {
                int? ID_roli = null;
                Rola rola = baza.Rolee.FirstOrDefault(r => r.Nazwa == nazwa_uzytkownika);

                if(rola != null)
                {
                    ID_roli = rola.ID_Roli;
                }
                else
                {
                    return odpowiedz;
                }
                List<Sesja> sesjeUzytkownika = baza.Sesje.Where(s => s.ID_Roli == ID_roli).ToList();   // wszystkie sesje uzytkownika
               
                for (int i = 0; i < sesjeUzytkownika.Count; i++)
                {
                    if (LogInController.konwertujNaStempel(DateTime.Now) < sesjeUzytkownika[i].Data_waznosci)
                    {
                        odpowiedz.Data = sesjeUzytkownika[i].ID_Sesji;
                    }
                }
                
            }
            return odpowiedz;
        }
    }
}