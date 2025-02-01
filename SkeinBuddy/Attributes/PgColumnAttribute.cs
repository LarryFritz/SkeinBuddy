using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PgColumnAttribute : Attribute
    {
        public string ColumnName { get; } = null!;
        public PgColumnAttribute(string columnName) => ColumnName = columnName;
    }
}
