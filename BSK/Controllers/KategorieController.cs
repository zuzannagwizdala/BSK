using BSK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BSK.Controllers
{
    public class KategorieController : Controller
    {
        // GET: Kategorie
        public ActionResult Index()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }
        public ActionResult del()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }

        public ActionResult update()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }

        public ActionResult select()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }

        public ActionResult insert()
        {
            ViewBag.Message = Session["uprawnienia"];
            return View();
        }
        /*[HttpPost]
        [MyAuthorize(Roles = "kategorie_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var kategoria = baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == id);
                    odpowiedz.Data = kategoria.ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }*/
        [HttpPost]
        [MyAuthorize(Roles = "kategorie_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var kategorie = baza.Kategorie;
                    var posortowane = kategorie.OrderBy(a => a.ID_Kategorii);
                    odpowiedz.Data = KonwertujKategorie(posortowane);
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "kategorie_update")]
        public JsonResult Put(Kategoria value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var kategoria = baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == value.ID_Kategorii);
                    if (value.Nazwa != null)
                    {
                        kategoria.Nazwa = value.Nazwa;
                    }
                    if (value.Opis != null)
                    {
                        kategoria.Opis = value.Opis;
                    }
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
        [MyAuthorize(Roles = "kategorie_insert")]
        public JsonResult Post(Kategoria value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                if (value.Nazwa == null || value.Opis == null)
                {
                    odpowiedz.Data = "Uzupełnij wszystkie pola aby dodać kategorię!";
                    return odpowiedz;
                }

                using (DB baza = new DB())
                {
                    baza.Kategorie.Add(value);
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
        [MyAuthorize(Roles = "kategorie_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    baza.Kategorie.Remove(baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == id));
                    baza.SaveChanges();
                   
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private IEnumerable<Kategoria> KonwertujKategorie(IEnumerable<Kategoria> kategorie)
        {
            var nowe = new List<Kategoria>();
            foreach (var kat in kategorie)
            {
                nowe.Add(new Kategoria { ID_Kategorii = kat.ID_Kategorii, Nazwa = kat.Nazwa, Opis = kat.Opis });
            }
            return nowe;
        }
    }
}