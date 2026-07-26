using Microsoft.AspNetCore.Mvc;

namespace ProjectMarkStudentWebClient.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Students()
        {
            return View();
        }

        public IActionResult Subjects()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        public IActionResult GradeItems()
        {
            return View();
        }

        public IActionResult Assign()
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
