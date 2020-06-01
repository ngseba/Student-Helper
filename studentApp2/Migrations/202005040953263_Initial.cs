namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseDepartments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseID = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        CourseYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseID);
            
            CreateTable(
                "dbo.TeacherCourses",
                c => new
                    {
                        TeacherCoursesID = c.Int(nullable: false, identity: true),
                        CourseID = c.Int(nullable: false),
                        TeacherID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TeacherCoursesID)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.TeacherID);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        TeacherId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TeacherId);
            
            CreateTable(
                "dbo.TeacherCoursesGroups",
                c => new
                    {
                        TeacherCoursesGroupID = c.Int(nullable: false, identity: true),
                        TeacherCoursesID = c.Int(nullable: false),
                        GroupID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TeacherCoursesGroupID)
                .ForeignKey("dbo.Groups", t => t.GroupID, cascadeDelete: true)
                .ForeignKey("dbo.TeacherCourses", t => t.TeacherCoursesID, cascadeDelete: true)
                .Index(t => t.TeacherCoursesID)
                .Index(t => t.GroupID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupID = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false),
                        DepartmentID = c.Int(nullable: false),
                        YearOfStudy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID, cascadeDelete: true)
                .Index(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentId = c.Int(nullable: false, identity: true),
                        GroupID = c.Int(),
                        UserId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StudentId)
                .ForeignKey("dbo.Groups", t => t.GroupID)
                .Index(t => t.GroupID);
            
            CreateTable(
                "dbo.GeneralPosts",
                c => new
                    {
                        GeneralPostID = c.Int(nullable: false, identity: true),
                        PostID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GeneralPostID)
                .ForeignKey("dbo.Posts", t => t.PostID, cascadeDelete: true)
                .Index(t => t.PostID);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostID = c.Int(nullable: false, identity: true),
                        PostTitle = c.String(),
                        PostBody = c.String(),
                        PostType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PostID);
            
            CreateTable(
                "dbo.GroupPosts",
                c => new
                    {
                        GroupPostID = c.Int(nullable: false, identity: true),
                        PostID = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupPostID)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostID, cascadeDelete: true)
                .Index(t => t.PostID)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Firstname = c.String(nullable: false),
                        Lastname = c.String(nullable: false),
                        PhoneNo = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.YearPosts",
                c => new
                    {
                        YearPostId = c.Int(nullable: false, identity: true),
                        YearPostYear = c.Int(nullable: false),
                        PostID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.YearPostId)
                .ForeignKey("dbo.Posts", t => t.PostID, cascadeDelete: true)
                .Index(t => t.PostID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YearPosts", "PostID", "dbo.Posts");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.GroupPosts", "PostID", "dbo.Posts");
            DropForeignKey("dbo.GroupPosts", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GeneralPosts", "PostID", "dbo.Posts");
            DropForeignKey("dbo.TeacherCoursesGroups", "TeacherCoursesID", "dbo.TeacherCourses");
            DropForeignKey("dbo.TeacherCoursesGroups", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.Students", "GroupID", "dbo.Groups");
            DropForeignKey("dbo.Groups", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.CourseDepartments", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.TeacherCourses", "TeacherID", "dbo.Teachers");
            DropForeignKey("dbo.TeacherCourses", "CourseID", "dbo.Courses");
            DropForeignKey("dbo.CourseDepartments", "CourseID", "dbo.Courses");
            DropIndex("dbo.YearPosts", new[] { "PostID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.GroupPosts", new[] { "GroupId" });
            DropIndex("dbo.GroupPosts", new[] { "PostID" });
            DropIndex("dbo.GeneralPosts", new[] { "PostID" });
            DropIndex("dbo.Students", new[] { "GroupID" });
            DropIndex("dbo.Groups", new[] { "DepartmentID" });
            DropIndex("dbo.TeacherCoursesGroups", new[] { "GroupID" });
            DropIndex("dbo.TeacherCoursesGroups", new[] { "TeacherCoursesID" });
            DropIndex("dbo.TeacherCourses", new[] { "TeacherID" });
            DropIndex("dbo.TeacherCourses", new[] { "CourseID" });
            DropIndex("dbo.CourseDepartments", new[] { "DepartmentID" });
            DropIndex("dbo.CourseDepartments", new[] { "CourseID" });
            DropTable("dbo.YearPosts");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.GroupPosts");
            DropTable("dbo.Posts");
            DropTable("dbo.GeneralPosts");
            DropTable("dbo.Students");
            DropTable("dbo.Departments");
            DropTable("dbo.Groups");
            DropTable("dbo.TeacherCoursesGroups");
            DropTable("dbo.Teachers");
            DropTable("dbo.TeacherCourses");
            DropTable("dbo.Courses");
            DropTable("dbo.CourseDepartments");
        }
    }
}
