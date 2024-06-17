using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByQueryAsync(string query);
        Task<IEnumerable<Product>> OrderByPriceDesc();
        Task<IEnumerable<Product>> OrderByPriceAsc();
        Task<IEnumerable<Product>> GetByCateIdAsync(int id);
        Task<IEnumerable<Product>> GetByClassifiIdAsync(int id);
        Task<IEnumerable<Product>> GetSomeAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task DecreaseQuantityAsync(int productId, int quantity);
    }
}
