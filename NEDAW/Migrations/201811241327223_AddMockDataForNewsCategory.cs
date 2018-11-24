namespace NEDAW.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMockDataForNewsCategory : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[NewsCategories] ([Name]) VALUES (N'Entertainment')");
        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM [dbo].[NewsCategories] WHERE [Name] = N'Entertainment'");
        }
    }
}
