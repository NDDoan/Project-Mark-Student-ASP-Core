using Microsoft.AspNetCore.Mvc;

namespace ProjectMarkStudentWebClient.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
