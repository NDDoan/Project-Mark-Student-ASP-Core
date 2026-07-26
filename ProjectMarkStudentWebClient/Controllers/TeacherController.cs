using Microsoft.AspNetCore.Mvc;

namespace ProjectMarkStudentWebClient.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult MarkEntry()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
