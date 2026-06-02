using System.ComponentModel.DataAnnotations;

namespace Health_Booking_MVC.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "⚠️ Vui lòng nhập họ và tên!")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "⚠️ Vui lòng nhập ngày sinh!")]
        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không đúng định dạng")]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "⚠️ Vui lòng nhập Email!")]
        [EmailAddress(ErrorMessage = "⚠️ Email không hợp lệ!")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "⚠️ Số điện thoại phải gồm 10-11 chữ số!")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "⚠️ Vui lòng nhập mật khẩu!")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[A-Z].*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]$",
            ErrorMessage = "⚠️ Mật khẩu phải bắt đầu bằng chữ in hoa và kết thúc bằng ký tự đặc biệt!")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "⚠️ Vui lòng chọn vai trò!")]
        public string Role { get; set; } = "patient"; // patient hoặc doctor
    }
}
