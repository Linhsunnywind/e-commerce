using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce.Models
{
    public class ProductImage
    {
        [Key]
        public Guid ProductImageId {set; get;} = Guid.NewGuid();

        public Guid ProductId {get; set;}
        [ForeignKey("ProductId")]
        public Product? Product {get; set;}

        [Required]
        public string ImageUrl {get; set;} = null!;
        //  public string? PublicId { get; set; }
    }
}
