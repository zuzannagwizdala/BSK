﻿using BSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BSK.Controllers
{
    public class LogInViewController : Controller
    {
        // GET: LogInView
        public ActionResult LogIn()
        {
            return View();
        }

        public HttpResponseMessage Post(string login, string haslo, int? rolaWybrana)
        {
            LogInZapytanie dane = new LogInZapytanie();
            dane.Login = login;
            dane.Haslo = haslo;
            dane.Rola = rolaWybrana;

            HttpResponseMessage odpowiedz = new HttpResponseMessage();
            odpowiedz.StatusCode = HttpStatusCode.OK;                 //CreateResponse(HttpStatusCode.OK);

            using (DB baza = new DB())
            {
                List<Uzytkownik> uzytkownicyWszystko = baza.Uzytkownicy.ToList(); //3
                List<Rola> roleWszystko = baza.Rolee.ToList();
                List<Uprawnienie> uprawnieniaWszystko = baza.Uprawnienia.ToList();
                if (uzytkownicyWszystko.Any(u => u.Login == dane.Login && u.Haslo == dane.Haslo)) // Uzytkownik.sha256(dane.Haslo)))
                {
                    Uzytkownik uzytkownik = uzytkownicyWszystko.First(u => u.Login == dane.Login);
                    IEnumerable<Rola> uzytkownik_role = roleWszystko.Where(r => uzytkownik.Uzytkownik_Rola.Select(ur => ur.ID_Roli).Contains(r.ID_Roli));

                    List<Sesja> sesjeUzytkownika = baza.Sesje.Where(s => s.ID_Uzytkownika == uzytkownik.ID_Uzytkownika).ToList();   // wszystkie sesje uzytkownika
                    if (!dane.Rola.HasValue) //logowanie uzytkownika, bez podania roli (pierwszy raz)
                    {
                        if (sesjeUzytkownika.Count > 0)
                        //jezeli uzytkownik kiedykolwiek mial jakakolwiek sesje
                        {
                            for (int i = 0; i < sesjeUzytkownika.Count; i++)
                            {
                                if (konwertujNaStempel(DateTime.Now) < sesjeUzytkownika[i].Data_waznosci)
                                {
                                    //jezeli jakas sesja nadal trwa, to zwroc tylko role ktora aktualnie (w tej sesji) pelni uzytkownik - na zadna inna nie moze sie zalogowac
                                    Rola rola = uzytkownik_role.FirstOrDefault(ur => ur.ID_Roli == sesjeUzytkownika[i].ID_Roli);
                                    List<Rola> wynik = new List<Rola>();
                                    wynik.Add(new Rola { ID_Roli = rola.ID_Roli, Nazwa = rola.Nazwa });

                                    odpowiedz.StatusCode = HttpStatusCode.OK;
                                    odpowiedz.Content = new StringContent(wynik.ToString());
                                    break;
                                }
                                //sesja byla, ale juz nie trwa (minal okres waznosci) - zwroc wszystkie role danego uzytkownika
                                if (i == sesjeUzytkownika.Count - 1)
                                {
                                    odpowiedz.StatusCode = HttpStatusCode.OK;
                                    odpowiedz.Content = new StringContent(uzytkownik_role.ToString());
                                }
                            }
                        }
                        else
                        //uzytkownik nie mial nigdy zadnej sesji wiec zwroc wszystkie jego role
                        {
                            odpowiedz.StatusCode = HttpStatusCode.OK;
                            odpowiedz.Content = new StringContent(uzytkownik_role.ToString());
                        }
                    }
                    else
                    {
                        Rola tempRola = uzytkownik_role.FirstOrDefault(r => r.ID_Roli == dane.Rola.Value);//8
                        Rola rola = new Rola { ID_Roli = tempRola.ID_Roli, Nazwa = tempRola.Nazwa };
                        IEnumerable<Uprawnienie> uprawnienia_rol = uprawnieniaWszystko.Where(upr => tempRola.Uprawnienie_Rola.Select(uprrola => uprrola.ID_Uprawnienia).Contains(upr.ID_Uprawnienia)); //4
                        List<Uprawnienie> uprawnienia = new List<Uprawnienie>();
                        foreach (Uprawnienie u in uprawnienia_rol)
                        {
                            uprawnienia.Add(new Uprawnienie { ID_Uprawnienia = u.ID_Uprawnienia, Instrukcja = u.Instrukcja, Nazwa_tabeli = u.Nazwa_tabeli }); //5
                        }
                        LogInOdpowiedz zawartoscOdpowiedzi = new LogInOdpowiedz
                        {
                            Nazwa = uzytkownik.Nazwa,
                            ID_Uzytkownika = uzytkownik.ID_Uzytkownika,
                            Rola = rola,
                            Uprawnienia = uprawnienia,
                            Data_waznosci = konwertujNaStempel(DateTime.Now.AddMinutes(10))//6
                        };

                        // mamy juz przypisane uprawnienia do roli wybranej przez uzytkownika, teraz sesja

                        if (sesjeUzytkownika.Count > 0)
                        //jezeli istnieje jakas sesja dla tego uzytkownika (o znanej roli)
                        {
                            for (int i = 0; i < sesjeUzytkownika.Count; i++)
                            {
                                if (konwertujNaStempel(DateTime.Now) < sesjeUzytkownika[i].Data_waznosci)
                                //...i ta sesja jest jeszcze wazna...
                                {
                                    if (sesjeUzytkownika[i].ID_Roli != rola.ID_Roli)
                                    //...i to dla innej roli!!!
                                    {
                                        odpowiedz.StatusCode = HttpStatusCode.Conflict;
                                        odpowiedz.Content = new StringContent("Nie możesz zalogować się na tej roli, ponieważ jesteś już zalogowany na innej.");
                                        return odpowiedz;
                                    }
                                    else
                                    {
                                        zawartoscOdpowiedzi.ID_Sesji = sesjeUzytkownika[i].ID_Sesji;
                                        sesjeUzytkownika[i].Data_waznosci = zawartoscOdpowiedzi.Data_waznosci;
                                        break;
                                    }
                                }
                                //ta sesja jest niewazna, wiec przypisujemy jej nowy id sesji
                                if (i == sesjeUzytkownika.Count - 1)
                                {
                                    zawartoscOdpowiedzi.ID_Sesji = HttpContext.Session.SessionID;
                                    baza.Sesje.Add(new Sesja
                                    {
                                        ID_Roli = rola.ID_Roli,
                                        ID_Sesji = zawartoscOdpowiedzi.ID_Sesji,
                                        ID_Uzytkownika = uzytkownik.ID_Uzytkownika,
                                        Data_waznosci = zawartoscOdpowiedzi.Data_waznosci
                                    });
                                }
                            }

                            odpowiedz.StatusCode = HttpStatusCode.OK;
                            odpowiedz.Content = new StringContent(zawartoscOdpowiedzi.ToString());
                            baza.SaveChanges();

                        }
                        else
                        //uzytkownik nie mial wczesniej sesji
                        {
                            zawartoscOdpowiedzi.ID_Sesji = HttpContext.Session.SessionID;              //.Current.Session.SessionID;
                            baza.Sesje.Add(new Sesja
                            {
                                ID_Roli = rola.ID_Roli,
                                ID_Sesji = zawartoscOdpowiedzi.ID_Sesji,
                                ID_Uzytkownika = uzytkownik.ID_Uzytkownika,
                                Data_waznosci = zawartoscOdpowiedzi.Data_waznosci
                            });
                        }
                        odpowiedz.StatusCode = HttpStatusCode.OK;
                        odpowiedz.Content = new StringContent(zawartoscOdpowiedzi.ToString());
                        baza.SaveChanges();

                    }
                }
                else
                // to jest else do pierwszego ifa sprawdzajacego uzytkownika i haslo! (jeszcze z rola ustawiona na null)
                {
                    odpowiedz.StatusCode = HttpStatusCode.Conflict;
                    odpowiedz.Content = new StringContent("Niepoprawne dane!");
                }
            }
            return odpowiedz;

        }

        private static readonly DateTime znak = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long konwertujNaStempel(DateTime teraz)
        {
            TimeSpan stempel = teraz - znak;
            return stempel.Ticks;
        }
    }
}