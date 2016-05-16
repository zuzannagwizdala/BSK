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
    public class RoleController : Controller
    {
        // GET: Role
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
        [MyAuthorize(Roles = "role_select")]
        public JsonResult Get(int id)
        {
            JsonResult odpowiedz = new JsonResult();

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
                    
                    odpowiedz.Data = role.ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpGet]
        [MyAuthorize(Roles = "role_select")]
        public JsonResult Get()
        {
            JsonResult odpowiedz = new JsonResult();

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
                    
                    odpowiedz.Data = roles.OrderBy(a => a.ID_Roli).ToString();
                }
            }
            catch (Exception ex)
            {
                odpowiedz.Data = ex.InnerException.ToString();
            }
            return odpowiedz;
        }

        [HttpPut]
        [MyAuthorize(Roles = "role_update")]
        public JsonResult Put(Rola value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var role = baza.Rolee.FirstOrDefault(k => k.ID_Roli == value.ID_Roli);
                    role.Nazwa = value.Nazwa;

                    var roleIds = value.Uprawnienie_Rola.Select(roPe => new { Id = roPe.Uprawnienie.ID_Uprawnienia });

                    var rolesPermissions = baza.Uprawnienia_Role.Where(rp => rp.ID_Roli == role.ID_Roli).ToList();
                    var rpToRemove = new List<Uprawnienie_Rola>();
                    foreach (var rolesPermission in rolesPermissions)
                    {
                        if (roleIds.All(r => r.Id != rolesPermission.ID_Uprawnienia))
                        {
                            rpToRemove.Add(rolesPermission);
                        }
                    }
                    baza.Uprawnienia_Role.RemoveRange(rpToRemove);
                    var rpToAdd = new List<Uprawnienie_Rola>();
                    foreach (var rolesPermission in value.Uprawnienie_Rola)
                    {
                        if (rolesPermissions.All(r => r.ID_Uprawnienia != rolesPermission.Uprawnienie.ID_Uprawnienia))
                        {
                            var permission =
                                baza.Uprawnienia.FirstOrDefault(p => p.ID_Uprawnienia == rolesPermission.Uprawnienie.ID_Uprawnienia);
                            rpToAdd.Add(new Uprawnienie_Rola { Uprawnienie = permission, ID_Uprawnienia = permission.ID_Uprawnienia, Rola = role, ID_Roli = role.ID_Roli });
                        }
                    }
                    baza.Uprawnienia_Role.AddRange(rpToAdd);
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
        public JsonResult Post(Rola value)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    var rolesPermissions = value.Uprawnienie_Rola.GetRange(0, value.Uprawnienie_Rola.Count);
                    value.Uprawnienie_Rola.Clear();
                    var role = baza.Rolee.Add(value);
                    var rpToAdd = new List<Uprawnienie_Rola>();
                    foreach (var rolesPermission in rolesPermissions)
                    {
                        var permission = baza.Uprawnienia.FirstOrDefault(p => p.ID_Uprawnienia == rolesPermission.Uprawnienie.ID_Uprawnienia);
                        rpToAdd.Add(new Uprawnienie_Rola { Rola = role, ID_Roli = role.ID_Roli, Uprawnienie = permission, ID_Uprawnienia = permission.ID_Uprawnienia });
                    }
                    baza.Uprawnienia_Role.AddRange(rpToAdd);
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
        [MyAuthorize(Roles = "role_delete")]
        public JsonResult Delete(int id)
        {
            JsonResult odpowiedz = new JsonResult();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Rolee.Remove(baza.Rolee.FirstOrDefault(k => k.ID_Roli == id));
                    var rolesPermissions = baza.Uprawnienia_Role.Where(r => r.ID_Roli == id);
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
        private IEnumerable<Rola> KonwertujRole(DbSet<Rola> role)
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