using Microsoft.AspNetCore.Mvc;
using Health_Booking_MVC.Models;
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

        public IActionResult ĐKCS()
        {
            var hospitals = _context.Hospitals.ToList();
            ViewBag.Specialties = _context.Specializations.ToList();
            return View(hospitals);
        }

        public IActionResult ĐKCK()
        {
            var specialties = _context.Specializations.ToList();
            return View(specialties);
        }

        public IActionResult ĐKBS()
        {
            var doctors = _context.Doctors.Include(d => d.Specialization).ToList();
            return View(doctors);
        }

        public IActionResult ĐKNG()
        {
            ViewBag.Hospitals = _context.Hospitals.ToList();
            ViewBag.Doctors = _context.Doctors.ToList();
            return View();
        }

        public IActionResult TTVP()
        {
            var hospitals = _context.Hospitals.ToList();
            return View(hospitals);
        }
    }
}