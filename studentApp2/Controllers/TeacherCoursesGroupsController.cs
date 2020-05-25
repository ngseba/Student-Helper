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
    public class TeacherCoursesGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public TeacherCoursesGroupsController() { }
        public TeacherCoursesGroupsController(ApplicationUserManager userManager)
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

        // GET: TeacherCoursesGroups
        public async Task<ActionResult> Index()
        {

            var teacherCoursesGroups = db.TeacherCoursesGroups.Include(t => t.Group).Include(t => t.TeacherCourses).ToList();
            var disp = new List<TCGViewModel>();
            foreach (var item in teacherCoursesGroups)
            {
                var teacher = db.Teachers.First(t => t.TeacherId == item.TeacherCourses.TeacherID);
                var course = db.Courses.First(c => c.CourseID == item.TeacherCourses.CourseID);
                ApplicationUser au = await UserManager.FindByIdAsync(teacher.UserId);
                disp.Add(new TCGViewModel
                {
                    TeacherCourseGroupID = item.TeacherCoursesGroupID,
                    tName = new TeacherNameViewModel
                        {
                            TCId = item.TeacherCoursesID,
                            UserId = teacher.UserId,
                            TeacherId = item.TeacherCourses.TeacherID,
                            FName = au.Firstname,
                            LName = au.Lastname,
                            CourseId = item.TeacherCourses.CourseID,
                            CourseName = course.CourseName
                        },
                    groupName = item.Group.GroupName
                });
            }
            ViewBag.Display = disp;
            return View();
        }

        // GET: TeacherCoursesGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCoursesGroup teacherCoursesGroup = db.TeacherCoursesGroups.Find(id);
            if (teacherCoursesGroup == null)
            {
                return HttpNotFound();
            }
            return View(teacherCoursesGroup);
        }

        // GET: TeacherCoursesGroups/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName");
            ViewBag.TeacherCoursesID = new SelectList(db.TeacherCourses, "TeacherCoursesID", "TeacherCoursesID");

            var sql = @"SELECT dbo.TeacherCourses.TeacherCoursesId as Value, 
            dbo.Courses.CourseName + ': ' + Users.LastName + ', ' + Users.FirstName  AS Text
            FROM dbo.AspNetUsers AS Users
            JOIN dbo.Teachers ON Users.Id = dbo.Teachers.UserId
            JOIN dbo.TeacherCourses ON dbo.TeacherCourses.TeacherId = dbo.Teachers.TeacherId
            JOIN dbo.Courses ON dbo.Courses.CourseId = dbo.TeacherCourses.CourseId
            Order by dbo.Courses.CourseName, Users.LastName, Users.FirstName";
            var res = db.Database.SqlQuery<QueryResults>(sql).ToList();
            SelectList teacherCourses = new SelectList(res, "Value", "Text");

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.TeacherCoursesID = teacherCourses;

            return View();
        }

        // POST: TeacherCoursesGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TeacherCoursesGroupID,TeacherCoursesID,GroupID")] TeacherCoursesGroup teacherCoursesGroup)
        {
            if (ModelState.IsValid)
            {
                db.TeacherCoursesGroups.Add(teacherCoursesGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", teacherCoursesGroup.GroupID);
            ViewBag.TeacherCoursesID = new SelectList(db.TeacherCourses, "TeacherCoursesID", "TeacherCoursesID", teacherCoursesGroup.TeacherCoursesID);
            return View(teacherCoursesGroup);
        }

        // GET: TeacherCoursesGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCoursesGroup teacherCoursesGroup = db.TeacherCoursesGroups.Find(id);
            if (teacherCoursesGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", teacherCoursesGroup.GroupID);
            ViewBag.TeacherCoursesID = new SelectList(db.TeacherCourses, "TeacherCoursesID", "TeacherCoursesID", teacherCoursesGroup.TeacherCoursesID);
            return View(teacherCoursesGroup);
        }

        // POST: TeacherCoursesGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TeacherCoursesGroupID,TeacherCoursesID,GroupID")] TeacherCoursesGroup teacherCoursesGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacherCoursesGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", teacherCoursesGroup.GroupID);
            ViewBag.TeacherCoursesID = new SelectList(db.TeacherCourses, "TeacherCoursesID", "TeacherCoursesID", teacherCoursesGroup.TeacherCoursesID);
            return View(teacherCoursesGroup);
        }

        // GET: TeacherCoursesGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TeacherCoursesGroup teacherCoursesGroup = db.TeacherCoursesGroups.Find(id);
            if (teacherCoursesGroup == null)
            {
                return HttpNotFound();
            }
            return View(teacherCoursesGroup);
        }

        // POST: TeacherCoursesGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TeacherCoursesGroup teacherCoursesGroup = db.TeacherCoursesGroups.Find(id);
            db.TeacherCoursesGroups.Remove(teacherCoursesGroup);
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
