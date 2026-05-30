namespace E_commerce.DTOs.ShippingAddress
{
    public class ShippingAddressDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string Street { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}
