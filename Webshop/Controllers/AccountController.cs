using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshop.Models;
using System.Web.Security;

namespace Webshop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.Login = TempData["login"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(Kund kund)
        {
            //Sätt användaren till inloggad
            FormsAuthentication.SetAuthCookie(kund.AnvandarNamn, true);

            using (TomasosConn conn = new TomasosConn())
            {
                Kund kundin = conn.Kunds.SingleOrDefault(x => x.AnvandarNamn == kund.AnvandarNamn);
                TempData["sucess"] = kundin.AnvandarNamn;
                Session["KundInloggad"] = kundin;

                if (kundin.Losenord == kund.Losenord)
                {
                    //ViewBag.Mess = "Välkommen" + user.Name;
                    return RedirectToAction("Logedin");
                }
                else
                {
                    ViewBag.Message = "Felaktig Inloggning";
                    return View();
                }
            }
        }

        public ActionResult Logedin()
        {

            ViewBag.LoginUsername = TempData["sucess"];
            Session["Varukorg"] = null;

            using (TomasosConn conn = new TomasosConn())
            {
                List<Matratt> list = conn.Matratts.Include("MatrattTyp1").OrderBy(x => x.MatrattTyp).ToList();
                return View(list);
            }
        }

        [Authorize]
        public PartialViewResult AddToChart(int id)
        {
            //ViewBag.LoginUsername = TempData["sucess"];
            List<Matratt> lista = null;

            TomasosConn db = new TomasosConn();
            Matratt matratt = db.Matratts.Include("MatrattTyp1").SingleOrDefault(p => p.MatrattID == id);

            //KOntroll om det är första produkten som läggs till
            if (Session["Varukorg"] == null)
            {
                //Om det inte finns produkter sedan tidigare 
                //skapa en ny lista och lägg till en produkt

                lista = new List<Matratt>();
                lista.Add(matratt);
            }
            else
            {
                //Om det finns produkter sedan tidigare 
                //hämta listan från sessionsvariabeln

                lista = (List<Matratt>)Session["Varukorg"];
                lista.Add(matratt);
            }

            //Lägg till/tillbaka listan i sessionsvariabeln          
            Session["Varukorg"] = lista;
            ViewBag.Total = counttotal();
            ViewBag.Totalkr = counttotal();
            TempData["total"] = counttotal();
            //Skickar tillbaka till PartailView        
            return PartialView("_PartialKopta", matratt);
        }

        public int counttotal() //returnerar summan av maträtterna i varukorgen
        {
            int summa = 0;

            if (Session["Varukorg"] != null)
            {
                List<Matratt> model = (List<Matratt>)Session["Varukorg"];
                summa = model.Sum(c => c.Pris);
                return summa;

            }
            else return summa;
        }

        [Authorize]
        public ActionResult ViewChart()
        {
            //Hämtar lista med produkter från sessionsvariabeln
            List<Matratt> model = (List<Matratt>)Session["Varukorg"];

            //summerar totalpriset på alla maträtter som ligger i varukorgen
            //int total = model.Sum(c => c.Pris);           
            ViewBag.Total = counttotal();
            ViewBag.LoginUsername = TempData["sucess"];

            //TempData["KrTotal"] = counttotal();
            //Skickar produkt listan till vyn där varukorgen visas
            return View(model);
        }

        [Authorize]
        public PartialViewResult DeleteMatratt(int id)
        {
            List<Matratt> model = (List<Matratt>)Session["Varukorg"];

            Matratt deletematratt = model.FirstOrDefault(x => x.MatrattID == id);
            model.Remove(deletematratt);
            Session["Varukorg"] = model;
            ViewBag.Total = counttotal();
            return PartialView("_PartialKöptaDeleted", model);

        }

        public ActionResult Account()
        {
            Kund kund = (Kund)Session["KundInloggad"];
            return View(kund);
        }

        [HttpPost]
        public ActionResult Account(Kund kund)
        {
            //Kund gammalkund = (Kund)Session["KundInloggad"];

            using (TomasosConn conn = new TomasosConn())
            {
                Kund nykund = conn.Kunds.Find(kund.KundID);
                nykund.Namn = kund.Namn;
                nykund.Gatuadress = kund.Gatuadress;
                nykund.Postnr = kund.Postnr;
                nykund.Postort = kund.Postort;
                nykund.Email = kund.Email;
                nykund.Telefon = kund.Telefon;
                nykund.AnvandarNamn = kund.AnvandarNamn;
                nykund.Losenord = kund.Losenord;
                conn.SaveChanges();

                Session["KundInloggad"] = nykund;
            }

            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["KundInloggad"] = null;
            Session["Varukorg"] = null;
            return Redirect("Index");
        }
    }
}