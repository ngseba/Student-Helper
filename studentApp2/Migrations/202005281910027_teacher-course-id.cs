namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class teachercourseid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "TeacherCoursesID_TeacherCoursesID", c => c.Int());
            CreateIndex("dbo.Events", "TeacherCoursesID_TeacherCoursesID");
            AddForeignKey("dbo.Events", "TeacherCoursesID_TeacherCoursesID", "dbo.TeacherCourses", "TeacherCoursesID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "TeacherCoursesID_TeacherCoursesID", "dbo.TeacherCourses");
            DropIndex("dbo.Events", new[] { "TeacherCoursesID_TeacherCoursesID" });
            DropColumn("dbo.Events", "TeacherCoursesID_TeacherCoursesID");
        }
    }
}
