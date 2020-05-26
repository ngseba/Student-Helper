using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using studentApp2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace studentApp2.Controllers
{
    public class CatalogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public CatalogsController() { }
        public CatalogsController(ApplicationUserManager userManager)
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

        // GET: Catalogs/ProcessModelling
        public ActionResult Index(string CourseName)
        {
            if (User.IsInRole("Student"))
                return RedirectToAction("Student");
            return RedirectToAction("Teacher");
        }

        public ActionResult Teacher(int? id)
        {
            var userId = User.Identity.GetUserId();
            var course = id;
            var teacher = db.Teachers.FirstOrDefault(dbTeacher => dbTeacher.UserId == userId);

            ViewBag.courseList = db.Courses.Join(db.TeacherCourses,
                c => c.CourseID,
                tc => tc.CourseID,
                (c, tc) => new { Course = c, TeacherCourses = tc })
                .Where(ctc => ctc.TeacherCourses.TeacherID == teacher.TeacherId)
                .ToList().Select(teacherCourse => teacherCourse.Course).ToList();

            return View();
        }

        public ActionResult Student()
        {
            var userId = User.Identity.GetUserId();
            var currentStudent = db.Students.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var dbGrades = from course in db.Courses
                           join cd in db.CourseDepartments
                           on course.CourseID equals cd.CourseID
                           join d in db.Departments
                           on cd.DepartmentID equals d.DepartmentID
                           join g in db.Groups
                           on d.DepartmentID equals g.DepartmentID
                           join s in db.Students
                           on g.GroupID equals s.GroupID
                           join grade in db.Catalogs
                           on s.StudentId equals grade.StudentID
                           where (course.CourseID == grade.CourseID) && s.StudentId == currentStudent.StudentId
                           select new { course, grade };



            var student = new CatalogStudentViewModel();
            var grades = new List<GradeStudentViewModel>();
            foreach (var dbGrade in dbGrades)
            {
                try
                {
                    var grade = new GradeStudentViewModel();
                    grade.CourseName = dbGrade.course.CourseName;
                    grade.Grade = dbGrade.grade.Grade;
                    grade.GradeDate = dbGrade.grade.GradeDate;
                    grades.Add(grade);
                }
                catch (Exception e) { }
            }
            student.grades = grades;
            return View(student);
        }


        public async Task<ActionResult> Course(int id)
        {
            var students = getStudentList(id);
            ApplicationUser au;
            var studentTable = new List<UserStudentViewModel>();
            foreach (var student in students)
            {
                var usvm = new UserStudentViewModel();
                au = await UserManager.FindByIdAsync(student.Student.Student.UserId);
                usvm.StudentId = student.Student.Student.StudentId;
                usvm.Fname = au.Firstname;
                usvm.Lname = au.Lastname;
                usvm.GroupName = student.Student.Student.Group.GroupName;
                try
                {
                    usvm.Grade = student.Grade.Grade;
                    usvm.GradeDate = student.Grade.GradeDate.ToString().Split(' ')[0];
                }
                catch (Exception e) { }
                studentTable.Add(usvm);
            }
            var ctvm = new CatalogTeacherViewModel();
            ctvm.Students = studentTable;
            ctvm.CourseID = id;
            ctvm.CourseName = db.Courses.FirstOrDefault(course => course.CourseID == id).CourseName;
            return View(ctvm);
        }

        public dynamic getStudentList(int courseID)
        {
            var userId = User.Identity.GetUserId();
            var teacher = db.Teachers.FirstOrDefault(dbTeacher => dbTeacher.UserId == userId);
            var studentGrades = db.Students.GroupJoin(db.Catalogs,
                s => s.StudentId,
                grade => grade.StudentID,
                (s, g) => new { Student = s, Grade = g })
                .SelectMany(g => g.Grade.DefaultIfEmpty(),
                (s, grade) => new { Student = s, Grade = grade });
            var studentGroups = studentGrades.Join(db.Groups,
                s => s.Student.Student.GroupID,
                g => g.GroupID,
                (s, g) => new { Student = s, Group = g });
            var studGroupTCG = studentGroups
                .Join(db.TeacherCoursesGroups,
                sg => sg.Group.GroupID,
                tcg => tcg.GroupID,
                (sg, tcg) => new { StudentGroup = sg, TeacherCoursesGroup = tcg });
            var students = studGroupTCG
                .Join(db.TeacherCourses,
                stcg => stcg.TeacherCoursesGroup.TeacherCoursesID,
                tc => tc.TeacherCoursesID,
                (stcg, tc) => new { StudentTeacherCoursesGroup = stcg, TeacherCourses = tc })
              .Where(stcg2 => stcg2.TeacherCourses.TeacherID == teacher.TeacherId && stcg2.TeacherCourses.CourseID == courseID)
              .ToList().Select(student => student.StudentTeacherCoursesGroup.StudentGroup.Student).ToList();
            return students;
        }



        // POST: Catalogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create([Bind(Include = "GradeID,StudentID,CourseID,Grade")] Catalog catalog)
        {
            if (ModelState.IsValid)
            {
                catalog.GradeDate = DateTime.Today;
                db.Catalogs.Add(catalog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(catalog);
        }

        // GET: Catalogs/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var students = getStudentList(id);
            ApplicationUser au;
            var studentTable = new List<UserStudentViewModel>();
            foreach (var student in students)
            {
                var usvm = new UserStudentViewModel();
                au = await UserManager.FindByIdAsync(student.Student.Student.UserId);
                usvm.StudentId = student.Student.Student.StudentId;
                usvm.Fname = au.Firstname;
                usvm.Lname = au.Lastname;
                usvm.GroupName = student.Student.Student.Group.GroupName;
                try
                {
                    usvm.Grade = student.Grade.Grade;
                    usvm.GradeDate = student.Grade.GradeDate.ToString().Split(' ')[0];
                    usvm.GradeID = student.Grade.GradeID;
                }
                catch (Exception e) { }
                studentTable.Add(usvm);
            }
            var ctvm = new CatalogTeacherViewModel();
            ctvm.Students = studentTable;
            ctvm.CourseID = id;
            return View(ctvm);
        }

        // POST: Catalogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "GradeID,StudentID,CourseID,Grade")] Catalog catalog)
        {
            var gradeId = catalog.GradeID;
            var catalogstud = catalog.StudentID;
            var catalogCourseID = catalog.CourseID;
            var cataloggrade = catalog.Grade;
            if (ModelState.IsValid)
            {
                try
                {
                    var updateRecord = db.Catalogs.FirstOrDefault(grade => grade.GradeID == catalog.GradeID);
                    updateRecord.Grade = catalog.Grade;
                    updateRecord.GradeDate = DateTime.Today;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ((IObjectContextAdapter)db).ObjectContext.Refresh(RefreshMode.ClientWins, db.Catalogs);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Catalogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Catalog catalog = db.Catalogs.Find(id);
            if (catalog == null)
            {
                return HttpNotFound();
            }
            return View(catalog);
        }

        // POST: Catalogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Catalog catalog = db.Catalogs.Find(id);
            db.Catalogs.Remove(catalog);
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
