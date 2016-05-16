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
    public class SprzedazeController : Controller
    {
        // GET: Sprzedaze
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

        [HttpGet]
        [MyAuthorize(Roles = "sprzedaze_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaz = baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == id);
                    
                    odpowiedz.Data = sprzedaz.ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
                //= Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException);
            }
            return odpowiedz;
        }

        [HttpGet]
        [MyAuthorize(Roles = "sprzedaze_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaze = baza.Sprzedaze.ToList();
                    
                    odpowiedz.Data = sprzedaze.OrderBy(a => a.ID_Sprzedazy).ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPut]
        [MyAuthorize(Roles = "sprzedaze_update")]
        public JsonResult Put(Sprzedaz value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaz = baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == value.ID_Sprzedazy);
                    sprzedaz.Data_sprzedazy = value.Data_sprzedazy;
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
        [MyAuthorize(Roles = "sprzedaze_insert")]
        public JsonResult Post(Sprzedaz value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Sprzedaze.Add(value);
                    baza.SaveChanges();
                    
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpDelete]
        [MyAuthorize(Roles = "sprzedaze_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Sprzedaze.Remove(baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == id));
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