using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
   public class CouponsController : Controller
    {
        private readonly ICouponRepository _couponRepository;

        public CouponsController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        // GET: Coupons
        public async Task<IActionResult> Index()
        {
            var coupons = await _couponRepository.GetAllCouponsAsync();
            return View(coupons);
        }

        // GET: Coupons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,DiscountAmount,IsPercentage,ExpirationDate,UsageLimit,TimesUsed")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                await _couponRepository.AddCouponAsync(coupon);
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Coupons/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponRepository.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,DiscountAmount,IsPercentage,ExpirationDate,UsageLimit,TimesUsed")] Coupon coupon)
        {
            if (id != coupon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _couponRepository.UpdateCouponAsync(coupon);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CouponExists(coupon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        // GET: Coupons/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponRepository.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _couponRepository.DeleteCouponAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CouponExists(int id)
        {
            var coupon = await _couponRepository.GetCouponByIdAsync(id);
            return coupon != null;
        }
    }
}
