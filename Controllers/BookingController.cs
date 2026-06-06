using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Health_Booking_MVC.Models;
using Health_Booking_MVC.Services;
using System.Text.RegularExpressions;

namespace Health_Booking_MVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly HealthBookingDbContext _context;
        private readonly NotificationService _notificationService;

        public BookingController(HealthBookingDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBooking(string patient_name, string phone, int? hospital_id, int? specialization_id, int? doctor_user_id, string appointment_date, string appointment_time)
        {
            // 1. Kiểm tra đăng nhập tài khoản Bệnh nhân
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var sessionRole = HttpContext.Session.GetString("Role");

            if (sessionUserId == null || sessionRole != "patient")
            {
                return Json(new { success = false, message = "Quý khách chưa đăng nhập tài khoản. Vui lòng đăng nhập để tiếp tục." });
            }

            // Lấy thông tin hồ sơ Bệnh nhân dựa theo tài khoản đang đăng nhập
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == sessionUserId);
            if (patient == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ bệnh nhân tương ứng với tài khoản này." });
            }

            // Nếu người dùng không điền tên hoặc SĐT (ví dụ đặt lịch nhanh từ cơ sở), lấy mặc định từ Profile của họ
            if (string.IsNullOrEmpty(patient_name))
            {
                patient_name = patient.FullName;
            }
            if (string.IsNullOrEmpty(phone))
            {
                phone = patient.Phone;
            }

            // 2. Validation dữ liệu trống
            if (string.IsNullOrEmpty(patient_name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(appointment_date) || string.IsNullOrEmpty(appointment_time))
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
            }

            // 3. Validate định dạng Số điện thoại (10-11 số)
            if (!Regex.IsMatch(phone, @"^[0-9]{10,11}$"))
            {
                return Json(new { success = false, message = "Số điện thoại không hợp lệ" });
            }

            // 4. Validate ngày giờ không ở quá khứ (Backend Security)
            DateTime appointmentDateTime;
            if (DateTime.TryParse($"{appointment_date} {appointment_time}", out appointmentDateTime))
            {
                if (appointmentDateTime < DateTime.Now)
                {
                    return Json(new { success = false, message = "Ngày và giờ đặt khám không được là quá khứ" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Định dạng ngày giờ không hợp lệ" });
            }

            try
            {
                // Xử lý logic tìm kiếm Bác sĩ nếu đặt lịch đích danh (Có truyền doctor_user_id)
                int? targetDoctorId = null;
                string doctorName = null;
                int? targetUserIdForDoctorNotification = null;

                if (doctor_user_id.HasValue)
                {
                    var doctorSelected = await _context.Doctors
                        .Include(d => d.User)
                        .FirstOrDefaultAsync(d => d.UserId == doctor_user_id.Value);

                    if (doctorSelected != null)
                    {
                        targetDoctorId = doctorSelected.DoctorId;
                        doctorName = doctorSelected.FullName;
                        targetUserIdForDoctorNotification = doctorSelected.UserId;
                    }
                }

                // 5. Tạo thực thể Appointment lưu vào Cơ sở dữ liệu (Đã đồng bộ hoàn toàn với Model của bạn)
                var newAppointment = new Health_Booking_MVC.Models.Appointment
                {
                    PatientId = patient.PatientId,

                    // Nếu chọn bác sĩ từ thanh tìm kiếm thì lấy targetDoctorId, 
                    // nếu không (đặt tự do từ form) thì gán mặc định là 1 để tránh lỗi NOT NULL trong DB
                    DoctorId = targetDoctorId > 0 ? targetDoctorId.Value : 1,

                    // Bắt buộc phải có giá trị số nguyên (vì Model để kiểu 'int' không cho phép null)
                    ScheduleId = 1,

                    HospitalId = hospital_id,
                    SpecializationId = specialization_id,

                    AppointmentDate = appointmentDateTime,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    BookingSource = "Home"
                    // XÓA HOÀN TOÀN 2 dòng HospitalId và SpecializationId ở đây đi vì Model không có!
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                int requestId = newAppointment.AppointmentId;

                // 6. Lấy tên Bệnh viện & Chuyên khoa để viết nội dung thông báo chuông
                string hospitalName = "Chưa chọn";
                if (hospital_id.HasValue)
                {
                    var hp = await _context.Hospitals.FindAsync(hospital_id.Value);
                    if (hp != null) hospitalName = hp.Name;
                }

                string specialtyName = "Chưa chọn";
                if (specialization_id.HasValue)
                {
                    var sp = await _context.Specializations.FindAsync(specialization_id.Value);
                    if (sp != null) specialtyName = sp.Name;
                }

                // 7. Bắn thông báo chuông cho các Admin (Gọi hàm của bạn bạn xây dựng)
                var admins = await _context.Users.Where(u => u.Role == "Admin").ToListAsync();
                foreach (var admin in admins)
                {
                    string adminMessage = $"Yêu cầu đặt lịch khám mới từ {patient_name} ({phone}). Bệnh viện: {hospitalName}, Chuyên khoa: {specialtyName}, Ngày: {appointmentDateTime:dd/MM/yyyy HH:mm}" +
                                          (!string.IsNullOrEmpty(doctorName) ? $", Bác sĩ: {doctorName}" : "");
                    await _notificationService.CreateNotification(admin.UserId, adminMessage);
                }

                // 8. Bắn thông báo chuông riêng cho Bác sĩ (Nếu có chọn bác sĩ)
                if (targetUserIdForDoctorNotification.HasValue)
                {
                    string doctorMessage = $"Bạn có yêu cầu đặt khám mới từ {patient_name} vào ngày {appointmentDateTime:dd/MM/yyyy} lúc {appointment_time}. Vui lòng phản hồi sớm.";
                    await _notificationService.CreateNotification(targetUserIdForDoctorNotification.Value, doctorMessage);
                }

                // 9. Bắn thông báo chuông về cho chính Bệnh nhân để cập nhật trạng thái
                string patientMessage = $"Chúng tôi đã nhận được yêu cầu đặt lịch khám của bạn vào ngày {appointmentDateTime:dd/MM/yyyy} lúc {appointment_time}. Trạng thái hiện tại: Đang chờ duyệt.";
                await _notificationService.CreateNotification(sessionUserId.Value, patientMessage);

                // 10. Trả JSON thông báo về cho giao diện AJAX hiển thị popup alert
                return Json(new
                {
                    success = true,
                    message = "Đặt lịch khám thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.",
                    request_id = requestId
                });
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? " | Chi tiết: " + ex.InnerException.Message : "";
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt lịch: " + ex.Message + innerMsg });
            }
        }
    }
}