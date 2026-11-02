using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PresentGlama.Models;
using System.Diagnostics;

namespace PresentGlama.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GlammaOptions _glammaOptions;

        public HomeController(ILogger<HomeController> logger, IOptions<GlammaOptions> config)
        {
            _logger = logger;
            _glammaOptions = config.Value;
        }

        public IActionResult Index()
        {
            ViewBag.Title = _glammaOptions.DefaultTitle;
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
