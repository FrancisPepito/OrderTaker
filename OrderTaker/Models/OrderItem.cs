namespace OrderTaker.Models
{
    public class OrderItem : BaseEntity
    {
        public Sku Sku { get; set; }
        public int SkuId { get; set; } 
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
