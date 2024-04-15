using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetNewAsync();
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByCustomerIdAsync(int id);
        Task UpdateAsync(int id, bool orderStatus);
    }
}
