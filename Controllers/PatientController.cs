using Microsoft.EntityFrameworkCore;
using Health_Booking_MVC.Models;
using Health_Booking_MVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Hosting;

namespace Health_Booking_MVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PatientController(
            HealthBookingDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Profile(string status = "all")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Status = status;

            var patient = _context.Patients
                .FirstOrDefault(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound();
            }

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            // Lấy danh sách lịch hẹn
            var appointments = _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patient.PatientId
                            && a.BookingSource == "Doctor")
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            var bookingRequests = _context.Appointments
                .Include(a => a.Hospital)
                .Include(a => a.Specialization)
                .Where(a => a.PatientId == patient.PatientId
                         && a.BookingSource == "Home")
                .ToList();

            ViewBag.BookingRequests = bookingRequests;
            ViewBag.Appointments = appointments;

            // Bộ lọc trạng thái
            if (status == "pending")
            {
                appointments = appointments
                    .Where(a => a.Status == AppointmentStatus.Pending)
                    .ToList();
            }
            else if (status == "completed")
            {
                appointments = appointments
                    .Where(a => a.Status == AppointmentStatus.Completed
                             || a.Status == AppointmentStatus.Confirmed)
                    .ToList();
            }
            else if (status == "cancelled")
            {
                appointments = appointments
                    .Where(a => a.Status == AppointmentStatus.Cancelled)
                    .ToList();
            }

            // Thống kê
            var allAppointments = _context.Appointments
                .Where(a => a.PatientId == patient.PatientId)
                .ToList();

            ViewBag.TotalAppointments = allAppointments.Count;

            ViewBag.PendingAppointments =
                allAppointments.Count(a =>
                    a.Status == AppointmentStatus.Pending);

            ViewBag.CompletedAppointments =
                allAppointments.Count(a =>
                    a.Status == AppointmentStatus.Completed ||
                    a.Status == AppointmentStatus.Confirmed);

            ViewBag.CancelledAppointments =
                allAppointments.Count(a =>
                    a.Status == AppointmentStatus.Cancelled);

            // Danh sách lịch hẹn gửi sang View
            ViewBag.Appointments = appointments;

            var model = new PatientProfileViewModel
            {
                UserId = patient.UserId,
                FullName = patient.FullName,
                Email = user.Email,
                Phone = patient.Phone,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Avatar = patient.Avatar
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = _context.Patients
                .FirstOrDefault(p => p.UserId == userId);

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (patient == null || user == null)
            {
                return NotFound();
            }

            var model = new PatientProfileViewModel
            {
                FullName = patient.FullName,
                Email = user.Email,
                Phone = patient.Phone,
                Address = patient.Address,
                DateOfBirth = patient.DateOfBirth,
                Avatar = patient.Avatar
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(PatientProfileViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = _context.Patients
                .FirstOrDefault(p => p.UserId == userId);

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (patient == null || user == null)
            {
                return NotFound();
            }

            patient.FullName = model.FullName;
            patient.Phone = model.Phone;
            patient.Address = model.Address;
            patient.DateOfBirth = model.DateOfBirth;

            user.Email = model.Email;

            // Đổi mật khẩu
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                bool isCorrect =
                    BCrypt.Net.BCrypt.Verify(
                        model.CurrentPassword,
                        user.Password);

                if (!isCorrect)
                {
                    TempData["Error"] =
                        "Mật khẩu hiện tại không đúng";

                    return RedirectToAction("EditProfile");
                }

                if (model.NewPassword != model.ConfirmPassword)
                {
                    TempData["Error"] =
                        "Xác nhận mật khẩu không khớp";

                    return RedirectToAction("EditProfile");
                }

                user.Password =
                    BCrypt.Net.BCrypt.HashPassword(
                        model.NewPassword);
            }

            _context.SaveChanges();

            TempData["Success"] =
                "Cập nhật hồ sơ thành công";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = _context.Patients
                .FirstOrDefault(p => p.UserId == userId);

            if (patient == null)
            {
                return NotFound();
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                string folder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "avatars");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName =
                    Guid.NewGuid().ToString()
                    + Path.GetExtension(avatarFile.FileName);

                string filePath =
                    Path.Combine(folder, fileName);

                using (var stream =
                    new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                patient.Avatar =
                    "/uploads/avatars/" + fileName;

                _context.SaveChanges();

                HttpContext.Session.SetString(
                    "Avatar",
                    patient.Avatar
                );
            }

            return RedirectToAction("Profile");
        }
    }
}
