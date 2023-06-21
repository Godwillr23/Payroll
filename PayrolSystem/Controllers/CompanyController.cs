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

namespace PayrolSystem.Controllers
{
    public class CompanyController : Controller
    {
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();

        public ActionResult Organizations()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var companies = _context.CompanyDetails.ToList();
            return View(companies);
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {
            var companies = from p in _context.CompanyDetails select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                companies = companies.Where(s => s.CompanyName.Contains(q) || s.CompanyAddress.Contains(q) || s.ActiveStatus.Contains(q) || s.DateCreated.Contains(q));
            }
            else
            {
                companies = from p in _context.CompanyDetails select p;
            }

            return View(companies);
        }

        public ActionResult AddOrganizations()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Session["OrganizationSuccess"] = null;
            return View();
        }

        protected int getRandomNumbers()
        {
            Random rnd = new Random();
            int num = 0;
            for (int j = 0; j < 4; j++)
            {
                num = rnd.Next();
            }

            return num;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrganizations(CompanyDetail model)
        {
            string userId = Session["UserId"].ToString();
            int userid = Convert.ToInt32(userId);

            if (ModelState.IsValid)
            {
                model.MasterID = userid;
                model.DateCreated = DateTime.Now.Date.ToShortDateString();  
                model.PaymentStatus = "UnPaid";
                model.ActiveStatus = "True";
                model.CompanyCode = model.CompanyName.Substring(0, 3) + getRandomNumbers();

                var OrganizationExist = IsOrganizationExist(model.CompanyName);

                if (OrganizationExist)
                {
                    ModelState.AddModelError("Error", "Organization already exist");
                    return View(model);
                }

                _context.CompanyDetails.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["OrganizationSuccess"] = "Organization Added!";
                    return RedirectToAction("Organizations", "Company");

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
        public bool IsOrganizationExist(string company)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.CompanyDetails.Where(a => a.CompanyName == company).FirstOrDefault();
                return v != null;
            }
        }

        [AllowAnonymous]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyDetail comp = _context.CompanyDetails.Find(id);
            if (comp == null)
            {
                return HttpNotFound();
            }
            return View(comp);
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
            var companyToUpdate = _context.CompanyDetails.Find(id);
            if (TryUpdateModel(companyToUpdate, "", new string[] { "CompanyID", "CompanyName", "CompanyAddress", "PaymentStatus", "PaymentDay", "Level", "ActiveStatus" }))
            {
                try
                {
                    _context.SaveChanges();

                    return RedirectToAction("Organizations");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("Error", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(companyToUpdate);
        }

        // GET: Course/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyDetail comp = _context.CompanyDetails.Find(id);
            if (comp == null)
            {
                return HttpNotFound();
            }
            return View(comp);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyDetail comp = _context.CompanyDetails.Find(id);
            _context.CompanyDetails.Remove(comp);
            _context.SaveChanges();
            return RedirectToAction("Organizations");
        }
    }
}