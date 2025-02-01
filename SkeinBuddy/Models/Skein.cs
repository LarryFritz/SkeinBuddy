using SkeinBuddy.Enumerations;

namespace SkeinBuddy.Models
{
    public class Skein : Base
    {
        public string Color { get; set; } = null!;
        public double Weight { get; set; }
        public string WeightUom { get; set; } = null!;
        public double Length { get; set; }
        public string LengthUom { get; set; } = null!;

        public string Upc { get; set; } = null!;
        public string LotNumber { get; set; } = null!;
        public Guid YarnId { get; set; }
    }

    public class SkeinDetails : Skein
    {
        public Yarn Yarn { get; set; } = null!;
    }
}
