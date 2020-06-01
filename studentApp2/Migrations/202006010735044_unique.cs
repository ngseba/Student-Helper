namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TeacherCourses", "UNIQUE_Teacher_Course");
            DropIndex("dbo.TeacherCoursesGroups", "UNIQUE_Teacher_Course_Group");
            CreateIndex("dbo.TeacherCourses", new[] { "CourseID", "TeacherID" }, unique: true, name: "UNIQUE_Teacher_Course");
            CreateIndex("dbo.TeacherCoursesGroups", "GroupID", unique: true, name: "UNIQUE_Teacher_Course_Group");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TeacherCoursesGroups", "UNIQUE_Teacher_Course_Group");
            DropIndex("dbo.TeacherCourses", "UNIQUE_Teacher_Course");
            CreateIndex("dbo.TeacherCoursesGroups", "GroupID", name: "UNIQUE_Teacher_Course_Group");
            CreateIndex("dbo.TeacherCourses", new[] { "CourseID", "TeacherID" }, name: "UNIQUE_Teacher_Course");
        }
    }
}
