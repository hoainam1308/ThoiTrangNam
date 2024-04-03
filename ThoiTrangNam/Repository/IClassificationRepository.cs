using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IClassificationRepository
    {
        Task<IEnumerable<Classification>> GetAllAsync();
        Task<Classification> GetByIdAsync(int id);
        Task AddAsync(Classification classification);
        Task UpdateAsync(Classification classification);
        Task DeleteAsync(int id);
    }
}
