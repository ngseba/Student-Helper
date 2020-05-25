using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using studentApp2.Models;

namespace studentApp2.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public PostsController() { }
        public PostsController(ApplicationUserManager userManager)
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
        // GET: Posts
        public ActionResult Index()
        {
            var cu = User.Identity.GetUserId();
            if (UserManager.IsInRole(cu, "Student") || UserManager.IsInRole(cu, "Teacher"))
            {
                return RedirectToAction("Newsfeed");
            }
            var posts = db.Posts.ToList();
            var pvm = new List<PostViewModel>();
            foreach(var post in posts)
            {
                pvm.Add(new PostViewModel
                {
                    Post = post,
                });
                if (post.PostType == PostType.General)
                {
                    pvm.Last().GroupName = "All users";
                }
                else if (post.PostType == PostType.Year)
                {
                    pvm.Last().GroupName = db.YearPosts.First(pid => pid.PostID == post.PostID).YearPostYear.ToString();
                }
                else if (post.PostType == PostType.Group)
                {
                    pvm.Last().GroupName = db.GroupPosts.First(pid => pid.PostID == post.PostID).Group.GroupName;
                }
            }
            ViewBag.posts = pvm;
            return View("Index");
        }

        [Authorize(Roles = "Admin")]
        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [Authorize]
        // GET: Posts/Create
        public ActionResult Create()
        {
            var GroupList = db.Groups.ToList();
            ViewBag.GroupList = new SelectList(GroupList, "GroupID", "GroupName");
            return View();
        }

        [Authorize]
        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PostViewModel postViewModel)
        {
            postViewModel.Post.PostType = postViewModel.PostType;
            if (ModelState.IsValid)
            {
                db.Posts.Add(postViewModel.Post);
                switch (postViewModel.Post.PostType)
                {
                    case PostType.General:
                        return CreateGeneralPost(postViewModel);
                    case PostType.Year:
                        return CreateYearPost(postViewModel);
                    case PostType.Group:
                        return CreateGroupPost(postViewModel);
                }
                if (postViewModel.PostType == PostType.General)

                    return RedirectToAction("Index");
            }
            return View(postViewModel);
        }

        [Authorize]
        public ActionResult CreateGeneralPost(PostViewModel postViewModel)
        {
            GeneralPost generalPost = new GeneralPost() { Post = postViewModel.Post };
            db.GeneralPosts.Add(generalPost);
            db.SaveChanges();
            return RedirectToAction("Index", new { area = "GeneralPosts" });
        }

        [Authorize]
        public ActionResult CreateYearPost(PostViewModel postViewModel)
        {
            YearPost yearPost = new YearPost() { Post = postViewModel.Post,  YearPostYear = postViewModel.PostYear };
            db.YearPosts.Add(yearPost);
            db.SaveChanges();
            return RedirectToAction("Index", new { area = "YearPosts" });
        }

        [Authorize]
        public ActionResult CreateGroupPost(PostViewModel postViewModel)
        {
            GroupPost groupPost = new GroupPost() { Post = postViewModel.Post,  GroupId = postViewModel.GroupID };
            db.GroupPosts.Add(groupPost);
            db.SaveChanges();
            return RedirectToAction("Index", new { area = "GroupPosts" });
        }

        [Authorize(Roles = "Admin")]
        // GET: Posts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostID,PostBody,PostTitle")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [Authorize(Roles="Admin")]
        // GET: Posts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        [Authorize]
        public ActionResult Newsfeed()
        {
            var posts = new List<Post>();
            var currentUser = User.Identity.GetUserId();
            //REDIRECT TO ADMIN PANEL
            if (UserManager.IsInRole(currentUser, "Admin"))
            {
                return RedirectToAction("Index");
            }

            //TEACHER NEWSFEED
            if (UserManager.IsInRole(currentUser, "Teacher"))
            {
                var teacher = db.Teachers.First(t => t.UserId == currentUser);
                var teacherCourses = db.TeacherCourses.Where(tc=>tc.TeacherID==teacher.TeacherId).ToList();
                foreach(var tc in teacherCourses)
                {
                    var tcgr = db.TeacherCoursesGroups.Where(tcg => tcg.TeacherCoursesID == tc.TeacherCoursesID).ToList();
                    foreach(var group in tcgr)
                    {

                        var groupP = db.GroupPosts.Where(gp => gp.GroupId == group.GroupID).ToList();
                        foreach (var post in groupP)
                        {
                            var po = db.Posts.Where(p => p.PostType == PostType.Group && p.PostID == post.PostID).ToList();
                            posts.AddRange(po);
                        }

                    }
                }
                var genPosts = db.Posts.Where(p => p.PostType == PostType.General).ToList();
                posts.AddRange(genPosts);
            }

            //STUDENT NEWSFEED
            if (UserManager.IsInRole(currentUser, "Student"))
            {
                var student = db.Students.First(s => s.UserId == currentUser);
                var group = db.Groups.First(g => g.GroupID == student.GroupID);

                var genPosts = db.Posts.Where(p => p.PostType == PostType.General).ToList();
                posts.AddRange(genPosts);

                var groupP = db.GroupPosts.Where(gp => gp.GroupId == group.GroupID).ToList();
                foreach (var post in groupP) 
                {
                    var po = db.Posts.Where(p => p.PostType == PostType.Group && p.PostID == post.PostID).ToList();
                    posts.AddRange(po);
                }

                var yearP = db.YearPosts.Where(yp => yp.YearPostYear == group.YearOfStudy).ToList();
                foreach (var post in yearP)
                {
                    var po = db.Posts.Where(p => p.PostType == PostType.Year && p.PostID == post.PostID).ToList();
                    posts.AddRange(po);
                }
            }
            ViewBag.Posts = posts;
            return View("Newsfeed");
        }

        [Authorize(Roles ="Admin")]
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
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
