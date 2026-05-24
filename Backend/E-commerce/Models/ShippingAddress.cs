using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class ShippingAddress
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required, StringLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required, StringLength(100)]
        public string Province { get; set; } = null!;      // Tỉnh / Thành phố

        [Required, StringLength(100)]
        public string District { get; set; } = null!;      // Quận / Huyện

        [Required, StringLength(100)]
        public string Ward { get; set; } = null!;          // Phường / Xã

        [Required, StringLength(255)]
        public string Street { get; set; } = null!;        // Số nhà, tên đường

        public bool IsDefault { get; set; } = false;
    }
}
