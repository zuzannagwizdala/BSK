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
    public class KsiazkiController : Controller
    {
        // GET: Ksiazki
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

        /*[HttpPost]
        [MyAuthorize(Roles = "ksiazki_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var ksiazka = baza.Ksiazki.FirstOrDefault(k => k.ID_Ksiazki == id);
                    ksiazka.Autor = new Autor { Imie = ksiazka.Autor.Imie, Nazwisko = ksiazka.Autor.Nazwisko };
                    ksiazka.Kategoria = new Kategoria { Nazwa = ksiazka.Kategoria.Nazwa };
                    List<Ksiazka> ksiazki = new List<Ksiazka>();
                    ksiazki.Add(ksiazka);

                    odpowiedz.Data = ksiazki;

                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }*/

        [HttpPost]
        [MyAuthorize(Roles = "ksiazki_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            try
            {
                using (DB baza = new DB())
                {
                    var ksiazki = baza.Ksiazki;
                    odpowiedz.Data = KonwertujKsiazki(ksiazki);
                    /*foreach (var ksiazka in ksiazki)
                    {
                        ksiazka.Autor = new Autor { Imie = ksiazka.Autor.Imie, Nazwisko = ksiazka.Autor.Nazwisko };
                        ksiazka.Kategoria = new Kategoria { Nazwa = ksiazka.Kategoria.Nazwa };
                    }
                    odpowiedz.Data = ksiazki;*/
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "ksiazki_update")]
        public JsonResult Put(Ksiazka value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";

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
                    
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "ksiazki_insert")]
        public JsonResult Post(Ksiazka value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";

            try
            {
                using (DB baza = new DB())
                {
                    value.ID_Autora = 1;
                    value.ID_Kategorii = 1;
                    baza.Ksiazki.Add(value);
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
        [MyAuthorize(Roles = "ksiazki_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";

            try
            {
                using (DB baza = new DB())
                {
                    baza.Ksiazki.Remove(baza.Ksiazki.FirstOrDefault(k => k.ID_Ksiazki == id));
                    baza.SaveChanges();
                    
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private IEnumerable<Ksiazka> KonwertujKsiazki(DbSet<Ksiazka> ksiazki)
        {
            var nowe = new List<Ksiazka>();
            foreach (var ksiazka in ksiazki)
            {
                nowe.Add(new Ksiazka { ID_Ksiazki = ksiazka.ID_Ksiazki, Tytul = ksiazka.Tytul, Liczba_dostepnych = ksiazka.Liczba_dostepnych, Cena_dostawa = ksiazka.Cena_dostawa, Cena_sprzedaz = ksiazka.Cena_sprzedaz, ISBN = ksiazka.ISBN, ID_Autora = ksiazka.ID_Autora, ID_Kategorii = ksiazka.ID_Kategorii });
            }
            return nowe;
        }
    }
}