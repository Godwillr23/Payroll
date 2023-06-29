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
    public class PatternController : Controller
    {
        // GET: Rate
        PayrolSystemDBEntities _context = new PayrolSystemDBEntities();
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            string companyId = Session["CompanyID"].ToString();
            int CompId = Convert.ToInt32(companyId);
            var clients = _context.PatternTables.Where(a => a.CompanyID == CompId).ToList();
            return View(clients);
        }

        public ActionResult AddShift()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "ClientAccount");
            }

            Session["ShiftSuccess"] = null;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult AddShift(PatternTable model)
        {
            string companyId = Session["CompanyID"].ToString();

            if (ModelState.IsValid)
            {
                model.CompanyID = Convert.ToInt32(companyId);
                var shiftTableExist = IsShiftExist(model.WorkShift);

                if (shiftTableExist)
                {
                    ModelState.AddModelError("Error", "Shift has already been added.");
                    return View(model);
                }

                _context.PatternTables.Add(model);

                try
                {
                    _context.SaveChanges();
                    Session["ShiftSuccess"] = "Shift Added!";
                    return RedirectToAction("Index", "Pattern");

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
            PatternTable shift = _context.PatternTables.Find(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
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
            var shiftToUpdate = _context.PatternTables.Find(id);
            if (TryUpdateModel(shiftToUpdate, "", new string[] { "PatternID", "WorkShift", "HoursWorked", "ClockInTime", "ClockOutTime" }))
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
            return View(shiftToUpdate);
        }

        // GET: Course/Delete/5
        [AllowAnonymous]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PatternTable shift = _context.PatternTables.Find(id);
            if (shift == null)
            {
                return HttpNotFound();
            }
            return View(shift);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(int id)
        {
            PatternTable shift = _context.PatternTables.Find(id);
            _context.PatternTables.Remove(shift);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool IsShiftExist(string shift)
        {
            using (PayrolSystemDBEntities dc = new PayrolSystemDBEntities())
            {
                var v = dc.PatternTables.Where(a => a.WorkShift == shift).FirstOrDefault();
                return v != null;
            }
        }
    }
}