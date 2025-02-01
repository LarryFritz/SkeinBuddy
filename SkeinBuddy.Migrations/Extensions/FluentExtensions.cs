using FluentMigrator.Builders.Create.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Migrations.Extensions
{
    public static class FluentExtensions
    {

        public static ICreateTableWithColumnSyntax WithBaseColumns(this ICreateTableWithColumnOrSchemaOrDescriptionSyntax table)
        {
            return table
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("created_at").AsDateTime().NotNullable()
                .WithColumn("updated_at").AsDateTime().Nullable();
        }
    }
}
