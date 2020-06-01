namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tcunique : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.TeacherCourses", new[] { "CourseID" });
            DropIndex("dbo.TeacherCourses", new[] { "TeacherID" });
            CreateIndex("dbo.TeacherCourses", new[] { "CourseID", "TeacherID" }, name: "UNIQUE_Teacher_Course");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TeacherCourses", "UNIQUE_Teacher_Course");
            CreateIndex("dbo.TeacherCourses", "TeacherID");
            CreateIndex("dbo.TeacherCourses", "CourseID");
        }
    }
}
