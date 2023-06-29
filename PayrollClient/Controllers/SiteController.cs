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
    public class SiteController : Controller
    {
        // GET: Site
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var clients = _context.SiteTables.Where(a => a.CompanyID == CompId).ToList();
            return View(clients);
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {
            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var site = from p in _context.SiteTables select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                site = site.Where(s => s.Location.Contains(q) || s.Period.Contains(q) || s.Requirement.Contains(q));
            }
            else
            {
                site = from p in _context.SiteTables.Where(a => a.CompanyID == CompId) select p;
            }

            return View(site);
        }

        public ActionResult AddSite()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            Session["SiteSuccess"] = null;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddSite(SiteTable model)
        {
            string userId = Session["UserId"].ToString();
            int userid = Convert.ToInt32(userId);
            string companyId = Session["CompanyID"].ToString();

            if (ModelState.IsValid)
            {
                model.CompanyID = Convert.ToInt32(companyId);

                var siteExist = IsSiteExist(model.Location, model.Period, model.Requirement, model.CompanyID);

                if (siteExist)
                {
                    ModelState.AddModelError("Error", "Site already exist");
                    return View(model);
                }

                _context.SiteTables.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["SiteSuccess"] = "Site Added!";
                    return RedirectToAction("Index", "Site");

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
            SiteTable site = _context.SiteTables.Find(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
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
            var siteToUpdate = _context.SiteTables.Find(id);
            if (TryUpdateModel(siteToUpdate, "", new string[] {"SiteName", "Location", "Period", "Requirement" }))
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
            return View(siteToUpdate);
        }

        // GET: Course/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SiteTable site = _context.SiteTables.Find(id);
            if (site == null)
            {
                return HttpNotFound();
            }
            return View(site);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            SiteTable user = _context.SiteTables.Find(id);
            _context.SiteTables.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool IsSiteExist(string location, string period, string requirement, int companyId)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.SiteTables.Where(a => a.CompanyID == companyId && a.Location == location && a.Period == period && a.Requirement == requirement).FirstOrDefault();
                return v != null;
            }
        }
    }
}