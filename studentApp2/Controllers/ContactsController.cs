using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using studentApp2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace studentApp2.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ContactsController() { }
        public ContactsController(ApplicationUserManager userManager)
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

        [Authorize]
        // GET: Contacts
        public ActionResult Index()
        {
            var currentUser = User.Identity.GetUserId();
            if (UserManager.IsInRole(currentUser, "Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            var contacts = new List<ApplicationUser>();
            if (UserManager.IsInRole(currentUser, "Teacher"))
            {
                var teacher = db.Teachers.First(s => s.UserId == currentUser);
                //Get list of courses the teacher has
                var tclist = db.TeacherCourses.Where(tc => tc.TeacherID == teacher.TeacherId).ToList();
                foreach(var tc in tclist)
                {
                    var tcg = db.TeacherCoursesGroups.Where(c => c.TeacherCoursesID == tc.TeacherCoursesID).ToList();
                    foreach(var c in tcg)
                    {
                        var students = db.Students.Where(s => s.GroupID == c.GroupID).ToList();
                        foreach(var s in students)
                        {
                            contacts.Add(db.Users.First(u => u.Id == s.UserId));
                        }
                    }
                }
            }
            if (UserManager.IsInRole(currentUser, "Student"))
            {
                //Get group colleagues
                var stud = db.Students.First(s => s.UserId == currentUser);
                var slist = db.Students.Where(st => st.GroupID == stud.GroupID && st.StudentId != stud.StudentId).ToList();
                foreach (var item in slist) 
                {
                    contacts.Add(db.Users.First(u=>u.Id == item.UserId));
                }

                //Get teachers that teach to current student's group
                var tcglist = db.TeacherCoursesGroups.Where(tcg => tcg.GroupID == stud.GroupID).ToList();
                foreach(var item in tcglist)
                {
                    var tc = db.TeacherCourses.First(t => t.TeacherCoursesID == item.TeacherCoursesID).TeacherID;
                    var teacher = db.Teachers.First(t => t.TeacherId == tc);
                    contacts.Add(db.Users.First(u => u.Id == teacher.UserId));
                }
            }
            ViewBag.Contacts = contacts;
            return View();
        }
    }
}