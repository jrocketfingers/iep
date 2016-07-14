namespace iep_ecommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBidCreatedAt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bids", "CreatedAt", c => c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Bids", "CreatedAt");
        }
    }
}
