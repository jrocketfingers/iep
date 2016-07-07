namespace iep_ecommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuctions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        StartingPrice = c.Double(nullable: false),
                        Duration = c.Long(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        OpenedAt = c.DateTime(nullable: false),
                        ClosedAt = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Auction_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Auctions", t => t.Auction_Id)
                .Index(t => t.Auction_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bids", "Auction_Id", "dbo.Auctions");
            DropIndex("dbo.Bids", new[] { "Auction_Id" });
            DropTable("dbo.Bids");
            DropTable("dbo.Auctions");
        }
    }
}
