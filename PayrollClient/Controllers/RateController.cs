using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using PayrolSystem.Models.DatabaseFirst;

namespace PayrollClient.Controllers
{
    public class RateController : Controller
    {
        // GET: Rate
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            var clients = _context.RateTables.ToList();
            return View(clients);
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {

            var user = from p in _context.RateTables select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                user = user.Where(s => s.RatePerHour.Contains(q) || s.JobTitle.Contains(q));
            }
            else
            {
                user = from p in _context.RateTables select p;
            }

            return View(user);
        }

        public ActionResult AddRate()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            Session["RateSuccess"] = null;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddRate(RateTable model)
        {
            string companyId = Session["CompanyID"].ToString();

            if (ModelState.IsValid)
            {
                model.CompanyID = Convert.ToInt32(companyId);
                var rateExist = IsRateExist(model.JobTitle, model.RatePerHour, model.CompanyID);

                if (rateExist)
                {
                    ModelState.AddModelError("Error", "Rate has already been added for the specified Job Title.");
                    return View(model);
                }

                _context.RateTables.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["RateSuccess"] = "Rate Added!";
                    return RedirectToAction("Index", "Rate");

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

        [AllowAnonymous]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTable rate = _context.RateTables.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var rateToUpdate = _context.RateTables.Find(id);
            if (TryUpdateModel(rateToUpdate, "", new string[] { "RateID", "JobTitle", "RatePerHour" }))
            {
                try
                {
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("Error", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(rateToUpdate);
        }

        // GET: Course/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RateTable rate = _context.RateTables.Find(id);
            if (rate == null)
            {
                return HttpNotFound();
            }
            return View(rate);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            RateTable rate = _context.RateTables.Find(id);
            _context.RateTables.Remove(rate);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool IsRateExist(string title, string rate, int companyId)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.RateTables.Where(a => a.JobTitle == title && a.RatePerHour == rate && a.CompanyID == companyId).FirstOrDefault();
                return v != null;
            }
        }
    }
}