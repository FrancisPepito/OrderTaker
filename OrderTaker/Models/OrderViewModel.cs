using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderTaker.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<Sku> SkuList { get; set; }
        public List<Customer> CustomerList { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
