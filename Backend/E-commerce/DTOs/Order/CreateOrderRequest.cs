using E_commerce.Models;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.DTOs.Order

{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "ShippingAddressId is required.")]
        public Guid ShippingAddressId { get; set; }

        [Required(ErrorMessage = "PaymentMethodId is required.")]
        public Guid PaymentMethodId { get; set; }

        public string? VoucherCode { get; set; }

        // Nếu có, chỉ thanh toán những item này. Nếu null => toàn bộ cart.
        public List<Guid>? CartItemIds { get; set; }
    }
}