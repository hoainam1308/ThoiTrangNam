using Microsoft.AspNetCore.Mvc;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Repository
{
    public interface ICouponRepository
    {
        Task<IEnumerable<Coupon>> GetAllCouponsAsync();
        Task<Coupon> GetCouponByIdAsync(int id);
        Task AddCouponAsync(Coupon coupon);
        Task UpdateCouponAsync(Coupon coupon);
        Task DeleteCouponAsync(int id);
    }
}
