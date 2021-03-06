﻿using BSK.Models;
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
    public class RoleController : Controller
    {
        // GET: Role
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
        [MyAuthorize(Roles = "role_select")]
        public JsonResult GetRola(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var role = baza.Rolee.FirstOrDefault(k => k.ID_Roli == id);
                    var permissions = baza.Uprawnienia.ToList();
                    var rolesPermissions = new List<Uprawnienie_Rola>();
                    foreach (var rolesPermission in role.Uprawnienie_Rola)
                    {
                        var permission = permissions.FirstOrDefault(p => p.ID_Uprawnienia == rolesPermission.ID_Uprawnienia);
                        rolesPermissions.Add(new Uprawnienie_Rola { Uprawnienie = new Uprawnienie { ID_Uprawnienia = permission.ID_Uprawnienia, Nazwa_tabeli = permission.Nazwa_tabeli, Instrukcja = permission.Instrukcja } });
                    }
                    role.Uprawnienie_Rola = rolesPermissions;
                    role.Uprawnienie_Rola.Clear();
                    
                    odpowiedz.Data = role;
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "role_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var roles = baza.Rolee.ToList();
                    var permissions = baza.Uprawnienia.ToList();
                    foreach (var role in roles)
                    {
                        var rolesPermissions = new List<Uprawnienie_Rola>();
                        foreach (var rolesPermission in role.Uprawnienie_Rola)
                        {
                            var permission = permissions.FirstOrDefault(p => p.ID_Uprawnienia == rolesPermission.ID_Uprawnienia);
                            rolesPermissions.Add(new Uprawnienie_Rola { Uprawnienie = new Uprawnienie { ID_Uprawnienia = permission.ID_Uprawnienia, Nazwa_tabeli = permission.Nazwa_tabeli, Instrukcja = permission.Instrukcja } });
                        }
                        role.Uprawnienie_Rola = rolesPermissions;
                        role.Uzytkownik_Rola.Clear();
                    }
                    odpowiedz.Data = roles.OrderBy(a => a.ID_Roli);
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPost]
        [MyAuthorize(Roles = "role_update")]
        public JsonResult Put(string nazwa, int id, int[] uprawnieniaDodanie, int[] uprawnieniaUsuwanie)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            //int id_roli = int.Parse(id);
            try
            {
                using (DB baza = new DB())
                {
                    var nowa = baza.Rolee.FirstOrDefault(k => k.ID_Roli == id);
                    nowa.ID_Roli = id;
                    if (nazwa != "")
                    {
                        nowa.Nazwa = nazwa;
                    }
                    int idUprawnienia = 0;
                    if (uprawnieniaDodanie != null)
                    {
                        var urToAdd = new List<Uprawnienie_Rola>();
                        
                        for (int i = 0; i < uprawnieniaDodanie.Length; i++)
                        {
                            idUprawnienia = uprawnieniaDodanie[i];
                            var uprawnienie = baza.Uprawnienia.FirstOrDefault(p => p.ID_Uprawnienia == idUprawnienia);
                            urToAdd.Add(new Uprawnienie_Rola { Rola = nowa, ID_Roli = nowa.ID_Roli, Uprawnienie = uprawnienie, ID_Uprawnienia = uprawnienie.ID_Uprawnienia });

                        }
                        baza.Uprawnienia_Role.AddRange(urToAdd);

                    }

                    if (uprawnieniaUsuwanie != null)
                    {
                        var urToAdd = new List<Uprawnienie_Rola>();
                        for (int i = 0; i < uprawnieniaUsuwanie.Length; i++)
                        {
                            idUprawnienia = uprawnieniaUsuwanie[i];
                            var uprawnienie = baza.Uprawnienia.FirstOrDefault(p => p.ID_Uprawnienia == idUprawnienia);
                            urToAdd.Add(new Uprawnienie_Rola { Rola = nowa, ID_Roli = nowa.ID_Roli, Uprawnienie = uprawnienie, ID_Uprawnienia = uprawnienie.ID_Uprawnienia });

                        }
                        int idRoli = 0;
                        int idUpr = 0;
                        for (int i = 0; i < urToAdd.Count(); i++)
                        {
                            idRoli = urToAdd[i].ID_Roli;
                            idUpr = urToAdd[i].ID_Uprawnienia;
                            baza.Uprawnienia_Role.Remove(baza.Uprawnienia_Role.FirstOrDefault(u => u.ID_Roli == idRoli && u.ID_Uprawnienia == idUpr));
                        }
                        
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
        [MyAuthorize(Roles = "role_insert")]
        public JsonResult Post(string uprawnienia)
        {
            var nazwa_upr = uprawnienia.Split(';');
            var nazwa = nazwa_upr[0];
            var upr = nazwa_upr[1].Split('-');
            upr = upr.Take(upr.Count() - 1).ToArray();
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                if(nazwa == "")
                {
                    odpowiedz.Data = "Podaj nazwę aby dodać rolę!";
                    return odpowiedz;
                }
                using (DB baza = new DB())
                {
                    var wszystkie = baza.Rolee.ToList();

                    foreach (var rol in wszystkie)
                    {
                        if (rol.Nazwa == nazwa)
                        {
                            odpowiedz.Data = "Rola o takiej nazwie już istnieje w bazie!";
                            return odpowiedz;
                        }
                    }
                    Rola value = new Rola();
                    var rola = baza.Rolee.Add(value);
                    value.Nazwa = nazwa;
                    var urToAdd = new List<Uprawnienie_Rola>();
                    var id_upr = 0;
                    for (int i = 0; i < upr.Length; i++)
                    {
                        id_upr = int.Parse(upr[i]);
                        var uprawnienie = baza.Uprawnienia.FirstOrDefault(p => p.ID_Uprawnienia == id_upr);
                        urToAdd.Add(new Uprawnienie_Rola { Rola = rola, ID_Roli = rola.ID_Roli, Uprawnienie = uprawnienie, ID_Uprawnienia = uprawnienie.ID_Uprawnienia });

                    }
                    baza.Uprawnienia_Role.AddRange(urToAdd);
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
        [MyAuthorize(Roles = "role_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();
            odpowiedz.Data = " ";
            try
            {
                using (DB baza = new DB())
                {
                    var rolesPermissions = baza.Uprawnienia_Role.Where(r => r.ID_Roli == id);
                    baza.Uprawnienia_Role.RemoveRange(rolesPermissions);
                    var rolesUsers = baza.Uzytkownicy_Role.Where(r => r.ID_Roli == id);
                    baza.Uzytkownicy_Role.RemoveRange(rolesUsers);
                    baza.Rolee.Remove(baza.Rolee.FirstOrDefault(k => k.ID_Roli == id));
                    baza.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }
        private List<Rola> KonwertujRole(IEnumerable<Rola> role)
        {
            var nowe = new List<Rola>();
            foreach (var rola in role)
            {
                nowe.Add(new Rola { ID_Roli = rola.ID_Roli, Nazwa = rola.Nazwa });		//tutaj jeszcze powinny byc tabele wiele do wiele
            }
            return nowe;
        }
    }
}