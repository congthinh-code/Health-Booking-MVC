using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health_Booking_MVC.Controllers
{
    public class CSYTController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public CSYTController(HealthBookingDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> BVCong()
        {
            var dsBenhVien = await _context.Hospitals
                .Where(h => h.Description == "Bệnh viện công")
                .ToListAsync();

            return View(dsBenhVien);
        }
        public async Task<IActionResult> BVTu()
        {
            var dsBenhVien = await _context.Hospitals
                .Where(h => h.Description == "Bệnh viện tư")
                .ToListAsync();

            return View(dsBenhVien);
        }

    }
}