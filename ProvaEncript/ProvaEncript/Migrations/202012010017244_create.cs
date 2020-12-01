namespace ProvaEncript.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MensagemModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mensagem = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MensagemModels");
        }
    }
}
