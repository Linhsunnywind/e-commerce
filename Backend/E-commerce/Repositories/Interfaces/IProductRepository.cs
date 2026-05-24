using E_commerce.Data;
using E_commerce.Models;

namespace E_commerce.Repositories.Interfaces
{
    public interface IProductRepository
    {
        // trả về đối tượng có khả năng truy vấn nhưng chưa thực thi xuống database
        IQueryable<Product> GetProductsQuery();
        // tìm kiếm sản phẩm dựa trên khóa chính 
        Task<Product?> GetById(Guid id);
        // thêm sản phẩm mới 
        Task AddProduct(Product product);
        // sản phẩm được update
        void UpdateProduct(Product product);
        // xóa sản phẩm
        void DeleteProduct(Product product);
        // cập nhật biến thể trực tiếp (bypass EF change tracking)
        Task UpdateVariantAsync(Guid id, string name, decimal price, int quantity);
        // thêm biến thể mới trực tiếp bằng raw SQL
        Task InsertVariantAsync(Guid variantId, Guid productId, string name, decimal price, int quantity);
        // xóa biến thể bằng ExecuteDeleteAsync
        Task DeleteVariantsAsync(List<Guid> variantIds);
        // xóa biến thể của sản phẩm (EF tracking - dùng cho images)
        void RemoveVariants(List<ProductVariant> variants);
        // Xóa danh sách các hình ảnh đính kèm của sản phẩm
        void RemoveImages(List<ProductImage> images);
        // đóng gói lại các chỉnh sửa và lưu vào database
        Task<bool> SaveChanges();
    }

}
