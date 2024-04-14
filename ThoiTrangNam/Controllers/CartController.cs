using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using ThoiTrangNam.Extensions;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public Cart? Cart { get; set; }
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var product = await GetProductFromDatabase(productId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetObjectFromJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(product, 1);
                HttpContext.Session.SetObjectAsJson("cart", Cart);
            }
            return View("Cart", Cart);
        }
        public async Task<IActionResult> AddMoreToCart(int productId, int num)
        {
            var product = await GetProductFromDatabase(productId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetObjectFromJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(product, num);
                HttpContext.Session.SetObjectAsJson("cart", Cart);
            }
            return View("Cart", Cart);
        }
        public async Task<IActionResult> RemoveAProductFromCart(int productId)
        {
            // Giả sử bạn có phương thức lấy thông tin sản phẩm từ productId
            var product = await GetProductFromDatabase(productId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetObjectFromJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(product, -1);
                HttpContext.Session.SetObjectAsJson("cart", Cart);
            }
            return View("Cart", Cart);
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            return View("Cart", cart);
        }
        
        // Các actions khác...
        private async Task<Product> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }
        public IActionResult RemoveFromCart(int productId)
        {
            Product? product = _context.Products.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                Cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
                Cart.RemoveItem(product);
                HttpContext.Session.SetObjectAsJson("cart", Cart);
            }
            return View("Cart", Cart);
        }
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            if (cart == null || cart.Items.Count == 0)
            {
                // Xử lý khi giỏ hàng trống
                return RedirectToAction("Index", "Home");
            }
            return View(new Order());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            if (cart == null || cart.Items.Count == 0)
            {
                // Xử lý khi giỏ hàng trống
                return RedirectToAction("Index", "Home");
            }
            
            // Lưu thông tin đơn hàng
            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.SubTotal = cart.ComputeToTalValue();
            order.TotalPrice = cart.ComputeToTotal();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            // Lưu các mục trong giỏ hàng thành các OrderDetail
            foreach (var item in cart.Items)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.Product.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product.SellPrice
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();
            // Xóa giỏ hàng sau khi đã thanh toán
            HttpContext.Session.Remove("cart");
            // Chuyển hướng đến trang xác nhận đơn hàng
            return RedirectToAction("OrderCompleted", new { orderId = order.Id });
        }
        public IActionResult OrderCompleted(int orderId)
        {
            return View(orderId);
        }

    }
}
