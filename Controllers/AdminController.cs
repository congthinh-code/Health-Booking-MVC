using Microsoft.AspNetCore.Mvc;

namespace Health_Booking_MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
