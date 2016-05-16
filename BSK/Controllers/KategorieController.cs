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
        }

        [HttpGet]
        [MyAuthorize(Roles = "kategorie_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var kategorie = baza.Kategorie;
                    odpowiedz.Data = KonwertujKategorie(kategorie);
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPut]
        [MyAuthorize(Roles = "kategorie_update")]
        public JsonResult Put(Kategoria value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var kategoria = baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == value.ID_Kategorii);
                    kategoria.Nazwa = value.Nazwa;
                    kategoria.Opis = value.Opis;
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

            try
            {
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

        [HttpDelete]
        [MyAuthorize(Roles = "kategorie_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();

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
        private IEnumerable<Kategoria> KonwertujKategorie(DbSet<Kategoria> kategorie)
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