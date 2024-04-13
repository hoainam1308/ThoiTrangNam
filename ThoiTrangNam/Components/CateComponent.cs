using Microsoft.AspNetCore.Mvc;
using ThoiTrangNam.Extensions;
using ThoiTrangNam.Models;

namespace ThoiTrangNam.Components
{
    public class CateComponent: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetObjectFromJson<Category>("cate"));
        }
    }
}
