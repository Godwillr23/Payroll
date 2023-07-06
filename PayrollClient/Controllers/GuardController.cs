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
            ViewBag.Grade = GetGrade(CompId);
            Session["guardSuccess"] = null;
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

            //Get last ID
            int lstID = getLastId();
            int convertedlasstID = 0;
            if (lstID == 0)
            {
                convertedlasstID = 0;
            }
            else
            {
                convertedlasstID = lstID;
            }

            if (ModelState.IsValid)
            {
                //model.CompanyID = userid;
                model.DateCreated = DateTime.Now.Date.ToShortDateString();
                model.PersNo = model.FirstName.Substring(0, 1) + model.LastName.Substring(0, 1) +
                    model.Grade + model.CompanyID + "PP" + (convertedlasstID + 1);
                model.ActiveStatus = "True";
                model.CompanyID = CompId;
                model.UserID = userid;

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
                    ViewBag.Grade = GetGrade(CompId);
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
            GuardsDetail guard = _context.GuardsDetails.Find(id);
            if (guard == null)
            {
                return HttpNotFound();
            }

            return View(guard);
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
            var userToUpdate = _context.GuardsDetails.Find(id);
            if (TryUpdateModel(userToUpdate, "", new string[] { "GuardID", "FirstName", "LastName", "CellNo", "Email","Grade", "ActiveStatus" }))
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
            GuardsDetail user = _context.GuardsDetails.Find(id);
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
            GuardsDetail user = _context.GuardsDetails.Find(id);
            _context.GuardsDetails.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool IsClientsExist(string name, string surname, int companyId)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.GuardsDetails.Where(a => a.CompanyID == companyId && a.FirstName == name && a.LastName == surname).FirstOrDefault();
                return v != null;
            }
        }

        public bool IsEmailExist(string email)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.GuardsDetails.Where(a => a.Email == email).FirstOrDefault();
                return v != null;
            }
        }
        public bool IsCellphoneExist(string cell)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.GuardsDetails.Where(a => a.CellNo == cell).FirstOrDefault();
                return v != null;
            }
        }

        public int getLastId()
        {
            int v = _context.GuardsDetails.Count();
            return v;
        }

        private static List<SelectListItem> GetGrade(int companyId)
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> shift = dbContext.RateTables.OrderBy(x => x.Grade).Where(a => a.CompanyID == companyId).Select(c => new SelectListItem
            {
                Value = c.Grade,
                Text = c.Grade

            }).ToList();

            return shift;
        }
    }
}