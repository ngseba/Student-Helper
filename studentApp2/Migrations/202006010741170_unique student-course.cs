namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniquestudentcourse : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Catalogs", new[] { "StudentID", "CourseID" }, unique: true, name: "UNIQUE_Student_Course");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Catalogs", "UNIQUE_Student_Course");
        }
    }
}
