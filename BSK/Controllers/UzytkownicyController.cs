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
    public class UzytkownicyController : Controller
    {
        // GET: Uzytkownicy
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "uzytkownicy_select")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var uzytkownik = baza.Uzytkownicy.FirstOrDefault(k => k.ID_Uzytkownika == id);
                    uzytkownik.Haslo = null;
                    foreach (var item in uzytkownik.Uzytkownik_Rola)
                    {
                        item.Uzytkownik = null;
                        var role = baza.Rolee.FirstOrDefault(r => r.ID_Roli == item.ID_Roli);
                        item.Rola = new Rola { ID_Roli = role.ID_Roli, Nazwa = role.Nazwa };
                    }
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(uzytkownik.ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "uzytkownicy_select")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var uzytkownicy = baza.Uzytkownicy.ToList();
                    foreach (var uzytkownik in uzytkownicy)
                    {
                        foreach (var item in uzytkownik.Uzytkownik_Rola)
                        {
                            item.Uzytkownik = null;
                            var rola = baza.Rolee.FirstOrDefault(r => r.ID_Roli == item.ID_Roli);
                            item.Rola = new Rola { ID_Roli = rola.ID_Roli, Nazwa = rola.Nazwa };
                        }
                    }
                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(uzytkownicy.OrderBy(a => a.ID_Uzytkownika).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "uzytkownicy_update")]
        public HttpResponseMessage Put(Uzytkownik value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var user = baza.Uzytkownicy.FirstOrDefault(k => k.ID_Uzytkownika == value.ID_Uzytkownika);
                    user.Login = value.Login;
                    user.Nazwa = value.Nazwa;
                    if (value.Haslo != null)
                    {
                        user.Haslo = Models.Uzytkownik.sha256(value.Haslo);
                    }

                    var roleIds = value.Uzytkownik_Rola.Select(roPe => new { Id = roPe.Rola.ID_Roli });

                    var userRoles = baza.Uzytkownicy_Role.Where(ur => ur.ID_Uzytkownika == user.ID_Uzytkownika).ToList();
                    var urToRemove = new List<Uzytkownik_Rola>();
                    foreach (var userRole in userRoles)
                    {
                        if (roleIds.All(r => r.Id != userRole.ID_Roli))
                        {
                            urToRemove.Add(userRole);
                        }
                    }
                    baza.Uzytkownicy_Role.RemoveRange(urToRemove);
                    var urToAdd = new List<Uzytkownik_Rola>();
                    foreach (var userRole in value.Uzytkownik_Rola)
                    {
                        if (userRoles.All(r => r.ID_Roli != userRole.Rola.ID_Roli))
                        {
                            var role = baza.Rolee.FirstOrDefault(p => p.ID_Roli == userRole.Rola.ID_Roli);
                            urToAdd.Add(new Uzytkownik_Rola { Uzytkownik = user, ID_Uzytkownika = user.ID_Uzytkownika, Rola = role, ID_Roli = role.ID_Roli });
                        }
                    }
                    baza.Uzytkownicy_Role.AddRange(urToAdd);
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
        [MyAuthorize(Roles = "uzytkownicy_insert")]
        public HttpResponseMessage Post(Uzytkownik value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var userRoles = value.Uzytkownik_Rola.GetRange(0, value.Uzytkownik_Rola.Count);
                    value.Uzytkownik_Rola.Clear();
                    value.Haslo = Models.Uzytkownik.sha256(value.Haslo);
                    var user = baza.Uzytkownicy.Add(value);

                    var urToAdd = new List<Uzytkownik_Rola>();
                    foreach (var userRole in userRoles)
                    {
                        var role = baza.Rolee.FirstOrDefault(p => p.ID_Roli == userRole.Rola.ID_Roli);
                        urToAdd.Add(new Uzytkownik_Rola { Uzytkownik = user, ID_Uzytkownika = user.ID_Uzytkownika, Rola = role, ID_Roli = role.ID_Roli });
                    }
                    baza.Uzytkownicy_Role.AddRange(urToAdd);

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
        [MyAuthorize(Roles = "uzytkownicy_delete")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Uzytkownicy.Remove(baza.Uzytkownicy.FirstOrDefault(k => k.ID_Uzytkownika == id));
                    var userRoles = baza.Uzytkownicy_Role.Where(r => r.ID_Uzytkownika == id);
                    baza.Uzytkownicy_Role.RemoveRange(userRoles);
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