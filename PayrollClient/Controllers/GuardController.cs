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
    public class GuardController : Controller
    {
        // GET: Guard
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var guard = _context.GuardsDetails.Where(a => a.CompanyID == CompId).ToList();
            return View(guard);
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {
            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var user = from p in _context.GuardsDetails select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                user = user.Where(s => s.PersNo.Contains(q) || s.FirstName.Contains(q) || s.LastName.Contains(q) || s.Gender.Contains(q) || s.Email.Contains(q) || s.CellNo.Contains(q) || s.ActiveStatus.Contains(q) || s.DateCreated.Contains(q) || s.Grade.Contains(q));
            }
            else
            {
                user = from p in _context.GuardsDetails.Where(a => a.CompanyID == CompId) select p;
            }

            return View(user);
        }

        public ActionResult AddGuard()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }
            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            Session["guardSuccess"] = null;
            ViewBag.JobTitle = GetTitle(CompId);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddGuard(GuardsDetail model)
        {
            string userId = Session["UserId"].ToString();
            int userid = Convert.ToInt32(userId);
            //ViewBag.Companies = GetCompanies();
            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var lastId = _context.GuardsDetails.Find(_context.GuardsDetails.Max(p => p.GuardsID));
            int convertedlasstID = Convert.ToInt32(lastId);

            if (ModelState.IsValid)
            {
                //model.CompanyID = userid;
                model.DateCreated = DateTime.Now.Date.ToShortDateString();
                model.PersNo = model.FirstName.Substring(0,1) + model.LastName.Substring(0, 1) + 
                    model.Grade + model.CompanyID + "PP"+ convertedlasstID;
                model.ActiveStatus = "True";
                model.CompanyID = CompId;

                var ClientExist = IsClientsExist(model.FirstName, model.LastName, model.CompanyID);
                var EmailExist = IsEmailExist(model.Email);
                var CellphoneExist = IsEmailExist(model.Email);

                if (ClientExist)
                {
                    ModelState.AddModelError("Error", "Client already exist");
                    return View(model);
                }
                else if (EmailExist)
                {
                    ModelState.AddModelError("Error", "Email Address already exist");
                    return View(model);
                }
                else if (CellphoneExist)
                {
                    ModelState.AddModelError("Error", "Cellphone Number already exist");
                    return View(model);
                }

                _context.GuardsDetails.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["guardSuccess"] = "Guard Added!";
                    return RedirectToAction("Index", "Guard");
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
            ClientDetail user = _context.ClientDetails.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
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
            var userToUpdate = _context.ClientDetails.Find(id);
            if (TryUpdateModel(userToUpdate, "", new string[] { "ClientID", "JobTitle", "FirstName", "LastName", "CellNo", "Email", "ActiveStatus" }))
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
            return View(userToUpdate);
        }

        // GET: Course/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientDetail user = _context.ClientDetails.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientDetail user = _context.ClientDetails.Find(id);
            _context.ClientDetails.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool IsClientsExist(string name, string surname, int companyId)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.ClientDetails.Where(a => a.CompanyID == companyId && a.FirstName == name && a.LastName == surname).FirstOrDefault();
                return v != null;
            }
        }

        public bool IsEmailExist(string email)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.ClientDetails.Where(a => a.Email == email).FirstOrDefault();
                return v != null;
            }
        }
        public bool IsCellphoneExist(string cell)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.ClientDetails.Where(a => a.CellNo == cell).FirstOrDefault();
                return v != null;
            }
        }
        private static List<SelectListItem> GetTitle(int companyId)
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> shift = dbContext.RateTables.OrderBy(x => x.JobTitle).Where(a => a.CompanyID == companyId).Select(c => new SelectListItem
            {
                Value = c.JobTitle,
                Text = c.JobTitle

            }).ToList();

            return shift;
        }
    }
}