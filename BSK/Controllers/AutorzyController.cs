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
    public class AutorzyController : Controller
    {
        // GET: Autorzy
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "autorzy_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var autor = baza.Autorzy.FirstOrDefault(k => k.ID_Autora == id);
                    autor.Ksiazki.Clear();
                    odpowiedz.Data = autor.ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "autorzy_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var autorzy = baza.Autorzy.ToList();
                    for (int index = 0; index < autorzy.Count; index++)
                    {
                        autorzy[index].Ksiazki.Clear();
                    }
                    odpowiedz.Data = (autorzy.OrderBy(a => a.ID_Autora)).ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "autorzy_update")]
        public JsonResult Put(Autor value)
        {
            JsonResult odpowiedz = new JsonResult();

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
        [MyAuthorize(Roles = "autorzy_insert")]
        public JsonResult Post(Autor value)
        {
            JsonResult odpowiedz = new JsonResult();

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
        [MyAuthorize(Roles = "autorzy_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();

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
    }
}