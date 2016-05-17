using BSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using BSK.Controllers;
using System.Data.Entity;

namespace BSK.Controllers
{
    public class AutorzyController : Controller
    {
        // GET: Autorzy
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
        [MyAuthorize(Roles = "autorzy_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var autor = baza.Autorzy.FirstOrDefault(k => k.ID_Autora == id);
                    List<Autor> autorzy = new List<Autor>();
                    autorzy.Add(autor);
                    odpowiedz.Data = autorzy;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "autorzy_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var autorzy = baza.Autorzy;
                    odpowiedz.Data = KonwertujAutorzy(autorzy);

                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "autorzy_update")]
        public JsonResult Put(Autor value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var autor = baza.Autorzy.FirstOrDefault(k => k.ID_Autora == value.ID_Autora);
                    autor.Imie = value.Imie;
                    autor.Nazwisko = value.Nazwisko;
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
        [MyAuthorize(Roles = "autorzy_insert")]
        public JsonResult Post(Autor value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    baza.Autorzy.Add(value);
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
        [MyAuthorize(Roles = "autorzy_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    baza.Autorzy.Remove(baza.Autorzy.FirstOrDefault(k => k.ID_Autora == id));
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private IEnumerable<Autor> KonwertujAutorzy(DbSet<Autor> autorzy)
        {
            var nowe = new List<Autor>();
            foreach (var autor in autorzy)
            {
                nowe.Add(new Autor { ID_Autora = autor.ID_Autora, Imie = autor.Imie, Nazwisko = autor.Nazwisko });
            }
            return nowe;
        }
    }
}