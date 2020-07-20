namespace ReadLater.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserToCategoriesAndBookmarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "UserId", c => c.String());
            AddColumn("dbo.Bookmarks", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bookmarks", "UserId");
            DropColumn("dbo.Categories", "UserId");
        }
    }
}
