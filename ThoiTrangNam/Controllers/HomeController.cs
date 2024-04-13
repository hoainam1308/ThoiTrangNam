using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Channels;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

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
        public async Task<IActionResult> Test()
        {
            return View();
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
