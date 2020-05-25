using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using studentApp2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace studentApp2.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private  ApplicationUserManager _userManager;
        ApplicationDbContext context = new ApplicationDbContext();

        public AdminController() { }
        public AdminController(ApplicationUserManager userManager)
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

        // GET: Admin
        public ActionResult Index()
        {
            return View(this.getUsersRoles());
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = this.getUserRoleById(id);
            if (user== null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,Username,Email,PhoneNo,Role")] UsersRolesViewModel model)
        {
            try
            {
                // TODO: Add update logic here
                var currentUser = UserManager.FindById(model.UserId);
                currentUser.UserName = model.Username;
                currentUser.Email = model.Email;
                currentUser.PhoneNo = model.PhoneNo;
                await UserManager.UpdateAsync(currentUser);
                string oldRoleName = "";
                if (UserManager.IsInRole(model.UserId, "Student") || UserManager.IsInRole(model.UserId, "Admin") || UserManager.IsInRole(model.UserId, "Teacher"))
                {

                    var oldRoleId = currentUser.Roles.SingleOrDefault().RoleId;
                    oldRoleName = context.Roles.SingleOrDefault(r => r.Id == oldRoleId).Name;
                }
                else
                {
                    oldRoleName = "gol";
                }
                var userRoles = new List<string> { "Admin", "Teacher", "Student" };

                if (model.Role != oldRoleName && oldRoleName != "gol")
                {
                    if (userRoles.Contains(model.Role))
                    {
                        UserManager.RemoveFromRole(model.UserId, oldRoleName);
                        UserManager.AddToRole(model.UserId, model.Role);
                        
                    }
                }
                else if (oldRoleName == "gol")
                {
                    if (userRoles.Contains(model.Role))
                    {
                        UserManager.AddToRole(model.UserId, model.Role);
                    }
                }

                if (model.Role == "Student")
                {
                    if (oldRoleName == "Teacher")
                    {
                        var teacher = (from t in context.Teachers
                                      where t.UserId == model.UserId
                                      select t).FirstOrDefault<Teacher>();
                        context.Teachers.Remove(teacher);
                        context.SaveChanges();
                    }
                    var student = new Student { UserId = model.UserId };
                    context.Students.Add(student);
                    context.SaveChanges();
                }
                else if (model.Role == "Teacher")
                {
                    if (oldRoleName == "Student")
                    {
                        var student = (from t in context.Students
                                       where t.UserId == model.UserId
                                       select t).FirstOrDefault<Student>();
                        context.Students.Remove(student);
                        context.SaveChanges();
                    }
                    var teacher = new Teacher { UserId = model.UserId };
                    context.Teachers.Add(teacher);
                    context.SaveChanges();
                }
                else if(model.Role == "Admin")
                {
                    if (oldRoleName == "Student")
                    {
                        var student = (from t in context.Students
                                       where t.UserId == model.UserId
                                       select t).FirstOrDefault<Student>();
                        context.Students.Remove(student);
                        context.SaveChanges();
                    }
                    if (oldRoleName == "Teacher")
                    {
                        var teacher = (from t in context.Teachers
                                       where t.UserId == model.UserId
                                       select t).FirstOrDefault<Teacher>();
                        context.Teachers.Remove(teacher);
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = this.getUserRoleById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete([Bind(Include = "UserId,Username,Email,Role")] UsersRolesViewModel model)
        {
            if(model.UserId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var user = await UserManager.FindByIdAsync(model.UserId);
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    this.removeFromRelativeTables(model.UserId, model.Role);
                    TempData["UserDeleted"] = "User Successfully Deleted";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["UserDeleted"] = "Error Deleting User";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        private void removeFromRelativeTables(string userId, string role)
        {
           if(role == "Student")
            {
                var student = (from t in context.Students
                               where t.UserId == userId
                               select t).FirstOrDefault<Student>();
                context.Students.Remove(student);
                context.SaveChanges();
            }
           else if(role == "Teacher")
            {
                var teacher= (from t in context.Teachers
                               where t.UserId == userId
                               select t).FirstOrDefault<Teacher>();
                context.Teachers.Remove(teacher);
                context.SaveChanges();
            }
        }

        private IEnumerable<UsersRolesViewModel> getUsersRoles()
        {
                var usersWithRoles = (from user in context.Users
                                      select new
                                      {
                                          UserId = user.Id,
                                          Username = user.UserName,
                                          Email = user.Email,
                                          RoleNames = (from userRole in user.Roles
                                                       join role in context.Roles on userRole.RoleId
                                                       equals role.Id
                                                       select role.Name).ToList()
                                      }).ToList().Select(p => new UsersRolesViewModel()

                                      {
                                          UserId = p.UserId,
                                          Username = p.Username,
                                          Email = p.Email,
                                          Role = string.Join(",", p.RoleNames)
                                      });
                return usersWithRoles;
        
        }

        private UsersRolesViewModel getUserRoleById(string id)
        {
            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindById(id);
            UsersRolesViewModel user = new UsersRolesViewModel();
            user.UserId = appUser.Id;
            user.Username = appUser.UserName;
            user.Email = appUser.Email;
            user.Role = UserManager.GetRoles(id).FirstOrDefault();
            return user;
        }
    }
}
