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
    public class RoleController : Controller
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "role_select")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

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

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(role.ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }
        [MyAuthorize(Roles = "role_select")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

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

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(roles.OrderBy(a => a.ID_Roli).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "role_update")]
        public HttpResponseMessage Put(Rola value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

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
        [MyAuthorize(Roles = "role_insert")]
        public HttpResponseMessage Post(Rola value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

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
        [MyAuthorize(Roles = "role_delete")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Rolee.Remove(baza.Rolee.FirstOrDefault(k => k.ID_Roli == id));
                    var rolesPermissions = baza.Uprawnienia_Role.Where(r => r.ID_Roli == id);
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