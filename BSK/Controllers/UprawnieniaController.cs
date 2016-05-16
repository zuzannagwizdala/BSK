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
    public class UprawnieniaController : Controller
    {
        // GET: Uprawnienia
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
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var uprawnienie = baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == id);
                    uprawnienie.Uprawnienie_Rola.Clear();
                    
                    odpowiedz.Data = uprawnienie.ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpGet]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var uprawnienia = baza.Uprawnienia.ToList();
                    for (int index = 0; index < uprawnienia.Count; index++)
                    {
                        uprawnienia[index].Uprawnienie_Rola.Clear();
                    }
                    
                    odpowiedz.Data = uprawnienia.OrderBy(a => a.ID_Uprawnienia).ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPut]
        public JsonResult Put(Uprawnienie value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var permission = baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == value.ID_Uprawnienia);
                    permission.Nazwa_tabeli = permission.Nazwa_tabeli;
                    permission.Instrukcja = permission.Instrukcja;
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
        public JsonResult Post(Uprawnienie value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Uprawnienia.Add(value);
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
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Uprawnienia.Remove(baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == id));
                    var rolesPermissions = baza.Uprawnienia_Role.Where(r => r.ID_Uprawnienia == id);
                    baza.Uprawnienia_Role.RemoveRange(rolesPermissions);
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private IEnumerable<Uprawnienie> KonwertujUprawnienia(DbSet<Uprawnienie> uprawnienia)
        {
            var nowe = new List<Uprawnienie>();
            foreach (var uprawnienie in uprawnienia)
            {
                nowe.Add(new Uprawnienie { ID_Uprawnienia = uprawnienie.ID_Uprawnienia, Instrukcja = uprawnienie.Instrukcja, Nazwa_tabeli = uprawnienie.Nazwa_tabeli });
            }
            return nowe;
        }
    }
}