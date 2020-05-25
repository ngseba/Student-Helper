using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace studentApp2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        public string PhoneNo { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<studentApp2.Models.Student> Students { get; set; }
        public System.Data.Entity.DbSet<studentApp2.Models.Teacher> Teachers { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.Department> Departments { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.Course> Courses { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.CourseDepartment> CourseDepartments { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.TeacherCourses> TeacherCourses { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.TeacherCoursesGroup> TeacherCoursesGroups { get; set; }
        public System.Data.Entity.DbSet<studentApp2.Models.Post> Posts { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.YearPost> YearPosts { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.GroupPost> GroupPosts { get; set; }

        public System.Data.Entity.DbSet<studentApp2.Models.GeneralPost> GeneralPosts { get; set; }
    }
}