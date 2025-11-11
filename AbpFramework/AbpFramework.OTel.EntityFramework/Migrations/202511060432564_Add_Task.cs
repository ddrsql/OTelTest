namespace AbpFramework.OTel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Task : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppTasks",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        AssignedUserId = c.Long(nullable: false),
                        Description = c.String(unicode: false),
                        CreationTime = c.DateTime(nullable: false, precision: 0),
                        State = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AbpUsers", t => t.AssignedUserId, cascadeDelete: true)
                .Index(t => t.AssignedUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AppTasks", "AssignedUserId", "dbo.AbpUsers");
            DropIndex("dbo.AppTasks", new[] { "AssignedUserId" });
            DropTable("dbo.AppTasks");
        }
    }
}
