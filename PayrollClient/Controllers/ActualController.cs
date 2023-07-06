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
    public class ActualController : Controller
    {
        // GET: Actual
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
    }
}