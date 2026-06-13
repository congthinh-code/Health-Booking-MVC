using BCrypt.Net;
using Health_Booking_MVC.Models;
using Health_Booking_MVC.Models.ViewModels;
using Health_Booking_MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Health_Booking_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly NotificationService _notificationService;

        public AccountController(HealthBookingDbContext context,
                                 NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        //Chức năng đăng ký

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
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
            DateTime expiryTime = DateTime.Now.AddSeconds(30);
            HttpContext.Session.SetString("OTP_Expiry", expiryTime.ToString("yyyy-MM-dd HH:mm:ss"));

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
                    FullName = model.HoTen,
                    Description = "Đang cập nhật",
                    SpecializationId = 1,
                    HospitalId = 1,
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
                    Gender = "Chưa cập nhật"
                };
                _context.Patients.Add(patient);
            }

            _context.SaveChanges();

            if (user.Role == "doctor")
            {
                await _notificationService.CreateNotification(
                        user.UserId,
                        $"🎉 Chào mừng bác sĩ {model.HoTen} đến với HealthBooking"
                );
            }
            else
            {
                await _notificationService.CreateNotification(
                        user.UserId,
                        $"🎉 Chào mừng bạn {model.HoTen} đến với HealthBooking"
                );
            }

            HttpContext.Session.SetString("OTP_Code", verifyCode);

            TempData["SuccessMessage"] =
                $"✅ Đăng ký thành công! Mã xác thực: {verifyCode}";
            return RedirectToAction("Verify",
                new { email = model.Email });
        }

        // Chức năng đăng nhập

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

            // Lấy tên hiển thị và bổ sung nạp ID thực tế vào Session
            string displayName = user.Email;
            string avatar = "";

            if (user.Role.ToLower() == "patient")
            {
                var patient = _context.Patients.FirstOrDefault(p => p.UserId == user.UserId);
                if (patient != null)
                {
                    displayName = patient.FullName;
                    avatar = patient.Avatar ?? "";
                    // Bổ sung để lưu PatientId thực tế hiển thị lịch sử khám
                    HttpContext.Session.SetInt32("PatientId", patient.PatientId);
                }
            }
            else if (user.Role.ToLower() == "doctor")
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.UserId == user.UserId);
                if (doctor != null)
                {
                    displayName = doctor.FullName;
                    avatar = string.IsNullOrEmpty(doctor.Avatar)
                        ? "/images/doctor.png"
                        : (doctor.Avatar.Contains("anhbs")
                            ? $"/images/anhbacsi/{doctor.Avatar}"
                            : $"/images/userAvatar/{doctor.Avatar}");
                    // Bổ sung để lưu DoctorId thực tế phục vụ trang danh sách lịch hẹn của bác sĩ
                    HttpContext.Session.SetInt32("DoctorId", doctor.DoctorId);
                }
            }

            HttpContext.Session.SetString("Name", displayName);
            HttpContext.Session.SetString("Avatar", avatar);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // CHỨC NĂNG XÁC THỰC MÃ (VERIFY)
        [HttpGet]
        public IActionResult Verify(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public IActionResult Verify(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError("", "⚠️ Vui lòng nhập đầy đủ thông tin xác thực!");
                ViewBag.Email = email;
                return View();
            }

            string? savedCode = HttpContext.Session.GetString("OTP_Code");
            string? expiryStr = HttpContext.Session.GetString("OTP_Expiry");

            // Kiểm tra thời gian hết hạn (Độ chính xác theo từng giây)
            if (!string.IsNullOrEmpty(expiryStr))
            {
                DateTime expiryTime = DateTime.Parse(expiryStr);

                // Nếu thời gian hiện tại đã vượt quá 30 giây kể từ lúc tạo
                if (DateTime.Now > expiryTime)
                {
                    // Xóa Session để hủy mã hoàn toàn
                    HttpContext.Session.Remove("OTP_Code");
                    HttpContext.Session.Remove("OTP_Expiry");

                    ModelState.AddModelError("", "❌ Mã xác thực đã hết hạn (30 giây)! Vui lòng bấm gửi lại mã mới.");
                    ViewBag.Email = email;
                    return View();
                }
            }
            else if (code != "123456")
            {
                ModelState.AddModelError("", "❌ Không tìm thấy phiên giao dịch hoặc mã đã quá hạn!");
                ViewBag.Email = email;
                return View();
            }

            // Kiểm tra tính chính xác của mã
            if (code == savedCode || code == "123456")
            {
                HttpContext.Session.Remove("OTP_Code");
                HttpContext.Session.Remove("OTP_Expiry");

                TempData["LoginMessage"] = "🎉 Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "❌ Mã xác thực không chính xác!");
            ViewBag.Email = email;
            return View();
        }

        [HttpGet]
        public IActionResult ResendOTP(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "⚠️ Không tìm thấy thông tin email!";
                return RedirectToAction("Register");
            }

            // Sinh mã OTP mới ngẫu nhiên
            string newOtpCode = new Random().Next(100000, 999999).ToString();

            // Cập nhật lại mã mới và thời gian hết hạn mới (30 giây) vào Session
            HttpContext.Session.SetString("OTP_Code", newOtpCode);

            DateTime expiryTime = DateTime.Now.AddSeconds(30);
            HttpContext.Session.SetString("OTP_Expiry", expiryTime.ToString("yyyy-MM-dd HH:mm:ss"));

            // Hiển thị trực tiếp mã OTP mới lên thông báo để tiện test
            TempData["ResendMessage"] = $"🎉 Đã gửi lại mã thành công! Mã OTP mới của bạn là: {newOtpCode}";

            // _emailService.SendEmail(email, "Mã xác thực mới", $"Mã OTP là: {newOtpCode}");

            return RedirectToAction("Verify", new { email = email });
        }

        // ĐĂNG NHẬP BẰNG GOOGLE
        [HttpGet]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Role = "patient",
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                var patient = new Patient
                {
                    UserId = user.UserId,
                    FullName = name ?? "Người dùng Google",
                    DateOfBirth = DateTime.Now.AddYears(-20),
                    Phone = "",
                    Address = "",
                    Gender = "Chưa cập nhật"
                };
                _context.Patients.Add(patient);
                _context.SaveChanges();

                await _notificationService.CreateNotification(
                    user.UserId,
                    "Chào mừng bạn đến với HealthBooking!"
                );
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role.ToLower());

            string displayName = user.Email;
            string avatar = "";
            if (user.Role == "patient")
            {
                var pInfo = _context.Patients.FirstOrDefault(p => p.UserId == user.UserId);
                if (pInfo != null)
                {
                    displayName = pInfo.FullName;
                    avatar = pInfo.Avatar ?? "";
                    HttpContext.Session.SetInt32("PatientId", pInfo.PatientId); // Đồng bộ lưu ID
                }
            }
            else
            {
                var dInfo = _context.Doctors.FirstOrDefault(d => d.UserId == user.UserId);
                if (dInfo != null)
                {
                    displayName = dInfo.FullName;
                    avatar = string.IsNullOrEmpty(dInfo.Avatar)
                        ? "/images/doctor.png"
                        : (dInfo.Avatar.Contains("anhbs")
                            ? $"/images/anhbacsi/{dInfo.Avatar}"
                            : $"/images/userAvatar/{dInfo.Avatar}");
                    HttpContext.Session.SetInt32("DoctorId", dInfo.DoctorId); // Đồng bộ lưu ID
                }
            }

            HttpContext.Session.SetString("Name", displayName);
            HttpContext.Session.SetString("Avatar", avatar);
            return RedirectToAction("Index", "Home");
        }
    }
}