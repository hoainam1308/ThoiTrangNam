using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IConverter _converter;
        private readonly ITempDataProvider _tempDataProvider;
        public OrderController(IOrderRepository orderRepository, IProductRepository productRepository, IProductImageRepository productImageRepository, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider, IConverter converter)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _viewEngine = viewEngine;
            _converter = converter;
            _tempDataProvider = tempDataProvider;
        }
        public async Task<IActionResult> Index(string searchString, DateTime? fromDate, DateTime? toDate)
        {
            var orders = await _orderRepository.GetAllAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    o.PhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fromDate != null && toDate != null)
            {
                orders = orders.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).ToList();
            }

            return View("Index", orders); // Đổi tên view thành "Index"
        }
        public async Task<IActionResult> IndexNew(string searchString, DateTime? fromDate, DateTime? toDate)
        {
            var orders = await _orderRepository.GetNewAsync();
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    o.PhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fromDate != null && toDate != null)
            {
                orders = orders.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).ToList();
            }
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
        public IActionResult ExportWholePagePdf(string searchString, DateTime? fromDate, DateTime? toDate)
        {
            var orders = _orderRepository.GetAllAsync().Result;

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    o.PhoneNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fromDate != null && toDate != null)
            {
                orders = orders.Where(o => o.OrderDate >= fromDate && o.OrderDate <= toDate).ToList();
            }

            var htmlContent = RenderPartialViewToString("_OrdersPdf", orders);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait,
        },
                Objects = {
            new ObjectSettings() {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Footer" }
            }
        }
            };

            var pdf = _converter.Convert(doc);

            return File(pdf, "application/pdf", $"Orders.pdf");
        }

        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    new TempDataDictionary(ControllerContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult(); // Đã thêm GetAwaiter().GetResult() để chờ render hoàn tất.
                return sw.GetStringBuilder().ToString();
            }
        }


        public async Task<IActionResult> ExportInvoicePdf(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            foreach (var item in order.OrderDetails)
            {
                item.Product = await _productRepository.GetByIdAsync(item.ProductId);
            }

            var htmlContent = await this.RenderViewAsync("Invoice", order);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Footer" }
                    }
                }
            };

            var pdf = _converter.Convert(doc);

            return File(pdf, "application/pdf", $"Invoice_{id}.pdf");
        }

        private async Task<string> RenderViewAsync(string viewName, object model)
        {
            var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
            if (!viewResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view '{viewName}'");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(ControllerContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

    }
}
