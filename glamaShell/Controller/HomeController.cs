using Microsoft.AspNetCore.Mvc;

namespace glamaShell.Controller
{
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
