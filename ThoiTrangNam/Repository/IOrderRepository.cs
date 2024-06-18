using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetNewAsync();
        Task<IEnumerable<Order>> GetByUserAsync(string userId);
        Task<IEnumerable<Order>> GetNeedNotificationToUserAsync(string userId);
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByCustomerIdAsync(string id);
        Task UpdateAsync(int id, bool orderStatus);
    }
}
