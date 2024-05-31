using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int id);
        Task UpdateReview(int id, int? rating, string? review);
    }
}
