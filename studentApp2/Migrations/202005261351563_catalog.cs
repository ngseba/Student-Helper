namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class catalog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Catalogs",
                c => new
                    {
                        GradeID = c.Int(nullable: false, identity: true),
                        StudentID = c.Int(nullable: false),
                        CourseID = c.Int(nullable: false),
                        Grade = c.Int(nullable: false),
                        GradeDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GradeID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Catalogs");
        }
    }
}
