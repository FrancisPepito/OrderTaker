namespace OrderTaker.Models
{
    public class Sku : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string UnitPrice { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
    }
}
