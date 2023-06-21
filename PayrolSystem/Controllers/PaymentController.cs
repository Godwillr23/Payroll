using PayrolSystem.Models.DatabaseFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PayrolSystem.Controllers
{
    public class PaymentController : Controller
    {
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        // GET: Payment
        public ActionResult PaymentHistory()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var payhistory = _context.PaymentRecords.ToList();
            return View(payhistory);
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {
            var user = from p in _context.PaymentRecords select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                user = user.Where(s => s.PaymentType.Contains(q) || s.PaymentStatus.Contains(q) || s.PaymentDate.Contains(q));
            }
            else
            {
                user = from p in _context.PaymentRecords select p;
            }

            return View(user);
        }

        public ActionResult AddPayment()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Companies = GetCompanies();
            Session["PaymentSuccess"] = null;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddPayment(PaymentRecord model)
        {
            string userId = Session["UserId"].ToString();
            int userid = Convert.ToInt32(userId);
            //ViewBag.Companies = GetCompanies();

            if (ModelState.IsValid)
            {
                //model.CompanyID = userid;
                model.PaymentDate = DateTime.Now.Date.ToShortDateString();
                model.PaymentTime = DateTime.Now.ToString("HH:mm:ss tt");
                model.PaymentStatus = "Paid";
                _context.PaymentRecords.Add(model);

                try
                {
                    UpdatePayment(model.CompanyID);
                    _context.SaveChanges();
                    
                    Session["PaymentSuccess"] = "Payment Record Added!";
                    return RedirectToAction("PaymentHistory", "Payment");

                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }

            }
            else
            {
                ModelState.AddModelError("Error", "Invalid Request");
                return View();
            }
        }
        private static List<SelectListItem> GetCompanies()
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> project = dbContext.CompanyDetails.OrderBy(x => x.CompanyName).Select(c => new SelectListItem
            {
                Value = c.CompanyID.ToString(),
                Text = c.CompanyName

            }).ToList();

            return project;
        }

        public void UpdatePayment(int companyID)
        {
            var model = _context.CompanyDetails.FirstOrDefault(s => s.CompanyID == companyID);
            if (model == null)
            {
                // user password did not match the DB password
                throw new Exception("Invalid password exception");
            }

            // update the record
            model.PaymentStatus = "Paid";
            model.ActiveStatus = "True";

            // save the Database changes
            _context.SaveChanges();
        }
    }
}