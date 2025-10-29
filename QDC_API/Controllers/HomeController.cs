using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QDC_API.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            //var indexPath = Path.Combine(wwwRootPath, @"index.html", "text/HTML");

            return PhysicalFile(Path.Combine(wwwRootPath, "Index.html"), "text/HTML");
        }
    }
}
