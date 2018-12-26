namespace NEDAW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeNewsModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.News", "CreatedBy");
            DropColumn("dbo.News", "ModifiedBy");
            AddColumn("dbo.News", "CreatedBy", c => c.Guid(nullable: false));
            AddColumn("dbo.News", "ModifiedBy", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "CreatedBy");
            DropColumn("dbo.News", "ModifiedBy");

            AddColumn("dbo.News", "CreatedBy", c => c.Int(nullable: false));
            AddColumn("dbo.News", "ModifiedBy", c => c.Int(nullable: false));
        }
    }
}
