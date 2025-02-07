using FluentMigrator;
using SkeinBuddy.Migrations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Migrations.Migrations
{
    [Migration(202502042214)]
    public class AddYarnNameEmbeddingsTable : Migration
    {
        public override void Up()
        {
            Execute.Sql("CREATE TABLE yarn_name_embedding (yarn_id uuid PRIMARY KEY REFERENCES yarn (id), embedding vector(384))");
        }

        public override void Down()
        {
            Delete.Table("yarn_name_embedding");
        }
    }
}
