using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IProductImageRepository
    {
        Task<List<ProductImage>> GetImagesByProductIdAsync(int id);
    }
}
