using Microsoft.EntityFrameworkCore;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public class EFClassificationRepository : IClassificationRepository
    {
        private readonly ApplicationDbContext _context;
        public EFClassificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Classification classification)
        {
            _context.Classifications.Add(classification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var classification = await _context.Classifications.FindAsync(id);
            _context.Classifications.Remove(classification);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            return await _context.Classifications.ToListAsync();
        }

        public IEnumerable<Classification> GetAll()
        {
            return  _context.Classifications.ToList();
        }

        public async Task<Classification> GetByIdAsync(int id)
        {
            return await _context.Classifications.SingleOrDefaultAsync(x => x.ClassificationId == id);
        }

        public async Task UpdateAsync(Classification classification)
        {
            _context.Classifications.Update(classification);
            await _context.SaveChangesAsync();
        }
    }
}
