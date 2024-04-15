using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        public OrderController(IOrderRepository orderRepository, IProductRepository productRepository, IProductImageRepository productImageRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }
        public async Task<IActionResult> IndexNew()
        {
            var orders = await _orderRepository.GetNewAsync();
            return View("Index", orders);
        }
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            foreach (var itemt in order.OrderDetails) 
            {
                itemt.Product = await _productRepository.GetByIdAsync(itemt.ProductId);
            }
            return View(order);
        }
        public async Task<IActionResult> ShopConfirm(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            bool shopConfirm = false;
            await _orderRepository.UpdateAsync(id, shopConfirm);
            return RedirectToAction(nameof(Index));
        }
    }
}
