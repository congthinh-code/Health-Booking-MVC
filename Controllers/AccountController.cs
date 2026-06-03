using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Health_Booking_MVC.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
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

            HttpContext.Session.SetString("OTP_Code", verifyCode);

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

            // 🔥 SỬA TẠI ĐÂY: Lấy mã OTP chính xác từ Session ra
            string? savedCode = HttpContext.Session.GetString("OTP_Code");

            // Tiến hành kiểm tra đối chiếu mã nhập vào
            if (code == savedCode || code == "123456") // Vẫn giữ mã 123456 dự phòng để bạn test nhanh
            {
                // Xác thực thành công -> Xóa mã OTP trong Session đi để bảo mật
                HttpContext.Session.Remove("OTP_Code");

                // Chuyển hướng sang trang Đăng nhập kèm thông báo thành công
                TempData["LoginMessage"] = "🎉 Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            // Nếu mã nhập vào bị sai hoặc Session hết hạn
            ModelState.AddModelError("", "❌ Mã xác thực không chính xác!");
            ViewBag.Email = email;
            return View();
        }
        // 1. Hàm kích hoạt yêu cầu đăng nhập bằng Google
        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            // Cấu hình thuộc tính chuyển hướng: Sau khi đăng nhập Google xong, Google sẽ trả kết quả về hàm "GoogleResponse"
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // 2. Hàm đón nhận kết quả trả về từ Google
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            // Đọc thông tin tài khoản mà Google trả về thông qua Cookie mã hóa
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
            {
                return RedirectToAction("Login"); // Nếu thất bại quay lại trang Đăng nhập
            }

            // Trích xuất các thông tin cơ bản từ Google tài khoản người dùng
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            // KIỂM TRA TRONG DATABASE CỦA BẠN:
            // Xem Email này đã từng tồn tại trong bảng Users chưa
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                // TRƯỜNG HỢP 1: Tài khoản mới tinh chưa từng đăng ký hệ thống của bạn
                // Tự động tạo một User mới lưu xuống Database
                user = new User
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Mật khẩu ngẫu nhiên vì họ đăng nhập qua Google
                    Role = "patient", // Mặc định gán vai trò là Bệnh nhân
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                // Tạo thêm bảng thông tin Patient tương ứng
                var patient = new Patient
                {
                    UserId = user.UserId,
                    FullName = name ?? "Người dùng Google",
                    DateOfBirth = DateTime.Now.AddYears(-20), // Ngày sinh tạm thời
                    Phone = "",
                    Address = "",
                    Gender = "Chưa cập nhật"
                };
                _context.Patients.Add(patient);
                _context.SaveChanges();
            }

            // TRƯỜNG HỢP 2: Tài khoản đã có trong hệ thống (Hoặc vừa tạo xong ở trên)
            // Thực hiện lưu dữ liệu vào Session giống hệt hàm Login cũ của bạn để đồng bộ hệ thống navbar
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role.ToLower());

            // Lấy tên hiển thị
            string displayName = user.Email;
            if (user.Role == "patient")
            {
                var pInfo = _context.Patients.FirstOrDefault(p => p.UserId == user.UserId);
                if (pInfo != null) displayName = pInfo.FullName;
            }
            else
            {
                var dInfo = _context.Doctors.FirstOrDefault(d => d.UserId == user.UserId);
                if (dInfo != null) displayName = dInfo.FullName;
            }

            HttpContext.Session.SetString("Name", displayName);

            // Đăng nhập thành công! Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }
    }
}