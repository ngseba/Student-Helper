namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_correction : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Events", "TeacherCoursesID_TeacherCoursesID", "dbo.TeacherCourses");
            DropForeignKey("dbo.Events", "TeacherCourses_TeacherCoursesID", "dbo.TeacherCourses");
            DropIndex("dbo.Events", new[] { "TeacherCourses_TeacherCoursesID" });
            DropIndex("dbo.Events", new[] { "TeacherCoursesID_TeacherCoursesID" });
            RenameColumn(table: "dbo.Events", name: "TeacherCourses_TeacherCoursesID", newName: "TeacherCoursesID");
            AlterColumn("dbo.Events", "TeacherCoursesID", c => c.Int(nullable: false));
            CreateIndex("dbo.Events", "TeacherCoursesID");
            AddForeignKey("dbo.Events", "TeacherCoursesID", "dbo.TeacherCourses", "TeacherCoursesID", cascadeDelete: true);
            DropColumn("dbo.Events", "TeacherCoursesID_TeacherCoursesID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "TeacherCoursesID_TeacherCoursesID", c => c.Int());
            DropForeignKey("dbo.Events", "TeacherCoursesID", "dbo.TeacherCourses");
            DropIndex("dbo.Events", new[] { "TeacherCoursesID" });
            AlterColumn("dbo.Events", "TeacherCoursesID", c => c.Int());
            RenameColumn(table: "dbo.Events", name: "TeacherCoursesID", newName: "TeacherCourses_TeacherCoursesID");
            CreateIndex("dbo.Events", "TeacherCoursesID_TeacherCoursesID");
            CreateIndex("dbo.Events", "TeacherCourses_TeacherCoursesID");
            AddForeignKey("dbo.Events", "TeacherCourses_TeacherCoursesID", "dbo.TeacherCourses", "TeacherCoursesID");
            AddForeignKey("dbo.Events", "TeacherCoursesID_TeacherCoursesID", "dbo.TeacherCourses", "TeacherCoursesID");
        }
    }
}
