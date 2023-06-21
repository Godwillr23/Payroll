using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using PayrolSystem.Models.DatabaseFirst;
using PayrolSystem.Models.ViewModel;

namespace PayrolSystem.Controllers
{
    public class HomeController : Controller
    {
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();

        public ActionResult Organizations()
        {
            var companies = _context.CompanyDetails.ToList();
            return View(companies);
        }

        public ActionResult AddOrganizations()
        {
            Session["OrganizationSuccess"] = null;
            return View();
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
                model.ActiveStatus = "True";

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
                    return View();

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

        public ActionResult Clients()
        {
            return View();
        }

        public bool IsOrganizationExist(string company)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.CompanyDetails.Where(a => a.CompanyName == company).FirstOrDefault();
                return v != null;
            }
        }

        public bool IsClientsExist(string name, string surname, int companyId)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.ClientDetails.Where(a => a.CompanyID == companyId && a.FirstName == name &&  a.LastName == surname).FirstOrDefault();
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