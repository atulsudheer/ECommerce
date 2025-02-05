namespace ECommerce.Model
{
    public class Orders
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? Orderdate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string? DeliveryExpected { get; set; }
        public bool ContainsGift { get; set; }
    }
    public class OrderItem
    {
        public string? OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
