using Health_Booking_MVC.Models;
using Health_Booking_MVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Health_Booking_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public HomeController(HealthBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new BookingViewModel();

            model.Hospitals = await _context.Hospitals
                .OrderBy(h => h.Name)
                .ToListAsync();

            model.Specializations = await _context.Specializations
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}