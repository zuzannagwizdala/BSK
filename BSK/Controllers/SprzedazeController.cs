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
    public class SprzedazeController : Controller
    {
        // GET: Sprzedaze
        public ActionResult Index()
        {
            return View();
        }

        [MyAuthorize(Roles = "sprzedaze_select")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaz = baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == id);

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(sprzedaz.ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
                //= Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.InnerException);
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "sprzedaze_select")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaze = baza.Sprzedaze.ToList();

                    odpowiedz.StatusCode = HttpStatusCode.OK;
                    odpowiedz.Content = new StringContent(sprzedaze.OrderBy(a => a.ID_Sprzedazy).ToString());
                }
            }
            catch (Exception ex)
            {
                odpowiedz.StatusCode = HttpStatusCode.BadRequest;
                odpowiedz.Content = new StringContent(ex.InnerException.ToString());
            }
            return odpowiedz;
        }

        [MyAuthorize(Roles = "sprzedaze_update")]
        public HttpResponseMessage Put(Sprzedaz value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    var sprzedaz = baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == value.ID_Sprzedazy);
                    sprzedaz.Data_sprzedazy = value.Data_sprzedazy;
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
        [MyAuthorize(Roles = "sprzedaze_insert")]
        public HttpResponseMessage Post(Sprzedaz value)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Sprzedaze.Add(value);
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
        [MyAuthorize(Roles = "sprzedaze_delete")]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage odpowiedz = new HttpResponseMessage();

            try
            {
                using (DB baza = new DB())
                {
                    baza.Sprzedaze.Remove(baza.Sprzedaze.FirstOrDefault(k => k.ID_Sprzedazy == id));
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