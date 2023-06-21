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
    public class ClientController : Controller
    {
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();

        public ActionResult Clients()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var clients = _context.ClientDetails.ToList();
            return View(clients);
        }

        public ActionResult AddClients()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Companies = GetCompanies();
            Session["ClientsSuccess"] = null;
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Search(string q)
        {
            var user = from p in _context.ClientDetails select p;

            if (!string.IsNullOrWhiteSpace(q))
            {
                user = user.Where(s => s.FirstName.Contains(q) || s.LastName.Contains(q) || s.Gender.Contains(q) || s.Email.Contains(q) || s.CellNo.Contains(q) || s.Username.Contains(q) || s.UserRole.Contains(q) || s.ActiveStatus.Contains(q) || s.DateCreated.Contains(q));
            }
            else
            {
                user = from p in _context.ClientDetails select p;
            }

            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddClients(ClientDetail model)
        {
            string userId = Session["UserId"].ToString();
            int userid = Convert.ToInt32(userId);
            //ViewBag.Companies = GetCompanies();

            if (ModelState.IsValid)
            {
                //model.CompanyID = userid;
                model.DateCreated = DateTime.Now.Date.ToShortDateString();
                model.ActiveStatus = "True";

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

                _context.ClientDetails.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["ClientsSuccess"] = "Client Added!";
                    return RedirectToAction("Clients", "Client");

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
            if (TryUpdateModel(userToUpdate, "", new string[] { "ClientID", "UserRole", "FirstName", "LastName", "CellNo", "Email", "Username", "ActiveStatus"}))
            {
                try
                {
                    _context.SaveChanges();

                    return RedirectToAction("Clients");
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
            return RedirectToAction("Clients");
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
    }
}