using E_commerce.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.DTOs.Order
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public Guid UserId { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string PaymentMethodName { get; set; } = null!;
        public string? VoucherCode { get; set; }
        public List<OrderDetailResponse> Items { get; internal set; }
    }
}
