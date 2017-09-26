namespace ContosoBankChatbot.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BankAccounts", "CreatedTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BankAccounts", "CreatedTime", c => c.DateTime(nullable: false));
        }
    }
}
