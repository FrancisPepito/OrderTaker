namespace OrderTaker.Models
{
    public class Order : BaseEntity
    {
        public Customer Customer { get; set; } = null!;
        public int CustomerId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Status { get; set; } = null!;
        public string TotalAmount { get; set; } = null!; 
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
