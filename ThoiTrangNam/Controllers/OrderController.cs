using Microsoft.AspNetCore.Mvc;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
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
    }
}
