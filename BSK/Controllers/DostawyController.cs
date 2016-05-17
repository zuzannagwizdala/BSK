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
    public class DostawyController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult del()
        {
            return View();
        }

        public ActionResult update()
        {
            return View();
        }

        public ActionResult select()
        {
            return View();
        }

        public ActionResult insert()
        {
            return View();
        }

        [HttpPost]
        [MyAuthorize(Roles = "dostawy_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var dostawa = baza.Dostawy.FirstOrDefault(k => k.ID_Dostawy == id);
                    odpowiedz.Data = dostawa.ToString();

                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "dostawy_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var dostawy = baza.Dostawy.ToList();
                    odpowiedz.Data = (dostawy.OrderBy(a => a.ID_Dostawy)).ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "dostawy_update")]
        public JsonResult Put(Dostawa value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var dostawa = baza.Dostawy.FirstOrDefault(k => k.ID_Dostawy == value.ID_Dostawy);
                    dostawa.Data_dostawy = value.Data_dostawy;
                    dostawa.Dostawca = value.Dostawca;
                    dostawa.Ksiazki = value.Ksiazki;
                    baza.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "dostawy_insert")]
        public JsonResult Post(Dostawa value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    baza.Dostawy.Add(value);
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "dostawy_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    baza.Dostawy.Remove(baza.Dostawy.FirstOrDefault(k => k.ID_Dostawy == id));
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
    }
}