using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Runtime.CompilerServices;
using ThoiTrangNam.Extensions;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;
using Microsoft.DiaSymReader;
using ThoiTrangNam.Helper;
using Microsoft.VisualBasic;
using static Azure.Core.HttpHeader;

namespace ThoiTrangNam.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public Cart? Cart { get; set; }
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVnPayService _vnpayRepository;
        private readonly PaypalClient _paypalClient;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IProductRepository productRepository, IOrderRepository orderRepository, IVnPayService vnpayRepository, PaypalClient paypalClient)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _vnpayRepository = vnpayRepository;
            _paypalClient = paypalClient;
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
            ViewBag.PaypalClientdId = _paypalClient.ClientId;
            return View(new Order());
        }
        [HttpPost]
        public IActionResult ApplyCoupon(string couponCode)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode);

            if (coupon == null)
            {
                TempData["Message"] = "Coupon không tồn tại.";
                return RedirectToAction("Index");
            }

            if (coupon.ExpirationDate < DateTime.Now)
            {
                TempData["Message"] = "Coupon đã hết hạn.";
                return RedirectToAction("Index");
            }

            if (coupon.UsageLimit <= coupon.TimesUsed)
            {
                TempData["Message"] = "Coupon đã hết lượt sử dụng.";
                return RedirectToAction("Index");
            }

            // Áp dụng coupon vào giỏ hàng
            cart.ApplyCoupon(coupon);
            HttpContext.Session.SetObjectAsJson("cart", cart);
            TempData["DiscountMessage"] = $"Đã giảm: {coupon.DiscountAmount.ToString("#,##0")}%";

            TempData["Message"] = "Áp dụng coupon thành công.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            cart.RemoveCoupon();
            HttpContext.Session.SetObjectAsJson("cart", cart);

            TempData["Message"] = "Đã xóa coupon.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string payment = "Dat hang(COD)")
        {
          
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            if (cart == null || cart.Items.Count == 0)
            {
                // Xử lý khi giỏ hàng trống
                return RedirectToAction("Index", "Home");
            }
            if (cart.AppliedCoupon != null)
            {
                var appliedCoupon = _context.Coupons.FirstOrDefault(c => c.Id == cart.AppliedCoupon.Id);
                if (appliedCoupon != null)
                {
                    appliedCoupon.TimesUsed++;
                    await _context.SaveChangesAsync();
                }
            }
           
            // Lưu thông tin đơn hàng
            var user = await _userManager.GetUserAsync(User);
           
            if (payment == "Thanh toan VNPAY")
            {
                var vnPayModel = new VnPaymentResquestModel
                {
                    Amount = (double)cart.ComputeToTotal(),
                    CreatedDate = DateTime.Now,
                    Description = "Thanh toán đơn hàng",
                    FullName = user.UserName,
                    OrderId = new Random().Next(1000, 10000)
                    
                };
                return Redirect(_vnpayRepository.CreatePaymentUrl(vnPayModel, HttpContext));
            }
            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.SubTotal = cart.ComputeToTalValue();
            order.TotalPrice = cart.ComputeToTotal();
            TempData["OrderNotes"] = order.Notes;

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
                await _productRepository.DecreaseQuantityAsync(item.Product.ProductId, item.Quantity);
            }
            await _context.SaveChangesAsync();
            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(StaticClass.FromEmail, StaticClass.Password);
                string subject = "Đơn hàng mới";
                string infor = "Tên khách hàng: " + order.CustomerName + "\nĐịa chỉ: " + order.ShippingAddress + "\nSĐT: " + order.PhoneNumber + "\nSố tiền: " + order.TotalPrice.ToString("#,##0") + " VNĐ";
                MailMessage message = new MailMessage(StaticClass.FromEmail, StaticClass.MyEmail, subject, infor);
                client.Send(message);
                ViewBag.Message = "Mail Send";
            }
            // Xóa giỏ hàng sau khi đã thanh toán
            HttpContext.Session.Remove("cart");
            // Chuyển hướng đến trang xác nhận đơn hàng
            return RedirectToAction("OrderCompleted", new { orderId = order.Id });
        }
        public IActionResult OrderCompleted(int orderId)
        {
            return View(orderId);
        }
        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnpayRepository.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Loi thanh toan VNPAY: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            //
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            if (cart == null || cart.Items.Count == 0)
            {
                // Xử lý khi giỏ hàng trống
                return RedirectToAction("Index", "Home");
            }

            // Tạo đơn hàng mới và lưu vào cơ sở dữ liệu
                var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                SubTotal = cart.ComputeToTalValue(),
                TotalPrice = cart.ComputeToTotal(),
                CustomerName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                ShippingAddress = user.Address,
                IsPaymented = true,
                Notes = TempData["OrderNotes"] as string
                // Thêm các thông tin khác của đơn hàng nếu cần
            };

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
                await _productRepository.DecreaseQuantityAsync(item.Product.ProductId, item.Quantity);
            }
            await _context.SaveChangesAsync();

            // Xóa giỏ hàng sau khi đã thanh toán
            HttpContext.Session.Remove("cart");

            // Gửi email xác nhận đơn hàng nếu cần
            using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(StaticClass.FromEmail, StaticClass.Password);
                string subject = "Đơn hàng mới";
                string infor = "Tên khách hàng: " + order.CustomerName + "\nĐịa chỉ: " + order.ShippingAddress + "\nSĐT: " + order.PhoneNumber + "\nSố tiền: " + order.TotalPrice.ToString("#,##0") + " VNĐ";
                MailMessage message = new MailMessage(StaticClass.FromEmail, StaticClass.MyEmail, subject, infor);
                client.Send(message);
                ViewBag.Message = "Mail Sent";
            }

            // Chuyển hướng đến trang xác nhận đơn hàng
            TempData["Message"] = "Thanh toán thành công";
            return RedirectToAction("OrderCompleted", new { orderId = order.Id });
        }
        #region Paypal payment
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
            // Thông tin đơn hàng gửi qua Paypal
            var tongTien = (cart.ComputeToTotal()/24000).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<Cart>("cart");
                var response = await _paypalClient.CaptureOrder(orderID);

                if (cart == null || cart.Items.Count == 0)
                {
                    // Xử lý khi giỏ hàng trống
                    return RedirectToAction("Index", "Home");
                }

                // Tạo đơn hàng mới và lưu vào cơ sở dữ liệu
                var user = await _userManager.GetUserAsync(User);
                var order = new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    SubTotal = cart.ComputeToTalValue(),
                    TotalPrice = cart.ComputeToTotal(),
                    CustomerName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    ShippingAddress = user.Address,
                    IsPaymented = true,
                    Notes = TempData["OrderNotes"] as string
                    // Thêm các thông tin khác của đơn hàng nếu cần
                };
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
                    await _productRepository.DecreaseQuantityAsync(item.Product.ProductId, item.Quantity);
                }
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đã thanh toán
                HttpContext.Session.Remove("cart");

                // Gửi email xác nhận đơn hàng nếu cần
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(StaticClass.FromEmail, StaticClass.Password);
                    string subject = "Đơn hàng mới";
                    string infor = "Tên khách hàng: " + order.CustomerName + "\nĐịa chỉ: " + order.ShippingAddress + "\nSĐT: " + order.PhoneNumber + "\nSố tiền: " + order.TotalPrice.ToString("#,##0") + " VNĐ";
                    MailMessage message = new MailMessage(StaticClass.FromEmail, StaticClass.MyEmail, subject, infor);
                    client.Send(message);
                    ViewBag.Message = "Mail Sent";
                }

                // Chuyển hướng đến trang xác nhận đơn hàng
                TempData["Message"] = "Thanh toán thành công";
                return RedirectToAction("OrderCompleted", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion
    }
}
