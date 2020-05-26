namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testcolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "TestColumn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "TestColumn");
        }
    }
}
