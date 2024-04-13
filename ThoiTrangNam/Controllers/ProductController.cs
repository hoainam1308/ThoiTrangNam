using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {      
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IClassificationRepository _classificationRepository;
        private readonly IProductImageRepository _productImageRepository;
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IClassificationRepository classificationRepository, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _classificationRepository = classificationRepository;
            _productImageRepository = productImageRepository;
        }
        /*
        public async Task<IActionResult> Home()
        {
            var products = await _productRepository.GetAllAsync();
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
        */
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        
        // Hien thi form them san pham mdi
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var classifications = await _classificationRepository.GetAllAsync();
            ViewBag.Classifications = new SelectList(classifications, "ClassificationId", "ClassificationName");
            return View();
        }
        // X0 ly thém san pham moi
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile imageUrl, List<IFormFile> Images)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                if(Images.Count > 0)
                {
                    product.Images = await SaveImages(Images);
                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            var classifications = await _classificationRepository.GetAllAsync();
            ViewBag.Classifications = new SelectList(classifications, "ClassificationId", "ClassificationName");

            return View(product);
        }
        private async Task<string> SaveImage(IFormFile image)
        {

            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }
        private async Task<List<ProductImage>> SaveImages(List<IFormFile> imageFiles)
        {
            var images = new List<ProductImage>();
            foreach (var imageFile in imageFiles)
            {
                var imageUrl = await SaveImage(imageFile);
                images.Add(new ProductImage { Url = imageUrl });
            }
            return images;
        }

        // Hién thi thong tin chi tiet san pham
        

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Images = await _productImageRepository.GetImagesByProductIdAsync(id);
            return View(product);
        }

        // Hién thi form cap nhét San pham
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // XU my cap nhét San phan
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // LUu hinh énh dai dién
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                await _productRepository.UpdateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
        // Hién thi form xac nhén XOa San pham
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // XU l9 xoa san pham
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
