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


        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult GetCourseList ()
        {
            //var courseList = db.Courses.Join(db.CourseDepartments,
            //    course => course.CourseID,
            //    cd => cd.CourseID,
            //    (course, cd) => new { Course = course, CourseDepartment = cd }).Join(db.Groups,
            //    department => department.CourseDepartment.DepartmentID,
            //    group => group.DepartmentID,
            //    (department, group) => new { CourseDepartment = department, Group = group }).Join(db.TeacherCourses,
            //    courseGroup => courseGroup.CourseDepartment.Course.CourseID,
            //    teacher => teacher.CourseID,
            //    (courseGroup, teacher) => new { CourseGroup = courseGroup, TeacherCourses = teacher }).Join(db.Users,
            //    teacherCoursesGroup => teacherCoursesGroup.TeacherCourses.Teacher.UserId,
            //    user => user.Id,
            //    (teacherCoursesGroup, user) => new { TeacherCoursesGroup = teacherCoursesGroup, User = user })
            //    .Where(teacherCourseList => teacherCourseList.TeacherCoursesGroup.CourseGroup.Group.GroupID == groupID)
            //    .Select(teacherCourseList => new SelectListItem {
            //        Value = teacherCourseList.TeacherCoursesGroup.TeacherCourses.TeacherCoursesID.ToString(),
            //        Text = teacherCourseList.TeacherCoursesGroup.CourseGroup.CourseDepartment.Course.CourseName + " - "
            //       + teacherCourseList.User.Firstname + " " + teacherCourseList.User.Lastname
            //    });

            return Json("2");
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
