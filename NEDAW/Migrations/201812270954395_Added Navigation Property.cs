namespace NEDAW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNavigationProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.News", "UserId");
            AddForeignKey("dbo.News", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.News", "ModifiedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.News", "ModifiedBy", c => c.Guid(nullable: false));
            DropForeignKey("dbo.News", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.News", new[] { "UserId" });
            DropColumn("dbo.News", "UserId");
        }
    }
}
