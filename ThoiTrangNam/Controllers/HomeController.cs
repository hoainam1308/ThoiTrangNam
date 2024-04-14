using Microsoft.AspNetCore.Mvc;
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
        public HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository, IClassificationRepository classificationRepository, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _classificationRepository = classificationRepository;
            _productImageRepository = productImageRepository;
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
            return View(product);
        }
        public async Task<IActionResult> IndexByCate(int id)
        {
            var products = await _productRepository.GetByCateIdAsync(id);
            return View("Index", products);
        }
        public async Task<IActionResult> IndexByClassifi(int id)
        {
            var products = await _productRepository.GetByClassifiIdAsync(id);
            return View("Index", products);
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

                    MailMessage message = new MailMessage(sendMailDTO.Email, StaticClass.MyEmail, sendMailDTO.Subject, sendMailDTO.Message);
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
        public async Task<IActionResult> Shop()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        public async Task<IActionResult> ShopDesc()
        {
            var products = await _productRepository.OrderByPriceDesc();
            return View("Shop", products);
        }
        public async Task<IActionResult> ShopAsc()
        {
            var products = await _productRepository.OrderByPriceAsc();
            return View("Shop", products);
        }
        public async Task<IActionResult> ShopSearch(string query)
        {
            var products = await _productRepository.GetByQueryAsync(query);
            return View("Shop", products);
        }
        public async Task<IActionResult> CategoryPartial()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
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
