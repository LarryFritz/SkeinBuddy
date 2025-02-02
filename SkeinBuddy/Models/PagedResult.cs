using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Records { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 1000;
    }
}
