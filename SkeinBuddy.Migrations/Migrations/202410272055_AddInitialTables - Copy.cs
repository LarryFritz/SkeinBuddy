using FluentMigrator;
using SkeinBuddy.Migrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Migrations.Migrations
{
    [Migration(202410272055)]
    public class AddInitialTables : Migration
    {
        public override void Up()
        {
            Execute.Sql(@"
                CREATE EXTENSION IF NOT EXISTS vector;

                GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA public to skein_buddy_api;
                ALTER DEFAULT PRIVILEGES FOR ROLE skein_buddy_api IN SCHEMA public GRANT SELECT, UPDATE, INSERT, DELETE ON TABLES TO skein_buddy_api;
                ALTER DATABASE skein_buddy_dev SET TIMEZONE TO 'UTC';
            ");

            Create.Table("brand")
                .WithBaseColumns()
                .WithColumn("name").AsString().NotNullable();

            Create.Table("yarn")
                .WithBaseColumns()
                .WithColumn("weight").AsInt32().NotNullable()
                .WithColumn("name").AsString().NotNullable()
                .WithColumn("brand_id").AsGuid().NotNullable().ForeignKey("brand", "id");

            Create.Table("skein")
                .WithBaseColumns()
                .WithColumn("color").AsString().NotNullable()
                .WithColumn("weight").AsDouble().NotNullable()
                .WithColumn("weight_uom").AsString().NotNullable()
                .WithColumn("length").AsDouble().NotNullable()
                .WithColumn("length_uom").AsString().NotNullable()
                .WithColumn("upc").AsString().NotNullable().Unique()                
                .WithColumn("yarn_id").AsGuid().NotNullable().ForeignKey("yarn", "id");

            Create.Table("user")
                .WithBaseColumns()
                .WithColumn("username").AsString().NotNullable().Unique();

            Create.Table("user_skein")
                .WithBaseColumns()
                .WithColumn("skein_id").AsGuid().NotNullable().ForeignKey("skein", "id")
                .WithColumn("user_id").AsGuid().NotNullable().ForeignKey("user", "id")
                .WithColumn("lot_number").AsString().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("user_skein");
            Delete.Table("user");
            Delete.Table("skein");
            Delete.Table("yarn");
            Delete.Table("brand");
        }
    }
}
