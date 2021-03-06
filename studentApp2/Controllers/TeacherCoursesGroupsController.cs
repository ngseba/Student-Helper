﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
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
                var studentList = db.Students
                    .Where(student => student.GroupID == teacherCoursesGroup.GroupID).ToList();
                var courseId = db.TeacherCourses
                    .Where(teacherCourses => teacherCourses.TeacherCoursesID == teacherCoursesGroup.TeacherCoursesID)
                    .FirstOrDefault().CourseID;
                foreach (var student in studentList)
                {
                    var grade = new Catalog { StudentID = student.StudentId, CourseID = courseId, Grade = 0, GradeDate = null };
                    db.Catalogs.Add(grade);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", teacherCoursesGroup.GroupID);
            ViewBag.TeacherCoursesID = new SelectList(db.TeacherCourses, "TeacherCoursesID", "TeacherCoursesID", teacherCoursesGroup.TeacherCoursesID);
            return View(teacherCoursesGroup);
        }


        [HttpPost]
        
        public ActionResult GetCourseList (int groupID)
        {
            var assignedCourses = db.TeacherCoursesGroups
                .Where(teacherCourseGroup => teacherCourseGroup.GroupID == groupID)
                .Select(assignedCoursesList => assignedCoursesList.TeacherCourses.CourseID);

           
            var courseList = db.CourseDepartments.Join(db.Groups,
                courseDepartment => courseDepartment.DepartmentID,
                group => group.DepartmentID,
                (courseDepartment,group) => new {
                    CourseDepartment = courseDepartment,
                    Group = group
                }).Join(db.TeacherCourses,
                courseDepartmentGroup => new { courseDepartmentGroup.CourseDepartment.CourseID},
                teacherCourses => new { teacherCourses.CourseID },
                (courseDepartmentGroup, teacherCourses) => new{
                    CourseDepartmentGroup = courseDepartmentGroup,
                    TeacherCourses = teacherCourses
                }).Where(teacherCoursesGroupDepartments => teacherCoursesGroupDepartments.CourseDepartmentGroup.Group.GroupID == groupID)
                .Where(teacherCoursesGroupDepartments => !assignedCourses.Contains(teacherCoursesGroupDepartments.TeacherCourses.CourseID))
                .Where(teacherCoursesGroupDepartments => 
                teacherCoursesGroupDepartments.CourseDepartmentGroup.Group.YearOfStudy == 
                teacherCoursesGroupDepartments.CourseDepartmentGroup.CourseDepartment.Course.CourseYear)
                .Select(teacherCourseList =>  new SelectListItem
                {
                    Value = teacherCourseList.TeacherCourses.TeacherCoursesID.ToString(),
                    Text = teacherCourseList.TeacherCourses.Course.CourseName + " - "
                   + teacherCourseList.TeacherCourses.Teacher.User.Firstname + " " +
                   teacherCourseList.TeacherCourses.Teacher.User.Lastname
                }).ToList();
            return Json(courseList);
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
                var teacherCoursesID = db.TeacherCoursesGroups
                    .Where(dbTeacherCoursesGroup => dbTeacherCoursesGroup.TeacherCoursesGroupID == teacherCoursesGroup.TeacherCoursesGroupID)
                    .Select(dbTeacherCoursesGroup => dbTeacherCoursesGroup.TeacherCoursesID);
                var courseId = db.TeacherCourses
                   .Where(teacherCourses => teacherCourses.TeacherCoursesID == teacherCoursesGroup.TeacherCoursesID)
                   .FirstOrDefault().CourseID;
                var groupId = db.TeacherCoursesGroups
                   .Where(dbTeacherCoursesGroup => dbTeacherCoursesGroup.TeacherCoursesGroupID == teacherCoursesGroup.TeacherCoursesGroupID)
                   .FirstOrDefault().GroupID;
                var gradeList = db.Catalogs.Join(db.Students,
                    Catalog => Catalog.StudentID,
                    Student => Student.StudentId,
                    (Catalog, Student) => new { Student = Student, Catalog = Catalog })
                    .Where(grade => grade.Catalog.CourseID == courseId)
                    .Where(grade => grade.Student.GroupID == groupId)
                    .Select(studentCatalog => studentCatalog.Catalog)
                    .ToList();
                foreach (var grade in gradeList)
                {
                    var dbGrade = db.Catalogs.Where(catalog => catalog.GradeID == grade.GradeID).FirstOrDefault();
                    db.Catalogs.Remove(dbGrade);
                    db.SaveChanges();
                }
                var newTeacherCoursesGroup = db.TeacherCoursesGroups
                    .Where(dbTcg => dbTcg.TeacherCoursesGroupID == teacherCoursesGroup.TeacherCoursesGroupID)
                    .FirstOrDefault();
                newTeacherCoursesGroup.TeacherCoursesGroupID = teacherCoursesGroup.TeacherCoursesGroupID;
                newTeacherCoursesGroup.TeacherCoursesID = teacherCoursesGroup.TeacherCoursesID;
                newTeacherCoursesGroup.GroupID = teacherCoursesGroup.GroupID;
                db.SaveChanges();
                var studentList = db.Students
                    .Where(student => student.GroupID == newTeacherCoursesGroup.GroupID).ToList();
                courseId = db.TeacherCourses
                    .Where(teacherCourses => teacherCourses.TeacherCoursesID == newTeacherCoursesGroup.TeacherCoursesID)
                    .FirstOrDefault().CourseID;
                foreach (var student in studentList)
                {
                    var grade = new Catalog { StudentID = student.StudentId, CourseID = courseId, Grade = 0, GradeDate = null };
                    db.Catalogs.Add(grade);
                }
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
