using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health_Booking_MVC.Controllers
{
    public class DVYTController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public DVYTController(HealthBookingDbContext context)
        {
            _context = context;
        }

        public IActionResult ĐKBS(int? id)
        {
            return View();
        }

        public IActionResult ĐKCS()
        {
            return View();
        }

        public IActionResult ĐKCK()
        {
            return View();
        }

        public IActionResult ĐKNG()
        {
            return View();
        }

        public IActionResult TTVP()
        {
            return View();
        }
    }
}