using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using PayrolSystem.Models.DatabaseFirst;
//using PayrolSystem.Models.ViewModel;


namespace PayrolSystem.Controllers
{
    public class AccountController : Controller
    {
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        [Authorize]
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        //Login POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(MasterLogin model)
        {
            if (model.Email != null && model.LogPassword != null)
            {
                var v = _context.MasterLogins.Where(a => a.Email == model.Email).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(Crypto.Hash(model.LogPassword), v.LogPassword) == 0)
                    {

                        var IsUserLegit = _context.MasterLogins.Where(r => r.Email == model.Email);

                        if (IsUserLegit != null)
                        {

                            int timeout = 2800; // 525600 min = 1 year
                            var ticket = new FormsAuthenticationTicket(model.Email, true, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);

                            string email = model.Email;
                            int userid = (from e in _context.MasterLogins
                                          where e.Email == email
                                          select e.MasterID).SingleOrDefault();

                            Session["UserId"] = userid;
                            Session["Email"] = email;
                            Session["Names"] = v.FirstName + " " + v.LastName;

                            return RedirectToAction("Organizations", "Company");
                        }
                        else
                        {
                            ModelState.AddModelError("Error", "Invalid Password");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Invalid Password");
                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "User not found in the database");
                }
            }

            return View();

        }
        [AllowAnonymous]
        public ActionResult MasterProfile()
        {

            string userEmail = Session["Email"].ToString();

            if (userEmail == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = _context.MasterLogins.FirstOrDefault(s => s.Email == userEmail);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(MasterLogin userToUpdate)
        {
            if (ModelState.IsValid)
            {
                string userEmail = Session["Email"].ToString();

                if (userEmail == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                userToUpdate = _context.MasterLogins.FirstOrDefault(s => s.Email == userEmail);

                try
                {
                    if (TryUpdateModel(userToUpdate, "", new string[] { "MasterID", "Firstname", "Lastname", "Email" }))
                    {
                        _context.SaveChanges();

                        return RedirectToAction("MasterProfile", "Account");
                    }
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
            return View(userToUpdate);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult UpdatePassword(MasterLogin model)
        {
            var userEmail = Session["Email"].ToString();
            var currentPassword = model.LogPassword;

            var user = _context.MasterLogins.FirstOrDefault(s => s.Email == userEmail);
            if (user == null)
            {
                // user password did not match the DB password
                throw new Exception("Invalid password exception");
            }

            // update the new password
            user.LogPassword = Crypto.Hash(model.LogPassword);

            // save the Database changes
            _context.SaveChanges();

            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }

        [Authorize]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Login", "Account");
        }
    }
}