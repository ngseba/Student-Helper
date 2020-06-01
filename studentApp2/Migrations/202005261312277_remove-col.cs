namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removecol : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Departments", "TestCol");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "TestCol", c => c.String());
        }
    }
}
