using E_commerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Order
{
    public class UpdateOrderStatusRequest
    {
        [Required(ErrorMessage = "Order status is required.")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid order status.")]
        public OrderStatus OrderStatus { get; set; }
    }
}
