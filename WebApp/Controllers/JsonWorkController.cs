using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    public class JsonWorkController : Controller
    {
        [Route("/fis")]
        public async Task<IActionResult> Index()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "response.json");
            var bill = new Bill(path);
            var rows = bill.GetRows();
            return View(rows);
        }     
    }
}
