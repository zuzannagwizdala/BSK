﻿using BSK.Models;
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

                    var posortowane = autorzy.OrderBy(a => a.ID_Autora);
                    odpowiedz.Data = KonwertujAutorzy(posortowane);

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
                    if (value.Imie != null)
                    {
                        autor.Imie = value.Imie;
                    }
                    if (value.Nazwisko != null)
                    {
                        autor.Nazwisko = value.Nazwisko;
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
        [MyAuthorize(Roles = "autorzy_insert")]
        public JsonResult Post(Autor value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                if (value.Imie == null || value.Nazwisko == null)
                {
                    odpowiedz.Data = "Uzupełnij wszystkie pola aby dodać autora!";
                    return odpowiedz;
                }
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
        private List<Autor> KonwertujAutorzy(IEnumerable<Autor> autorzy)
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