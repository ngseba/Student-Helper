using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using studentApp2.Models;

namespace studentApp2.Controllers
{
    public class CourseDepartmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseDepartments
        public ActionResult Index()
        {
            var courseDepartments = db.CourseDepartments.Include(c => c.Course).Include(c => c.Department);
            return View(courseDepartments.ToList());
        }

        // GET: CourseDepartments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseDepartment courseDepartment = db.CourseDepartments.Find(id);
            if (courseDepartment == null)
            {
                return HttpNotFound();
            }
            return View(courseDepartment);
        }

        [Authorize(Roles ="Admin")]
        // GET: CourseDepartments/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        // POST: CourseDepartments/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CourseID,DepartmentID")] CourseDepartment courseDepartment)
        {
            if (ModelState.IsValid)
            {
                db.CourseDepartments.Add(courseDepartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseDepartment.CourseID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", courseDepartment.DepartmentID);
            return View(courseDepartment);
        }

        [Authorize(Roles = "Admin")]
        // GET: CourseDepartments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseDepartment courseDepartment = db.CourseDepartments.Find(id);
            if (courseDepartment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseDepartment.CourseID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", courseDepartment.DepartmentID);
            return View(courseDepartment);
        }

        // POST: CourseDepartments/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CourseID,DepartmentID")] CourseDepartment courseDepartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseDepartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseDepartment.CourseID);
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", courseDepartment.DepartmentID);
            return View(courseDepartment);
        }

        [Authorize(Roles = "Admin")]
        // GET: CourseDepartments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseDepartment courseDepartment = db.CourseDepartments.Find(id);
            if (courseDepartment == null)
            {
                return HttpNotFound();
            }
            return View(courseDepartment);
        }

        // POST: CourseDepartments/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseDepartment courseDepartment = db.CourseDepartments.Find(id);
            db.CourseDepartments.Remove(courseDepartment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
