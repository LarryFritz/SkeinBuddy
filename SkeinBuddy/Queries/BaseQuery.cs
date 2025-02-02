using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Queries
{
    public class BaseQuery
    {
        public Guid? Id { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 1000;
        public string? Search {  get; set; }
    }
}
