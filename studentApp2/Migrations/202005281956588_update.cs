namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Teachers", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Teachers", "UserId");
            AddForeignKey("dbo.Teachers", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teachers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Teachers", new[] { "UserId" });
            AlterColumn("dbo.Teachers", "UserId", c => c.String(nullable: false));
        }
    }
}
