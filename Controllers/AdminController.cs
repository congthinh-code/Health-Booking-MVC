using Health_Booking_MVC.Models;
using Health_Booking_MVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health_Booking_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly NotificationService _notificationService;
        public AdminController(
            HealthBookingDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Hàm Filter kiểm tra quyền Admin/Doctor trước khi thực thi
        private bool IsAuthorized()
        {
            var role = HttpContext.Session.GetString("Role");
            return !string.IsNullOrEmpty(role) && (role == "admin" || role == "doctor");
        }

        // 1. TRANG TỔNG QUAN (DASHBOARD)
        public IActionResult Index()
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Đếm thống kê số lượng gửi ra màn hình Dashboard
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TotalDoctors = _context.Doctors.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();

            return View();
        }

        // 2. TRANG QUẢN LÝ DANH SÁCH BÁC SĨ
        public IActionResult Doctor()
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Lấy toàn bộ danh sách Bác sĩ kèm thông tin Chuyên khoa và Bệnh viện kết nối khóa ngoại
            var doctors = _context.Doctors
                .Include(d => d.Specialization)
                .Include(d => d.Hospital)
                .ToList();

            return View(doctors);
        }

        // 3. TRANG QUẢN LÝ DANH SÁCH BỆNH NHÂN
        public IActionResult Patient()
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            var patients = _context.Patients.ToList();
            return View(patients);
        }

        // 1. XỬ LÝ THÊM BÁC SĨ (CREATE)
        [HttpGet]
        public IActionResult CreateDoctor()
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");
            ViewBag.Specializations = _context.Specializations.ToList();
            ViewBag.Hospitals = _context.Hospitals.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDoctor(Doctor model)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Loại bỏ kiểm tra Validation đối với các thuộc tính liên kết hệ thống tự sinh
            ModelState.Remove("User");
            ModelState.Remove("Specialization");
            ModelState.Remove("Hospital");
            // Loại bỏ bắt buộc đối với danh sách Lịch trình
            ModelState.Remove("Schedules");

            if (ModelState.IsValid)
            {
                model.UserId = 1; // ID tài khoản liên kết tạm thời

                _context.Doctors.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Doctor");
            }

            ViewBag.Specializations = _context.Specializations.ToList();
            ViewBag.Hospitals = _context.Hospitals.ToList();
            return View(model);
        }

        // 2. XỬ LÝ SỬA BÁC SĨ (UPDATE)
        [HttpGet]
        public IActionResult EditDoctor(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");
            var doctor = _context.Doctors.Find(id);
            if (doctor == null) return NotFound();

            ViewBag.Specializations = _context.Specializations.ToList();
            ViewBag.Hospitals = _context.Hospitals.ToList();
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditDoctor(Doctor model)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Loại bỏ kiểm tra Validation đối với các thuộc tính liên kết hệ thống tự sinh
            ModelState.Remove("User");
            ModelState.Remove("Specialization");
            ModelState.Remove("Hospital");
            // Loại bỏ bắt buộc đối với danh sách Lịch trình
            ModelState.Remove("Schedules");

            if (ModelState.IsValid)
            {
                var existingDoctor = _context.Doctors.Find(model.DoctorId);
                if (existingDoctor == null) return NotFound();

                existingDoctor.FullName = model.FullName;
                existingDoctor.Phone = model.Phone;
                existingDoctor.ExperienceYears = model.ExperienceYears;
                existingDoctor.Description = model.Description;
                existingDoctor.SpecializationId = model.SpecializationId;
                existingDoctor.HospitalId = model.HospitalId;

                _context.Doctors.Update(existingDoctor);
                _context.SaveChanges();
                return RedirectToAction("Doctor");
            }

            ViewBag.Specializations = _context.Specializations.ToList();
            ViewBag.Hospitals = _context.Hospitals.ToList();
            return View(model);
        }

        // 5. HÀM XÓA BÁC SĨ (DELETE)
        [HttpPost]
        [ValidateAntiForgeryToken] // Lớp bảo mật chống giả mạo request từ bên ngoài
        public IActionResult DeleteDoctor(int id)
        {
            // Kiểm tra quyền truy cập của Admin/Doctor trước khi thực hiện
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // 1. Tìm thông tin bác sĩ trong DB theo ID truyền lên
            var doctor = _context.Doctors.Find(id);

            if (doctor != null)
            {
                try
                {
                    // 2. Xóa bản ghi bác sĩ khỏi bảng Doctors
                    _context.Doctors.Remove(doctor);

                    // 3. Lưu thay đổi xuống cơ sở dữ liệu
                    _context.SaveChanges();

                    // Bạn có thể dùng TempData để thông báo thành công ra giao diện
                    TempData["SuccessMessage"] = "Xóa bác sĩ thành công!";
                }
                catch (Exception ex)
                {
                    // Trường hợp bác sĩ đã có lịch hẹn hoặc lịch trình khám, database sẽ chặn không cho xóa trực tiếp
                    // Lúc này hệ thống sẽ thông báo lỗi thay vì bị sập trang
                    TempData["ErrorMessage"] = "Không thể xóa bác sĩ này vì đã có dữ liệu lịch trình hoặc lịch hẹn liên quan!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin bác sĩ cần xóa.";
            }

            // Sau khi xử lý xong, tải lại trang danh sách Bác sĩ
            return RedirectToAction("Doctor");
        }

        // 3. THÊM MỚI: HÀM KHÓA TÀI KHOẢN BỆNH NHÂN (LOCK)
        [HttpPost]
        public IActionResult LockPatient(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Tìm thông tin bệnh nhân theo ID
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                // Tìm tài khoản User liên kết mật thiết với Bệnh nhân này
                var user = _context.Users.Find(patient.UserId);
                if (user != null)
                {
                    // Thay đổi vai trò (Role) thành "locked" để vô hiệu hóa quyền truy cập đăng nhập
                    user.Role = "locked";
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Patient"); // Khóa xong tải lại trang danh sách bệnh nhân
        }
        // 4. THÊM MỚI: HÀM XÓA BỆNH NHÂN (DELETE)
        [HttpPost]
        public IActionResult DeletePatient(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // 1. Tìm thông tin bệnh nhân trong bảng Patients dựa vào ID
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                // Lưu lại UserId trước khi xóa Patient để tí nữa xóa tài khoản đăng nhập tương ứng
                int userId = patient.UserId;

                // 2. Xóa hồ sơ bệnh nhân trước
                _context.Patients.Remove(patient);

                // 3. Tìm tài khoản User liên kết để xóa tận gốc (tránh rác dữ liệu đăng nhập)
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                // 4. Lưu thay đổi xuống Database
                _context.SaveChanges();
            }

            // Xóa xong quay lại tải lại trang danh sách bệnh nhân
            return RedirectToAction("Patient");
        }
        // 6. TRANG QUẢN LÝ LỊCH HẸN KHÁM (READ)
        public IActionResult Appointments()
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // Lấy danh sách lịch hẹn, kèm theo thông tin Bệnh nhân và Bác sĩ để hiển thị ra bảng
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Schedule)
                .Include(a => a.Hospital)
                .Include(a => a.Specialization)
                .Where(a => a.BookingSource == "Home")
                .OrderByDescending(a => a.AppointmentDate) // Lịch hẹn mới nhất xếp lên đầu
                .ToList();

            return View(appointments); // Trả về tệp Views/Admin/Appointments.cshtml
        }

        // 7. XỬ LÝ XÁC NHẬN ĐẶT LỊCH KHÁM (UPDATE STATUS)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmAppointment(int id)
        {
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // 1. Tìm thông tin lịch hẹn theo mã ID gửi lên
            var appointment = _context.Appointments.Find(id);

            if (appointment != null)
            {
                // So sánh trực tiếp với Enum AppointmentStatus.Pending
                if (appointment.Status == AppointmentStatus.Pending)
                {
                    // Gán trạng thái mới bằng Enum AppointmentStatus.Confirmed
                    appointment.Status = AppointmentStatus.Confirmed;

                    _context.Appointments.Update(appointment);
                    _context.SaveChanges();

                    var patient = _context.Patients
                        .FirstOrDefault(p => p.PatientId == appointment.PatientId);

                    if (patient != null)
                    {
                        _notificationService.CreateNotification(
                            patient.UserId,
                            $"✅ Lịch khám ngày {appointment.AppointmentDate:dd/MM/yyyy HH:mm} đã được xác nhận."
                        ).Wait();
                    }

                    TempData["SuccessMessage"] = $"Đã xác nhận thành công lịch hẹn mã #{id}!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Lịch hẹn này đã được xử lý hoặc không ở trạng thái chờ duyệt.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin lịch hẹn.";
            }

            return RedirectToAction("Appointments");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointment(int id)
        {
            // Kiểm tra quyền truy cập trước khi thực hiện xóa
            if (!IsAuthorized()) return RedirectToAction("Login", "Account");

            // 1. Tìm lịch hẹn trong cơ sở dữ liệu theo ID
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "❌ Không tìm thấy lịch hẹn hoặc lịch hẹn đã bị xóa trước đó.";
                return RedirectToAction("Appointments");
            }

            try
            {
                // 2. Thực hiện xóa khỏi DbContext và lưu thay đổi
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"🎉 Đã xóa vĩnh viễn lịch hẹn #{id} thành công!";
            }
            catch (Exception ex)
            {
                // Xử lý trường hợp ràng buộc dữ liệu (ví dụ lịch hẹn đã phát sinh hóa đơn, bệnh án...)
                TempData["ErrorMessage"] = "❌ Không thể xóa lịch hẹn này do có dữ liệu liên quan khác trong hệ thống!";
            }

            // Sử dụng Redirect (đối với đường dẫn URL) hoặc quay về trang Appointments cố định
            string refererUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return Redirect(refererUrl); // Điều hướng an toàn về đúng URL trang trước đó của Admin
            }

            return RedirectToAction("Appointments"); // Phương án dự phòng quay về trang danh sách lịch hẹn
        }
    }
}
