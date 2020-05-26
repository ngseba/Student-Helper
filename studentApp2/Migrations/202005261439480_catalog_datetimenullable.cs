namespace studentApp2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class catalog_datetimenullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Catalogs", "GradeDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Catalogs", "GradeDate", c => c.DateTime(nullable: false));
        }
    }
}
