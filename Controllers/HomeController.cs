using Health_Booking_MVC.Models;
using Health_Booking_MVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq; // Đảm bảo có dòng này để chạy được các hàm đếm, lọc

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
        [HttpGet]
        public IActionResult GetFormAppointments()
        {
            // Lấy UserId từ Session (đảm bảo key "UserId" khớp với bên AccountController)
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var patient = _context.Patients.FirstOrDefault(p => p.UserId == userId);
            if (patient == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ bệnh nhân" });
            }

            // Lọc theo PatientId lấy từ bảng Patients
            // Đảm bảo dùng patient.PatientId để khớp với Model Appointment
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patient.PatientId) // SỬA ĐÚNG TÊN Ở ĐÂY
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new {
                    appointmentId = a.AppointmentId,
                    doctorName = (a.DoctorId == 1 || a.Doctor == null) ? "Chờ phân công" : a.Doctor.FullName,
                    appointmentDate = a.AppointmentDate.ToString("dd/MM/yyyy - HH:mm"),
                    status = (int)a.Status
                })
                .ToList();
            return Json(new { success = true, data = appointments });
        }
    }
}