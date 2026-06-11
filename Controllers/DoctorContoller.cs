using Health_Booking_MVC.Models;
using Health_Booking_MVC.Models.ViewModels;
using Health_Booking_MVC.Services; // Thêm dòng này để gọi service thông báo
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Health_Booking_MVC.Controllers
{
    public class DoctorController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly NotificationService _notificationService; // Thêm service thông báo

        // Inject thêm NotificationService vào constructor
        public DoctorController(HealthBookingDbContext context, IWebHostEnvironment environment, NotificationService notificationService)
        {
            _context = context;
            _environment = environment;
            _notificationService = notificationService;
        }

        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var doctor = _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .Include(d => d.User)
                .FirstOrDefault(d => d.UserId == userId);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Schedule)
                .Where(a => a.DoctorId == doctor.DoctorId && a.BookingSource == "Doctor")
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            ViewBag.Doctor = doctor;

            ViewBag.Total = appointments.Count;
            ViewBag.Pending = appointments.Count(x => x.Status == AppointmentStatus.Pending);
            ViewBag.Completed = appointments.Count(x => x.Status == AppointmentStatus.Completed || x.Status == AppointmentStatus.Confirmed);
            ViewBag.Cancelled = appointments.Count(x => x.Status == AppointmentStatus.Cancelled);

            return View("Doctor", appointments);
        }

        // Action Xác nhận lịch khám
        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn này!" });
            }

            appointment.Status = AppointmentStatus.Confirmed;
            await _context.SaveChangesAsync();

            // Gửi thông báo cho bệnh nhân
            string msg = $"📅 Lịch hẹn khám ngày {appointment.AppointmentDate:dd/MM/yyyy} của bạn đã được Bác sĩ {appointment.Doctor.FullName} xác nhận.";
            await _notificationService.CreateNotification(appointment.Patient.UserId, msg);

            return Json(new { success = true, message = "Xác nhận lịch hẹn thành công!" });
        }

        // Action Từ chối lịch khám kèm lý do
        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id, string reason)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lịch hẹn này!" });
            }

            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();

            // Gửi thông báo cho bệnh nhân kèm lý do từ chối
            string msg = $"❌ Lịch khám ngày {appointment.AppointmentDate:dd/MM/yyyy} với Bác sĩ {appointment.Doctor.FullName} đã bị từ chối. Lý do: {reason}";
            await _notificationService.CreateNotification(appointment.Patient.UserId, msg);

            return Json(new { success = true, message = "Đã từ chối lịch hẹn thành công!" });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var doctor = _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .Include(d => d.User)
                .FirstOrDefault(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            var model = new DoctorEditViewModel
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                Phone = doctor.Phone,
                ExperienceYears = doctor.ExperienceYears,
                SpecializationId = doctor.SpecializationId,
                HospitalId = doctor.HospitalId,
                CurrentAvatar = doctor.Avatar,
                Description = doctor.Description
            };

            ViewBag.Specializations = _context.Specializations.ToList();
            ViewBag.Hospitals = _context.Hospitals.ToList();

            return View("Edit_doctor", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DoctorEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Specializations = _context.Specializations.ToList();
                ViewBag.Hospitals = _context.Hospitals.ToList();
                return View("Edit_doctor", model);
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == model.DoctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.FullName = model.FullName;
            doctor.Phone = model.Phone;
            doctor.ExperienceYears = model.ExperienceYears;
            doctor.SpecializationId = model.SpecializationId;
            doctor.HospitalId = model.HospitalId;
            doctor.Description = model.Description;

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                string folder = Path.Combine(_environment.WebRootPath, "images", "userAvatar");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.AvatarFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(stream);
                }

                doctor.Avatar = fileName;

                int? currentUserId = HttpContext.Session.GetInt32("UserId");
                if (currentUserId == doctor.UserId)
                {
                    HttpContext.Session.SetString("Avatar", "/images/userAvatar/" + fileName);
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật hồ sơ bác sĩ thành công!";

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == userId);
            if (doctor == null)
            {
                return NotFound();
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                string folder = Path.Combine(_environment.WebRootPath, "images", "userAvatar");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                doctor.Avatar = fileName;
                _context.SaveChanges();

                HttpContext.Session.SetString("Avatar", "/images/userAvatar/" + fileName);
            }

            return RedirectToAction("Dashboard");
        }
    }
}