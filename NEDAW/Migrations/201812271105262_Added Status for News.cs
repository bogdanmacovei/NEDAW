namespace NEDAW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStatusforNews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "Status");
        }
    }
}
