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
    public class UzytkownicyController : Controller
    {
        // GET: Uzytkownicy
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
        [MyAuthorize(Roles = "uzytkownicy_select")]
        public JsonResult GetUser(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";

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
                    
                    odpowiedz.Data = uzytkownik;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }*/

        [HttpPost]
        [MyAuthorize(Roles = "uzytkownicy_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
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
                    odpowiedz.Data = uzytkownicy.OrderBy(a => a.ID_Uzytkownika);
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "uzytkownicy_update")]
        public JsonResult Put(int id, string login, string nazwa, string stareHaslo, string noweHaslo, int[] roleDodanie, int[] roleUsuwanie)    //(Uzytkownik value)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var user = baza.Uzytkownicy.FirstOrDefault(k => k.ID_Uzytkownika == id);
                    if (login != "")
                    {
                        user.Login = login;
                    }
                    if (nazwa != "")
                    {
                        user.Nazwa = nazwa;
                    }

                    if (noweHaslo != "" || stareHaslo != "")
                    {
                        if (noweHaslo != "" && stareHaslo != "")
                        {
                            if (user.Haslo == Models.Uzytkownik.sha256(stareHaslo))
                            {
                                user.Haslo = Models.Uzytkownik.sha256(noweHaslo);
                            }
                            else
                            {
                                odpowiedz.Data = "Podano niepoprawne hasło, zmiana nie powiodła się!";
                            }
                        }
                        else
                        {
                            odpowiedz.Data = "Oba hasła muszą być podane aby dokonać zmiany!";
                        }
                    }
                    int idRoli = 0;
                    if (roleDodanie != null)
                    {
                        var urToAdd = new List<Uzytkownik_Rola>();

                        for (int i = 0; i < roleDodanie.Length; i++)
                        {
                            idRoli = roleDodanie[i];
                            var rola = baza.Rolee.FirstOrDefault(p => p.ID_Roli == idRoli);
                            urToAdd.Add(new Uzytkownik_Rola { Uzytkownik = user, ID_Uzytkownika = user.ID_Uzytkownika, Rola = rola, ID_Roli = rola.ID_Roli });

                        }
                        baza.Uzytkownicy_Role.AddRange(urToAdd);

                    }

                    if (roleUsuwanie != null)
                    {
                        var urToAdd = new List<Uzytkownik_Rola>();
                        for (int i = 0; i < roleUsuwanie.Length; i++)
                        {
                            idRoli = roleUsuwanie[i];
                            var rola = baza.Rolee.FirstOrDefault(p => p.ID_Roli == idRoli);
                            urToAdd.Add(new Uzytkownik_Rola { Uzytkownik = user, ID_Uzytkownika = user.ID_Uzytkownika, Rola = rola, ID_Roli = rola.ID_Roli });

                        }
                        int idUzytkownika = 0;
                        for (int i = 0; i < urToAdd.Count(); i++)
                        {
                            idRoli = urToAdd[i].ID_Roli;
                            idUzytkownika = urToAdd[i].ID_Uzytkownika;
                            baza.Uzytkownicy_Role.Remove(baza.Uzytkownicy_Role.FirstOrDefault(u => u.ID_Roli == idRoli && u.ID_Uzytkownika == idUzytkownika));
                        }

                    }
                    baza.SaveChanges();

                    /*if (value.Haslo != null)
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

                    baza.Uzytkownicy_Role.AddRange(urToAdd);*/
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
        [MyAuthorize(Roles = "uzytkownicy_insert")]
        public JsonResult Post(Uzytkownik value, int[] wartosci_int)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                if (value.Login == null || value.Nazwa == null || value.Haslo == null)
                {
                    odpowiedz.Data = "Uzupełnij wszystkie pola aby dodać użytkownika!";
                    return odpowiedz;
                }
                using (DB baza = new DB())
                {
                    var wszyscy = baza.Uzytkownicy.ToList();

                    foreach (var uz in wszyscy)
                    {
                        if (uz.Login == value.Login)
                        {
                            odpowiedz.Data = "Użytkownik o takim loginie już istnieje w bazie!";
                            return odpowiedz;
                        }
                    }
                    value.Haslo = Models.Uzytkownik.sha256(value.Haslo);
                    var user = baza.Uzytkownicy.Add(value);

                    int indeks = 0;

                    var urToAdd = new List<Uzytkownik_Rola>();
                    if (wartosci_int != null)
                    {
                        
                        for (int i = 0; i < wartosci_int.Length; i++)
                        {
                            indeks = wartosci_int[i];
                            var role = baza.Rolee.FirstOrDefault(p => p.ID_Roli == indeks);
                            urToAdd.Add(new Uzytkownik_Rola { Uzytkownik = user, ID_Uzytkownika = user.ID_Uzytkownika, Rola = role, ID_Roli = role.ID_Roli });
                        }
                        
                    }
                    baza.Uzytkownicy_Role.AddRange(urToAdd);
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
        [MyAuthorize(Roles = "uzytkownicy_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var userRoles = baza.Uzytkownicy_Role.Where(r => r.ID_Uzytkownika == id);
                    var sesje = baza.Sesje.Where(s => s.ID_Uzytkownika == id);
                    baza.Uzytkownicy_Role.RemoveRange(userRoles);
                    baza.Sesje.RemoveRange(sesje);

                    baza.Uzytkownicy.Remove(baza.Uzytkownicy.FirstOrDefault(k => k.ID_Uzytkownika == id));
                    
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private IEnumerable<Uzytkownik> KonwertujUzytkownikow(DbSet<Uzytkownik> uzytkownicy)
        {
            var nowe = new List<Uzytkownik>();
            foreach (var uzytkownik in uzytkownicy)
            {
                nowe.Add(new Uzytkownik { ID_Uzytkownika = uzytkownik.ID_Uzytkownika, Login = uzytkownik.Login, Haslo = uzytkownik.Haslo });
            }
            return nowe;
        }
    }
}