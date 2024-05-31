using Microsoft.EntityFrameworkCore;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public class EFProductImageRepository : IProductImageRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProductImage>> GetImagesByProductIdAsync(int productId)
        {
            return await _context.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync();
        }
    }
}
