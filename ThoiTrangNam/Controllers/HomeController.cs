using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Channels;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ThoiTrangNam.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IClassificationRepository _classificationRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public string search = "";
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, IClassificationRepository classificationRepository, IProductImageRepository productImageRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _classificationRepository = classificationRepository;
            _productImageRepository = productImageRepository;
            _context = context;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        public async Task<IActionResult> IndexSome()
        {
            var products = await _productRepository.GetSomeAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Images = await _productImageRepository.GetImagesByProductIdAsync(id);
            ProductDetailViewModel productDetailViewModel = new ProductDetailViewModel();
            var orderDetails = await _orderDetailRepository.GetByProductIdAsync(id);
            List<ReviewViewModel> reviewViewModels = new List<ReviewViewModel>();
            foreach(var orderDetail in orderDetails)
            {
                ReviewViewModel reviewViewModel = new ReviewViewModel();
                reviewViewModel.OrderDetailId = orderDetail.Id;
                reviewViewModel.UserName = orderDetail.Order.CustomerName;
                reviewViewModel.UserReview = orderDetail.Review;
                reviewViewModel.Rating = orderDetail.Rating;
                reviewViewModel.ReviewDate = orderDetail.ReviewDate;
                reviewViewModels.Add(reviewViewModel);
            }
            productDetailViewModel.Product = product;
            productDetailViewModel.Reviews = reviewViewModels;
            return View(productDetailViewModel);
        }
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContactUs(SendMailDTO sendMailDTO)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(sendMailDTO.Email, sendMailDTO.Password);
                    string subject = sendMailDTO.Name + " " + sendMailDTO.Subject;
                    MailMessage message = new MailMessage(sendMailDTO.Email, StaticClass.MyEmail, subject, sendMailDTO.Message);
                    client.Send(message);
                    ViewBag.Message = "Mail Send";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View();
            }
        }
        public async Task<IActionResult> Shop(string query = "null", int orderBy = 0, int cate = 0, int classifi = 0, int currentPage = 1)
        {
            var proView = new ProductViewModel();
            if (!string.IsNullOrEmpty(query) && query!="null")
            {
                search = StaticClass.LocDau(query);
            }
            var products = await _context.Products.Include(p => p.Category).Where(p => p.RemovedDiacriticsName.Contains(search)).ToListAsync();
            switch (orderBy)
            {
                case 1:
                    products = products.OrderBy(p => p.SellPrice).ToList();
                    break;
                case 2:
                    products = products.OrderByDescending(p => p.SellPrice).ToList();
                    break;
                default:
                    break;
            }
            if(cate > 0)
            {
                products = products.Where(p => p.CategoryId == cate).ToList();
            }
            if (classifi > 0)
            {
                products = products.Where(p => p.Category.ClassificationId == classifi).ToList();
            }
            int totalRecords = products.Count();
            int pageSize = 9;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            products = products.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            proView.Products = products;
            proView.CurrentPage = currentPage;
            proView.PageSize = pageSize;
            proView.TotalPages = totalPages;
            return View(proView);
        }
        public async Task<IActionResult> CategoryPartial()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }
        public async Task<IActionResult> OrderList()
        {
            var user = await _userManager.GetUserAsync(User);
            var orderList = await _orderRepository.GetByUserAsync(user.Id);
            return View(orderList);
        }
        public async Task<IActionResult> UserConfirm(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            bool userConfirm = true;
            await _orderRepository.UpdateAsync(orderId, userConfirm);
            foreach (var itemt in order.OrderDetails)
            {
                itemt.Product = await _productRepository.GetByIdAsync(itemt.ProductId);
            }
            return View("OrderDetail", order);
        }
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
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
        public async Task<IActionResult> Review(int orderDetailId)
        {
            ReviewViewModel review = new ReviewViewModel();
            review.OrderDetailId = orderDetailId;
            return View(review);
        }
        [HttpPost]
        public async Task<IActionResult> Review(ReviewViewModel review)
        {
            if (ModelState.IsValid)
            {
                await _orderDetailRepository.UpdateReview(review.OrderDetailId, review.Rating, review.UserReview);
                return RedirectToAction(nameof(Index));
            }
            var orderDetail = await _orderDetailRepository.GetByIdAsync(review.OrderDetailId);
            return View(orderDetail);
        }
        public async Task<IActionResult> Test()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
