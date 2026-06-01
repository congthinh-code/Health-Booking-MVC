using Microsoft.AspNetCore.Mvc;

namespace Health_Booking_MVC.Controllers
{
    public class CSYTController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BVCong()
        {
            return View();
        }

        public IActionResult BVTu()
        {
            return View();
        }
    }
}