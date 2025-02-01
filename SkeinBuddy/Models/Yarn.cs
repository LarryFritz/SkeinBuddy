using SkeinBuddy.Attributes;
using SkeinBuddy.Enumerations;

namespace SkeinBuddy.Models
{
    public class Yarn : Base
    {
        public Guid BrandId { get; set; }
        
        public YarnWeight Weight { get; set; }
        public string Name { get; set; } = null!;
    }
}
