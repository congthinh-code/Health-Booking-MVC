using Health_Booking_MVC.Models;
using Health_Booking_MVC.Services; // Thêm thư viện dịch vụ thông báo
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Health_Booking_MVC.Controllers
{
    public class DVYTController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly NotificationService _notificationService; // Khai báo service thông báo

        // Inject thêm NotificationService vào Constructor
        public DVYTController(HealthBookingDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> ĐKBS(int? doctorId)
        {
            var doctors = await _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .Include(d => d.User)
                .ToListAsync();

            if (doctorId.HasValue)
            {
                var selectedDoctor = doctors
                    .FirstOrDefault(d => d.DoctorId == doctorId.Value);

                if (selectedDoctor != null)
                {
                    ViewBag.SelectedDoctor = selectedDoctor;

                    // Loại bác sĩ này khỏi danh sách bên dưới để không bị lặp
                    doctors = doctors
                        .Where(d => d.DoctorId != doctorId.Value)
                        .ToList();
                }
            }

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

        // Action đặt lịch khám (Có bắn thông báo tự động cho Bác sĩ)
        [HttpPost]
        public async Task<IActionResult> Book(int doctorId, string patientName, string phone, DateTime date, TimeSpan time)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "patient")
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập với tài khoản Bệnh nhân để đặt khám!" });
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin bệnh nhân của tài khoản này!" });
            }

            // Cập nhật thông tin bệnh nhân nếu có thay đổi
            if (!string.IsNullOrEmpty(patientName)) patient.FullName = patientName;
            if (!string.IsNullOrEmpty(phone)) patient.Phone = phone;

            // Tìm hoặc tạo lịch làm việc (DoctorSchedule) cho ngày này
            var schedule = await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.WorkDate == date.Date);

            if (schedule == null)
            {
                schedule = new DoctorSchedule
                {
                    DoctorId = doctorId,
                    WorkDate = date.Date,
                    StartTime = time,
                    EndTime = time.Add(TimeSpan.FromMinutes(30)),
                    MaxPatients = 10,
                    CurrentPatients = 1
                };
                _context.DoctorSchedules.Add(schedule);
            }
            else
            {
                if (schedule.CurrentPatients >= schedule.MaxPatients)
                {
                    return Json(new { success = false, message = "Lịch khám vào ngày này đã đầy!" });
                }
                schedule.CurrentPatients++;
            }

            // Tạo và lưu Lịch hẹn mới
            var appointment = new Appointment
            {
                PatientId = patient.PatientId,
                DoctorId = doctorId,
                Schedule = schedule,
                AppointmentDate = date.Date.Add(time),
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.Now,

                BookingSource = "Doctor"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            await _notificationService.CreateNotification(
                userId.Value,
                $"✅ Bạn đã đặt lịch khám với bác sĩ thành công vào lúc {time:hh\\:mm} ngày {date:dd/MM/yyyy}. Vui lòng chờ bác sĩ xác nhận."
            );  

            // 🔔 GỬI THÔNG BÁO CHO BÁC SĨ
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor != null)
            {
                string msg = $"📅 Lịch khám mới: Bệnh nhân {patient.FullName} đã đặt khám thành công vào lúc {time:hh\\:mm} ngày {date.ToString("dd/MM/yyyy")}.";
                await _notificationService.CreateNotification(doctor.UserId, msg);
            }

            return Json(new { success = true, message = "Đặt lịch khám thành công!" });
        }
    }
}