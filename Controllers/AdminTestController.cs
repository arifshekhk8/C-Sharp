using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminTestController : Controller
    {
        public IActionResult Index()
        {
            return Content("✅ You are an ADMIN");
        }
    }
}