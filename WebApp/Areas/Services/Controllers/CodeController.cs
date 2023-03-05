using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Services.Controllers
{
    public class CodeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
