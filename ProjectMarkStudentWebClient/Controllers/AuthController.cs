using Microsoft.AspNetCore.Mvc;

namespace ProjectMarkStudentWebClient.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
