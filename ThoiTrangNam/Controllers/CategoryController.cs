using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ThoiTrangNam.Models;
using ThoiTrangNam.Repository;

namespace ThoiTrangNam.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IClassificationRepository _classificationRepository;
        public CategoryController(ICategoryRepository categoryRepository, IClassificationRepository classificationRepository)
        {
            _categoryRepository = categoryRepository;
            _classificationRepository = classificationRepository;
        }
        public async Task<IActionResult> Create()
        {
            var classifications = await _classificationRepository.GetAllAsync();
            ViewBag.Classifications = new SelectList(classifications, "ClassificationId", "ClassificationName");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            var classifications = await _classificationRepository.GetAllAsync();
            ViewBag.Classifications = new SelectList(classifications, "ClassificationId", "ClassificationName");
            return View(category);
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }
    }
}
