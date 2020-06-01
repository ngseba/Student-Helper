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
        [Authorize(Roles = "Student,Teacher")]
        public ActionResult Index(string CourseName)
        {
            //if (User.IsInRole("Student"))
            //    return RedirectToAction("Student");
            //return RedirectToAction("Teacher");
            if (User.IsInRole("Teacher"))
            {
                ViewBag.CourseList = new SelectList(getCourseList(), "CourseID", "CourseName");

            }
            else
            {
                ViewBag.StudentGradeList = getStudentGrades();
            }
            return View();
        }

     

        public List<Course> getCourseList()
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbTeacher => dbTeacher.UserId == userId);
            var courseList = db.TeacherCourses
                .Where(teacherCourses => teacherCourses.TeacherID == currentTeacher.TeacherId)
                .Select(teacherCourses => teacherCourses.Course)
                .ToList();


            return courseList;


        }

        [Authorize(Roles = "Teacher")]

        public String getGroupList(int id)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var teacherCourseGroupList = db.TeacherCoursesGroups
                .Where(teacherCourseGroup => teacherCourseGroup.TeacherCourses.TeacherID == currentTeacher.TeacherId
                && teacherCourseGroup.TeacherCourses.CourseID == id)
                .Select(teacherCourseGroup => new {
                    GroupID = teacherCourseGroup.Group.GroupID,
                    GroupName = teacherCourseGroup.Group.GroupName
                }).ToList();

            string output = JsonConvert.SerializeObject(teacherCourseGroupList);
            return output;

        }

        public String getCourseGrades(int id)
        {
            var userId = User.Identity.GetUserId();
            var currentTeacher = db.Teachers.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
            var gradeList = db.TeacherCoursesGroups.Join(db.Students,
                teacherCoursesGroup => teacherCoursesGroup.GroupID,
                student => student.GroupID,
                (teacherCoursesGroup, student) => new { TeacherCoursesGroup = teacherCoursesGroup, Student = student })
                .Where(StudentsGroup => StudentsGroup.TeacherCoursesGroup.TeacherCourses.TeacherID == currentTeacher.TeacherId &&
                StudentsGroup.TeacherCoursesGroup.TeacherCourses.CourseID == id).Join(db.Catalogs,
                StudentsGroup => StudentsGroup.Student.StudentId,
                Catalog => Catalog.StudentID,
                (StudentsGroup, Catalog) => new {StudentGroup = StudentsGroup, Catalog = Catalog })
                .Where(catalog => catalog.Catalog.CourseID == id).Join(db.Users,
                StudentGroupCatalog => StudentGroupCatalog.StudentGroup.Student.UserId,
                User => User.Id,
                (StudentGroupCatalog, User) => new { StudentGroupCatalog = StudentGroupCatalog, User = User })
                .Select(Catalog => new
                {
                    GroupName = Catalog.StudentGroupCatalog.StudentGroup.TeacherCoursesGroup.Group.GroupName,
                    StudentName = Catalog.StudentGroupCatalog.StudentGroup.Student.User.Firstname + " " 
                    + Catalog.StudentGroupCatalog.StudentGroup.Student.User.Lastname,
                    Grade = Catalog.StudentGroupCatalog.Catalog,
                    GroupId = Catalog.StudentGroupCatalog.StudentGroup.TeacherCoursesGroup.Group.GroupID

                }).ToList();
            return JsonConvert.SerializeObject(gradeList); ;
        }

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

        [HttpPost]
        public JsonResult Edit([Bind(Include = "GradeID,StudentID,CourseID,Grade,GradeDate")] Catalog catalog)
        {
            Catalog grade = db.Catalogs.Where(dbGrade => dbGrade.GradeID == catalog.GradeID)
                .FirstOrDefault();
            if (ModelState.IsValid)
            {
                grade.Grade = catalog.Grade;
                grade.GradeDate = catalog.GradeDate;
                db.SaveChanges();
                var response = new
                {
                    info = "Success",
                    date = grade.GradeDate.ToString(),
                    studentId = grade.StudentID
                };
                return Json(response);
            }
            else
            {
                var response = new
                {
                    info = "An error has occured in upgrading grade"
                };
                return Json(response);
            }
        }

        public List<GradeStudentViewModel> getStudentGrades()
        {
            var userId = User.Identity.GetUserId();
            var currentStudent = db.Students.FirstOrDefault(dbStudent => dbStudent.UserId == userId);
           
            var grades = db.Catalogs.Join(db.Courses,
                Catalog => Catalog.CourseID,
                Course => Course.CourseID,
                (Catalog,Course) => new { Catalog = Catalog,Course = Course})
                .Where(grade => grade.Catalog.StudentID == currentStudent.StudentId)
                .Select(grade =>
            new GradeStudentViewModel { 
                CourseName = grade.Course.CourseName,
                GradeDate = grade.Catalog.GradeDate,
                Grade = grade.Catalog.Grade
            }).ToList();

            return grades;
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
