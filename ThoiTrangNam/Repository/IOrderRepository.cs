using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task UpdateAsync(bool orderStatus);
    }
}
