namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "TestCol", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "TestCol");
        }
    }
}
