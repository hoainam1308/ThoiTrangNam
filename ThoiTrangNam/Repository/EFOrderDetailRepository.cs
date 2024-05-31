using Microsoft.EntityFrameworkCore;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public class EFOrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public EFOrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OrderDetail> GetByIdAsync(int id)
        {
            return await _context.OrderDetails
                                 .Include(od => od.Order)
                                 .Include(od => od.Product)
                                 .SingleOrDefaultAsync(od => od.Id == id);
        }
        public async Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int id)
        {
            return await _context.OrderDetails
                                 .Include(od => od.Order)
                                 .Include(od => od.Product)
                                 .Where(od => od.ProductId == id).ToListAsync();
        }
        public async Task UpdateReview(int id, int? rating, string? review)
        {
            var detail = await _context.OrderDetails
                                 .Include(od => od.Order)
                                 .Include(od => od.Product)
                                 .SingleOrDefaultAsync(od => od.Id == id);
            detail.ReviewDate = DateTime.Now;
            detail.Rating = rating;
            detail.Review = review;
            await _context.SaveChangesAsync();
        }
    }
}
