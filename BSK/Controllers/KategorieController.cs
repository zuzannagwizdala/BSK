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
    public class KategorieController : Controller
    {
        // GET: Kategorie
        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize(Roles = "kategorie_select")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var kategoria = baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == id);
                    kategoria.Ksiazki.Clear();
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(kategoria.ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "kategorie_select")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var kategorie = baza.Kategorie.ToList();
                    for (int index = 0; index < kategorie.Count; index++)
                    {
                        kategorie[index].Ksiazki.Clear();
                    }
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent((kategorie.OrderBy(a => a.ID_Kategorii)).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "kategorie_update")]
        public HttpResponseMessage Put(Kategoria value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var kategoria = baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == value.ID_Kategorii);
                    kategoria.Nazwa = value.Nazwa;
                    kategoria.Opis = value.Opis;
                    baza.SaveChanges();

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "kategorie_insert")]
        public HttpResponseMessage Post(Kategoria value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Kategorie.Add(value);
                    baza.SaveChanges();

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "kategorie_delete")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Kategorie.Remove(baza.Kategorie.FirstOrDefault(k => k.ID_Kategorii == id));
                    baza.SaveChanges();

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
    }
}