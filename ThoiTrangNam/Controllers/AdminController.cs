using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentYear = DateTime.Now.Year;
            var totalUsers = await _context.Users.CountAsync();
            // Tính tổng số tiền trong năm
            var annualEarnings = await _context.Orders
                .Where(o => o.OrderDate.Year == currentYear)
                .SumAsync(o => o.TotalPrice);

            // Tính tổng số tiền trong tháng
            var currentMonth = DateTime.Now.Month;
            var monthlyEarnings = await _context.Orders
                .Where(o => o.OrderDate.Month == currentMonth && o.OrderDate.Year == currentYear)

                .SumAsync(o => o.TotalPrice);
            // Tính tổng lợi nhuận từ số lượng sản phẩm đã bán
            var totalProfit = await _context.OrderDetails
                .Include(od => od.Product)
                .SumAsync(od => (od.Product.SellPrice - od.Product.BuyPrice) * od.Quantity);

            ViewBag.AnnualEarnings = annualEarnings;
            ViewBag.MonthlyEarnings = monthlyEarnings;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProfit = totalProfit;


            return View();
        }
        
    }
}
