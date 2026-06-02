using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Health_Booking_MVC.Models.ViewModels;
using BCrypt.Net;

namespace Health_Booking_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HealthBookingDbContext _context;
        public AccountController(HealthBookingDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra trùng lặp email
            bool isExist = _context.Users.Any(u => u.Email == model.Email);
            if (isExist)
            {
                ModelState.AddModelError("", "⚠️ Email đã tồn tại trên hệ thống!");
                return View(model);
            }

            // Mã hóa mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);
            string verifyCode = new Random().Next(100000, 999999).ToString();

            // 1. Tạo thực thể User
            var user = new User
            {
                Email = model.Email,
                Password = passwordHash,
                Role = model.Role.ToLower(),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges(); // Lưu trước để lấy ra UserId tự tăng

            // 2. Tạo thông tin chi tiết dựa theo Vai trò
            if (user.Role == "doctor")
            {
                var doctor = new Doctor
                {
                    UserId = user.UserId,
                    FullName = model.HoTen, // Bổ sung trường bắt buộc
                    Description = "Đang cập nhật", // Bổ sung trường bắt buộc
                    SpecializationId = 1, // Sửa từ SpecialtyId sang đúng SpecializationId theo Model
                    HospitalId = 1, // Bổ sung khóa ngoại bắt buộc để tránh lỗi DB
                    ExperienceYears = 0,
                    Phone = model.SoDienThoai ?? ""
                };
                _context.Doctors.Add(doctor);
            }
            else
            {
                var patient = new Patient
                {
                    UserId = user.UserId,
                    FullName = model.HoTen,
                    DateOfBirth = model.NgaySinh,
                    Phone = model.SoDienThoai ?? "",
                    Address = model.DiaChi ?? "",
                    Gender = "Chưa cập nhật" // Bổ sung giá trị mặc định tránh lỗi NOT NULL trong DB
                };
                _context.Patients.Add(patient);
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = $"✅ Đăng ký thành công! Mã xác thực: {verifyCode}";
            return RedirectToAction("Verify", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "❌ Sai tài khoản hoặc mật khẩu!");
                return View(model);
            }

            // Đăng nhập thành công -> Lưu dữ liệu vào Session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role.ToLower());

            // Lấy tên hiển thị
            string displayName = user.Email;
            if (user.Role == "patient" && user.Patient != null) displayName = user.Patient.FullName;
            if (user.Role == "doctor" && user.Doctor != null) displayName = user.Doctor.FullName;

            HttpContext.Session.SetString("Name", displayName);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        // ==========================================
        // CHỨC NĂNG XÁC THỰC MÃ (VERIFY)
        // ==========================================

        // 1. Hàm GET: Hiển thị giao diện nhập mã xác thực
        [HttpGet]
        public IActionResult Verify(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }

            // Truyền email sang View để hiển thị cho người dùng biết mã gửi về đâu
            ViewBag.Email = email;
            return View();
        }

        // 2. Hàm POST: Xử lý khi người dùng nhấn nút "Xác thực"
        [HttpPost]
        public IActionResult Verify(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("", "⚠️ Vui lòng nhập đầy đủ thông tin xác thực!");
                ViewBag.Email = email;
                return View();
            }

            // Lấy mã xác thực đã được lưu tạm ở TempData lúc đăng ký thành công
            // Lưu ý: TempData chỉ tồn tại qua 1 lần Redirect, nên cần dùng .Peek() nếu muốn giữ lại khi nhập sai
            string? savedCode = TempData.Peek("SuccessMessage")?.ToString()?.Split(':').LastOrDefault()?.Trim();

            // LƯU Ý THỰC TẾ: Vì hiện tại hệ thống chưa tích hợp Server gửi Email thật, 
            // Nên nếu mã tạo ngẫu nhiên bị mất, bạn có thể mặc định hoặc chấp nhận mã trùng khớp từ TempData.
            if (code == savedCode || code == "123456") // Cho phép dùng 123456 làm mã dự phòng để test
            {
                // Xác thực thành công -> Chuyển hướng sang trang Đăng nhập kèm thông báo
                TempData["LoginMessage"] = "🎉 Xác thực tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            // Nếu mã nhập vào bị sai
            ModelState.AddModelError("", "❌ Mã xác thực không chính xác hoặc đã hết hạn!");
            ViewBag.Email = email;
            return View();
        }
    }
}