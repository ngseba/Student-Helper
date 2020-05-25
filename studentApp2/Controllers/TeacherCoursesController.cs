using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using studentApp2.Models;

namespace studentApp2.Controllers
{
    public class TeacherCoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public TeacherCoursesController() { }
        public TeacherCoursesController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: TeacherCourses
        public async Task<ActionResult> Index()
        {
            var teacherCourses = db.TeacherCourses.Include(t => t.Course).Include(t => t.Teacher);
            var disp = new List<TeacherNameViewModel>();
            foreach (var item in teacherCourses)
            {
                ApplicationUser au = await UserManager.FindByIdAsync(item.Teacher.UserId);
                disp.Add( new TeacherNameViewModel 
                { 
                    TCId = item.TeacherCoursesID,
                    UserId = item.Teacher.UserId,
                    TeacherId = item.Teacher.TeacherId,
                    FName = au.Firstname,
                    LName = au.Lastname,
                    CourseId = item.Course.CourseID,
                    CourseName = item.Course.CourseName
                });
            }
            ViewBag.Display = disp;
            return View();
        }

        // GET: TeacherCourses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourses teacherCourses = db.TeacherCourses.Find(id);
            if (teacherCourses == null)
            {
                return HttpNotFound();
            }
            return View(teacherCourses);
        }

        // GET: TeacherCourses/Create
        public ActionResult Create()
        {
            var sql = @"SELECT dbo.Teachers.TeacherId as Value, 
            Users.LastName + ', ' + Users.FirstName  AS Text
            FROM dbo.AspNetUsers AS Users
            JOIN dbo.Teachers ON Users.Id = dbo.Teachers.UserId
            Order by Users.LastName, Users.FirstName ";
            var res = db.Database.SqlQuery<QueryResults>(sql).ToList();
            SelectList teachers = new SelectList(res, "Value", "Text");

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.TeacherID = teachers;
            return View();
        }

        // POST: TeacherCourses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeacherCoursesID,CourseID,TeacherID")] TeacherCourses teacherCourses)
        {
            if (ModelState.IsValid)
            {
                db.TeacherCourses.Add(teacherCourses);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", teacherCourses.CourseID);
            ViewBag.TeacherID = new SelectList(db.Teachers, "TeacherId", "UserId", teacherCourses.TeacherID);
            return View(teacherCourses);
        }

        // GET: TeacherCourses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourses teacherCourses = db.TeacherCourses.Find(id);
            if (teacherCourses == null)
            {
                return HttpNotFound();
            }

            var sql = @"SELECT dbo.Teachers.TeacherId as Value, 
            Users.LastName + ', ' + Users.FirstName  AS Text
            FROM dbo.AspNetUsers AS Users
            JOIN dbo.Teachers ON Users.Id = dbo.Teachers.UserId
            Order by Users.LastName, Users.FirstName ";
            var res = db.Database.SqlQuery<QueryResults>(sql).ToList();
            SelectList teachers = new SelectList(res, "Value", "Text");

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", teacherCourses.CourseID);
            ViewBag.TeacherID = teachers;
            return View(teacherCourses);
        }

        // POST: TeacherCourses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeacherCoursesID,CourseID,TeacherID")] TeacherCourses teacherCourses)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacherCourses).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", teacherCourses.CourseID);
            ViewBag.TeacherID = new SelectList(db.Teachers, "TeacherId", "UserId", teacherCourses.TeacherID);
            return View(teacherCourses);
        }

        // GET: TeacherCourses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCourses teacherCourses = db.TeacherCourses.Find(id);
            if (teacherCourses == null)
            {
                return HttpNotFound();
            }
            return View(teacherCourses);
        }

        // POST: TeacherCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeacherCourses teacherCourses = db.TeacherCourses.Find(id);
            db.TeacherCourses.Remove(teacherCourses);
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
