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
    public class UprawnieniaController : Controller
    {
        // GET: Uprawnienia
        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var uprawnienie = baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == id);
                    uprawnienie.Uprawnienie_Rola.Clear();

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(uprawnienie.ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var uprawnienia = baza.Uprawnienia.ToList();
                    for (int index = 0; index < uprawnienia.Count; index++)
                    {
                        uprawnienia[index].Uprawnienie_Rola.Clear();
                    }

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(uprawnienia.OrderBy(a => a.ID_Uprawnienia).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        public HttpResponseMessage Put(Uprawnienie value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var permission = baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == value.ID_Uprawnienia);
                    permission.Nazwa_tabeli = permission.Nazwa_tabeli;
                    permission.Instrukcja = permission.Instrukcja;
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
        public HttpResponseMessage Post(Uprawnienie value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Uprawnienia.Add(value);
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
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Uprawnienia.Remove(baza.Uprawnienia.FirstOrDefault(k => k.ID_Uprawnienia == id));
                    var rolesPermissions = baza.Uprawnienia_Role.Where(r => r.ID_Uprawnienia == id);
                    baza.Uprawnienia_Role.RemoveRange(rolesPermissions);
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