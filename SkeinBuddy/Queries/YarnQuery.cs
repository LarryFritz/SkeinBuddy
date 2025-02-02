using SkeinBuddy.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.Queries
{
    public class YarnQuery : BaseQuery
    {
        public Guid? BrandId { get; set; }
        public YarnWeight? Weight { get; set; }
    }
}
