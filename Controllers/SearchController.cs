using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health_Booking_MVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public SearchController(HealthBookingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSearch(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(new List<object>());

            q = q.Trim();

            var doctors = _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .Where(d =>
                    d.FullName.Contains(q) ||
                    d.Description.Contains(q) ||
                    d.Specialization.Name.Contains(q) ||
                    d.Hospital.Name.Contains(q))
                .Select(d => new
                {
                    type = "doctor",
                    id = d.DoctorId,
                    title = d.FullName, 
                    subtitle = d.Specialization.Name,
                    url = "/DVYT/ĐKBS?doctorId=" + d.DoctorId
                })
                .Take(10)
                .ToList();

            var hospitals = _context.Hospitals
                .Where(h =>
                    h.Name.Contains(q) ||
                    h.Address.Contains(q))
                .Select(h => new
                {
                    type = "hospital",
                    id = h.HospitalId,
                    title = h.Name,
                    subtitle = h.Address,
                    url = "/DVYT/ĐKCS"
                })
                .Take(5)
                .ToList();

            var specializations = _context.Specializations
                .Where(s => s.Name.Contains(q))
                .Select(s => new
                {
                    type = "specialization",
                    id = s.SpecializationId,
                    title = s.Name,
                    subtitle = "Chuyên khoa",
                    url = "/DVYT/ĐKCK"
                })
                .Take(5)
                .ToList();

            var results = new List<object>();
            results.AddRange(doctors);
            results.AddRange(hospitals);
            results.AddRange(specializations);

            return Json(results);
        }
    }
}