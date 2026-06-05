using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Health_Booking_MVC.Controllers
{
    public class DVYTController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public DVYTController(HealthBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ĐKBS(int? id)
        {

            var doctors = await _context.Doctors
    .Include(d => d.Specialization)
    .Include(d => d.Hospital)
    .ToListAsync();

            return View(doctors);
        }

       
        public async Task<IActionResult> ĐKCS()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            return View(hospitals);
        }

       
        public async Task<IActionResult> ĐKCK()
        {
            var specializations = await _context.Specializations.ToListAsync();
            return View(specializations);
        }


        public async Task<IActionResult> ĐKNG()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            // Lấy danh sách bác sĩ kèm chuyên khoa và cơ sở y tế
            ViewBag.Doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .ToListAsync();
            return View(hospitals);
        }


        public async Task<IActionResult> TTVP()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            return View(hospitals);
        }
    }
}