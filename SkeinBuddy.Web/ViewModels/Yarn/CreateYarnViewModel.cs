using SkeinBuddy.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace SkeinBuddy.Web.ViewModels.Yarn
{
    public class CreateYarnViewModel
    {
        [Required]
        public YarnWeight Weight { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public Guid BrandId { get; set; }
    }
}
