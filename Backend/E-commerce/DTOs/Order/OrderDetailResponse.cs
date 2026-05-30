namespace E_commerce.DTOs.Order
{
    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int OrderQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalPrice { get; internal set; }
    }
}
