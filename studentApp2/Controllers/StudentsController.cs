using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using studentApp2.Models;

namespace studentApp2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public StudentsController() { }
        public StudentsController(ApplicationUserManager userManager)
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

        // GET: Students
        public ActionResult Index()
        {
            var students = db.Students.ToList();
            var userStud = new List<UserStudentViewModel>();
            foreach (var item in students)
            {
                var uid = item.UserId;
                ApplicationUser au = db.Users.First(u => u.Id == uid);
                userStud.Add(new UserStudentViewModel
                {
                    StudentId = item.StudentId,
                    UserId = item.UserId,
                    Username = au.UserName,
                    Fname = au.Firstname,
                    Lname = au.Lastname,
                    GroupName = "N/A",
                    YearOfStudy = 0
                });
                if(item.GroupID != null)
                {
                    userStud.Last().GroupName = item.Group.GroupName;
                    userStud.Last().YearOfStudy = item.Group.YearOfStudy;
                }
            }
            ViewBag.Students = userStud;
            return View();
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }

            //Get selected student role (deh)
            ApplicationUser au = db.Users.First(u => u.Id == student.UserId);
            string RoleName = "";
            foreach (IdentityUserRole role in au.Roles)
            {
                string name = role.RoleId;
                RoleName = db.Roles.First(r => r.Id == role.RoleId).Name;
            }

            ViewBag.Student = student;
            ViewBag.Role = RoleName;
            return View();
        }

        [Authorize(Roles = "Admin")]
        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName",student.GroupID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,GroupID,YearOfStudy,UserId")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.Groups, "GroupID", "GroupName", student.GroupID);
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        [Authorize(Roles = "Admin")]
        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);

            //Remove student from role(leaving a user without a set role)
            var currentUser =await UserManager.FindByIdAsync(student.UserId);
            var RoleId = currentUser.Roles.SingleOrDefault().RoleId;
            var RoleName = db.Roles.SingleOrDefault(r => r.Id == RoleId).Name;
            await UserManager.RemoveFromRoleAsync(student.UserId, RoleName);

            db.Students.Remove(student);
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
