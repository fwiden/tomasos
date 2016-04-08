using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webshop.Models;
using System.Web.Security;
using Webshop.ViewModels;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Menu()
        {
            ViewBag.Message = "Your application Menu page.";

            using (TomasosConn conn = new TomasosConn())
            {
                List<Matratt> list = conn.Matratts.Include("MatrattTyp1").OrderBy(x=>x.MatrattTyp).ToList();
                return View(list);
            }
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your application Contact page.";
            return View();
        }

        public ActionResult Signup()
        {
            ViewBag.SignupMessage = "Your application Signup page.";
            return View();
        }


        [HttpPost]
        public ActionResult Signup(Kund kund)
        {            
            //Sparar ned den nya kunden i databasen
            using (TomasosConn conn = new TomasosConn())
            {
                conn.Kunds.Add(kund);
                conn.SaveChanges();
            }

            //TempData["login"] = kund.AnvandarNamn;
            TempData["lll"] = kund.AnvandarNamn;

            return RedirectToAction("Login", "Home");
        }

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
            FormsAuthentication.SetAuthCookie(kund.AnvandarNamn, false);


            using (TomasosConn conn = new TomasosConn())
            {
                Kund kundin = conn.Kunds.SingleOrDefault(x => x.AnvandarNamn == kund.AnvandarNamn);
                TempData["sucess"] = kundin.AnvandarNamn;
                Session["KundInloggad"] = kundin;
                
                if (kundin.Losenord == kund.Losenord)
                {
                    TempData["AnvändareID"] = kundin.KundID;
                    return RedirectToAction("ViewProductsShopping");
                }
                else
                {
                    ViewBag.Message = "Felaktig Inloggning";
                    return View();
                }
            }
        }

        [Authorize]
        public ActionResult Logedin()
        {
            
            ViewBag.LoginUsername = TempData["sucess"];
            Session["Varukorg"] = null;

            using (TomasosConn conn = new TomasosConn())
            {
                List<Matratt> list = conn.Matratts.Include("MatrattTyp1").OrderBy(x=>x.MatrattTyp).ToList();
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

        
        public ActionResult ViewChart()
        {                 
            //Hämtar lista med produkter från sessionsvariabeln
            List<Matratt> model = (List<Matratt>)Session["Varukorg"];

            if (model == null)
            {
                ViewBag.Error = "Välj en produkt innan du går till kassan!";               

            }
            else            
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
            List<Matratt> model= (List<Matratt>)Session["Varukorg"];

            Matratt deletematratt = model.FirstOrDefault(x => x.MatrattID == id);
            model.Remove(deletematratt);
            Session["Varukorg"] = model;
            ViewBag.Total = counttotal();
            return PartialView("_PartialKöptaDeleted",model);                  
            
        }

        public ActionResult Account()
        {
            //if (User.Identity.IsAuthenticated)
            
                Kund kund = (Kund)Session["KundInloggad"];
                return View(kund);                  
        }

        [HttpPost]
        public ActionResult Account(Kund kund)
        {
            //Kund gammalkund = (Kund)Session["KundInloggad"];

            using (TomasosConn conn =new TomasosConn())
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

        // Fyller vyn med alla maträtter som kan beställas
        public ActionResult ViewProductsShopping()
        {
            TomasosConn db = new TomasosConn();

            //Skapar vyn kopplade modell
            ViewList model = new ViewList();

            //Lägger till en lista med produkter till modellen
            model.AllProducts = db.Matratts.ToList();

            //KOntroll om det finns något i varukorgen
            if (Session["Varukorg"] == null)
            {
                model.SelectedProducts = null;
            }
            else
            {
                //Finns det produkter i varukorgen skall dessa skickas med i
                //en lista (SelectedProducts)
                model.SelectedProducts = (List<Matratt>)Session["Varukorg"];
                model.TotalPrice = (decimal)model.SelectedProducts.Sum(s => s.Pris);
            }
            return View(model);
        }

        //Lägger till en ny produkt i varukorgen
        //id är den produkts som valts i vyns id
        public ActionResult AddToChart2(int id)
        {
            List<Matratt> selProducts = null;
            TomasosConn db = new TomasosConn();

            //Hämtar den produkt som valts
            Matratt product = db.Matratts.SingleOrDefault(p => p.MatrattID == id);

            //Är det första produkten som läggs i korgen
            if (Session["Varukorg"] == null)
            {
                selProducts = new List<Matratt>();
            }
            else
            {
                //Är det inte första produkten, hämta befintlig varukorg
                selProducts = (List<Matratt>)Session["Varukorg"];
            }

            //Lägg in produkten i listan och lägg listan i
            //sessionsvariabeln
            selProducts.Add(product);
            Session["Varukorg"] = selProducts;

            return RedirectToAction("ViewProductsShopping");
        }

        [Authorize]
        public ActionResult DeleteMatratt2(int id)
        {
            List<Matratt> model = (List<Matratt>)Session["Varukorg"];

            Matratt deletematratt = model.FirstOrDefault(x => x.MatrattID == id);
            model.Remove(deletematratt);
            Session["Varukorg"] = model;
            //ViewBag.Total = counttotal();
            return RedirectToAction("ViewProductsShopping", model);

        }

        public ActionResult Details(int id)
        {
            using (TomasosConn conn = new TomasosConn())
            {
                Matratt matratt = conn.Matratts.SingleOrDefault(c => c.MatrattID == id);
                return PartialView("_PartialDetails", matratt);
            }
        }

        public ActionResult PlacOrder()  //Hit kommer man om man lägger en order
        {
            List<Matratt> model = (List<Matratt>)Session["Varukorg"];
            int antalmatratter = model.Select(i => i.MatrattID).Distinct().Count();   //antal olika maträtter i varukorgen     
                
            var kund = Session["KundInloggad"] as Kund;
            TempData["Kundnamn"] = "Skickas till:"+kund.Namn;
            TempData["kundadress"] = "Adress:" +kund.Gatuadress;
            TempData["Kundpostnummer"] ="Postnummer:"+ kund.Postnr;
            TempData["Kundpostort"] ="Postort:"+ kund.Postort;

            int userId = kund.KundID;
            DateTime date = DateTime.Now;
            int totalbelopp = counttotal();         
            
            Bestallning bestallning = new Bestallning();

            bestallning.BestallningDatum = date; //funkar!
            bestallning.Totalbelopp = totalbelopp;//funkar!
            bestallning.Levererad = false;        //funkar!?
            bestallning.KundID = userId;         //funkar!

            using (TomasosConn conn = new TomasosConn())
            {
                conn.Bestallnings.Add(bestallning);
                conn.SaveChanges();
                int id = bestallning.BestallningID;

                foreach (Matratt item in model)
                {
                    BestallningMatratt matratt = new BestallningMatratt();

                    matratt.MatrattID = item.MatrattID;
                    matratt.BestallningID = id;
                    matratt.Antal = 1;
                    conn.BestallningMatratts.Add(matratt);
                    conn.SaveChanges();
                }
            }                    

            //var result = model.GroupBy(i => i.MatrattID)
            //   .Select(x => new { DishID = x.Key, BestID = , Count = x.Count() })
            //   .ToList();

            //List<Matratt> lista = result.Cast<Matratt>().ToList();      
            
            //foreach (Matratt matrattena in model)
            //{
            //    BestallningMatratt bestmatratt = new BestallningMatratt();

            //    bestmatratt.BestallningID = bestallningdid;
            //    bestmatratt.MatrattID = matrattena.MatrattID;
            //    bestmatratt.Antal = 1;

            //    conn.BestallningMatratts.Add(bestmatratt);
            //    conn.SaveChanges();
            //}

            TempData["fff"] = "Tack för beställningen!";
            //Session["Varukorg"] = null;   
            return RedirectToAction("ViewChart");
        }

        public ActionResult Order()
        {
            Kund kundid = (Kund)Session["KundInloggad"];            

            if (kundid == null)
            {
                return RedirectToAction("Login");
            }

            using (TomasosConn conn = new TomasosConn())
            {
                List<Bestallning> order = conn.Bestallnings.Include("Kund").Where(i => i.KundID == kundid.KundID).OrderByDescending(i=>i.BestallningDatum).ToList();
                return View(order);
            }           

        }


    }
}