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
    public class ScheduleController : Controller
    {
        // GET: Schedule
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            var clients = _context.ScheduleTables.Where(a => a.CompanyID == CompId).ToList();
            return View(clients);
        }
        public ActionResult CreateSchedule()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            ViewBag.Shifts = GetShift(CompId);
            ViewBag.Sites = GetSites(CompId);
            ViewBag.Employees = GetEmployee(CompId);
            

            Session["ScheduleSuccess"] = null;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSchedule(ScheduleTable model)
        {
            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);

            if (ModelState.IsValid)
            {
                model.CompanyID = Convert.ToInt32(companyId);
                var shiftTableExist = IsScheduleExist(model.CompanyID, model.SiteID, model.EmployeeID, model.ShiftID,model.ScheduledDate);

                if (shiftTableExist)
                {
                    ModelState.AddModelError("Error", "Schedule has already been added for the selected employee.");
                    return View(model);
                }

                _context.ScheduleTables.Add(model);

                try
                {
                    _context.SaveChanges();

                    ViewBag.Sites = GetSites(CompId);
                    ViewBag.Employees = GetEmployee(CompId);
                    ViewBag.Shifts = GetShift(CompId);

                    Session["ScheduleSuccess"] = "Schedule Created!";
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
        public bool IsScheduleExist(int CompanyID, int SiteID, int EmployeeID, int ShiftID, string ScheduledDate)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.ScheduleTables.Where(a => a.CompanyID == CompanyID && 
                a.SiteID == SiteID && a.EmployeeID == EmployeeID && a.ShiftID == ShiftID && a.ScheduledDate == ScheduledDate).FirstOrDefault();
                return v != null;
            }
        }
        private static List<SelectListItem> GetSites(int companyId)
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> site = dbContext.SiteTables.OrderBy(x => x.SiteName).Where(a => a.CompanyID == companyId).Select(c => new SelectListItem
            {
                Value = c.SiteID.ToString(),
                Text = c.SiteName +", "+ c.Location

            }).ToList();

            return site;
        }
        private static List<SelectListItem> GetEmployee(int companyId)
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> emp = dbContext.ClientDetails.OrderBy(x => x.ClientID).Where(a => a.CompanyID == companyId && a.UserRole != "Admin").Select(c => new SelectListItem
            {
                Value = c.ClientID.ToString(),
                Text = c.FirstName + " " + c.LastName

            }).ToList();

            return emp;
        }
        private static List<SelectListItem> GetShift(int companyId)
        {
            // Create db context object here
            PayrolSystemDBEntities dbContext = new PayrolSystemDBEntities();
            //Get the value from database and then set it to ViewBag to pass it View
            List<SelectListItem> shift = dbContext.PatternTables.OrderBy(x => x.WorkShift).Where(a => a.CompanyID == companyId).Select(c => new SelectListItem
            {
                Value = c.PatternID.ToString(),
                Text = c.WorkShift

            }).ToList();

            return shift;
        }
    }
}