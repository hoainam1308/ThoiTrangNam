using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(x => x.Category).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetSomeAsync()
        {
            return await _context.Products.Include(x => x.Category).Take(5).ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(x => x.Category).SingleOrDefaultAsync(x => x.ProductId == id);
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Product>> GetByCateIdAsync(int id)
        {
            return await _context.Products
            .Where(pi => pi.CategoryId == id)
            .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByClassifiIdAsync(int id)
        {
            return await _context.Products
            .Where(pi => pi.Category.ClassificationId == id)
            .ToListAsync();
        }

        public async Task<IEnumerable<Product>> OrderByPriceAsc()
        {
            return await _context.Products.Include(x => x.Category).OrderBy(x => x.SellPrice).ToListAsync();
        }

        public async Task<IEnumerable<Product>> OrderByPriceDesc()
        {
            return await _context.Products
                        .Include(x => x.Category)
                        .OrderByDescending(x => x.SellPrice)
                        .ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByQueryAsync(string query)
        {
            string queryStr = StaticClass.LocDau(query);
            return await _context.Products
                        .Include(x => x.Category)
                        .Where(x => x.RemovedDiacriticsName.Contains(queryStr))
                        .ToListAsync();
        }
        public async Task UpdateRating(int id, int rating)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (product.QuantityRating != null)
                {
                    product.AvgRating = (product.AvgRating * product.QuantityRating + rating) / (float)(++product.QuantityRating);
                }
                else
                {
                    product.QuantityRating = 1;
                    product.AvgRating = (float)rating;
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task DecreaseQuantityAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.Quantity -= quantity;
                await _context.SaveChangesAsync();
            }
        }

    }
}
