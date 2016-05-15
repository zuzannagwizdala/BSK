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
    public class KsiazkiController : Controller
    {
        // GET: Ksiazki
        public ActionResult Index()
        {
            return View();
        }
        [MyAuthorize(Roles = "ksiazki_select")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var ksiazka = baza.Ksiazki.FirstOrDefault(k => k.ID_Ksiazki == id);
                    ksiazka.Autor = new Autor { Imie = ksiazka.Autor.Imie, Nazwisko = ksiazka.Autor.Nazwisko };
                    ksiazka.Kategoria = new Kategoria { Nazwa = ksiazka.Kategoria.Nazwa };
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(ksiazka.ToString());

                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "ksiazki_select")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var ksiazki = baza.Ksiazki.ToList();
                    foreach (var ksiazka in ksiazki)
                    {
                        ksiazka.Autor = new Autor { Imie = ksiazka.Autor.Imie, Nazwisko = ksiazka.Autor.Nazwisko };
                        ksiazka.Kategoria = new Kategoria { Nazwa = ksiazka.Kategoria.Nazwa };
                    }
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent((ksiazki.OrderBy(a => a.ID_Ksiazki)).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "ksiazki_update")]
        public HttpResponseMessage Put(Ksiazka value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var ksiazka = baza.Ksiazki.FirstOrDefault(k => k.ID_Ksiazki == value.ID_Ksiazki);
                    ksiazka.ID_Autora = value.ID_Autora;
                    ksiazka.ID_Kategorii = value.ID_Kategorii;
                    ksiazka.Liczba_dostepnych = value.Liczba_dostepnych;
                    ksiazka.Cena_dostawa = value.Cena_dostawa;
                    ksiazka.Cena_sprzedaz = value.Cena_sprzedaz;
                    ksiazka.Tytul = value.Tytul;
                    ksiazka.ISBN = value.ISBN;
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
        [MyAuthorize(Roles = "ksiazki_insert")]
        public HttpResponseMessage Post(Ksiazka value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    value.ID_Autora = 1;
                    value.ID_Kategorii = 1;
                    baza.Ksiazki.Add(value);
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
        [MyAuthorize(Roles = "ksiazki_delete")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Ksiazki.Remove(baza.Ksiazki.FirstOrDefault(k => k.ID_Ksiazki == id));
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