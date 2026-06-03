using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Health_Booking_MVC.Models;

namespace Health_Booking_MVC.Controllers
{
    public class LHController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public LHController(HealthBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CSYT(int page = 1)
        {
            int pageSize = 4;

            int totalItems = await _context.Hospitals.CountAsync();

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var hospitals = await _context.Hospitals
                .OrderBy(x => x.HospitalId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(hospitals);
        }

        //public async Task<IActionResult> CSYT()
        //{
        //    var hospitals = await _context.Hospitals.ToListAsync();

        //    return View(hospitals);
        //}

        public async Task<IActionResult> QC()
        {
            var hospitals = await _context.Hospitals
                .Take(5)
                .ToListAsync();

            return View(hospitals);
        }

        public IActionResult TD()
        {
            return View();
        }

        public IActionResult JobDetail(int id)
        {
            dynamic job = id switch
            {
                1 => new
                {
                    Title = "Phát Triển Kinh Doanh (B2B)",
                    Type = "Full Time",
                    Description = "Tìm kiếm và phát triển khách hàng doanh nghiệp."
                },

                2 => new
                {
                    Title = "Chuyên Viên Hành Chính Nhân Sự",
                    Type = "Full Time",
                    Description = "Quản lý hồ sơ nhân sự, tuyển dụng và hành chính văn phòng."
                },

                3 => new
                {
                    Title = "Kế Toán Viên Kiêm Chăm Sóc Khách Hàng",
                    Type = "Full Time",
                    Description = "Theo dõi công nợ, lập báo cáo và hỗ trợ khách hàng."
                },

                4 => new
                {
                    Title = "Thực Tập Sinh Content Marketing",
                    Type = "Part Time",
                    Description = "Hỗ trợ xây dựng nội dung cho website và mạng xã hội."
                },

                5 => new
                {
                    Title = "Giám Đốc Phát Triển Kinh Doanh - Dịch Vụ",
                    Type = "Full Time",
                    Description = "Xây dựng chiến lược kinh doanh và mở rộng thị trường."
                },

                6 => new
                {
                    Title = "Business Analyst",
                    Type = "Full Time",
                    Description = "Phân tích yêu cầu nghiệp vụ và đề xuất giải pháp."
                },

                7 => new
                {
                    Title = "Frontend ReactJS Developer",
                    Type = "Full Time",
                    Description = "Phát triển giao diện web bằng ReactJS."
                },

                8 => new
                {
                    Title = "Backend NodeJS Developer",
                    Type = "Full Time",
                    Description = "Xây dựng API và hệ thống backend bằng NodeJS."
                },

                9 => new
                {
                    Title = "Mobile App Developer (iOS/Android)",
                    Type = "Full Time",
                    Description = "Phát triển ứng dụng di động đa nền tảng."
                },

                10 => new
                {
                    Title = "Digital Marketing Specialist",
                    Type = "Part Time",
                    Description = "Thực hiện các chiến dịch quảng cáo và marketing."
                },

                11 => new
                {
                    Title = "Data Analyst Intern",
                    Type = "Internship",
                    Description = "Hỗ trợ phân tích dữ liệu và lập báo cáo."
                },

                12 => new
                {
                    Title = "UI/UX Designer",
                    Type = "Full Time",
                    Description = "Thiết kế giao diện và tối ưu trải nghiệm người dùng."
                },

                13 => new
                {
                    Title = "Product Manager",
                    Type = "Full Time",
                    Description = "Quản lý sản phẩm từ ý tưởng đến triển khai."
                },

                14 => new
                {
                    Title = "QA Engineer",
                    Type = "Full Time",
                    Description = "Kiểm thử phần mềm và đảm bảo chất lượng sản phẩm."
                },

                15 => new
                {
                    Title = "Senior Technical Writer",
                    Type = "Part Time",
                    Description = "Biên soạn tài liệu kỹ thuật và hướng dẫn sử dụng."
                },

                16 => new
                {
                    Title = "DevOps Engineer",
                    Type = "Full Time",
                    Description = "Quản lý CI/CD và hạ tầng hệ thống."
                },

                17 => new
                {
                    Title = "Graphics Designer Intern",
                    Type = "Internship",
                    Description = "Thiết kế hình ảnh cho website và marketing."
                },

                _ => new
                {
                    Title = "Không tìm thấy vị trí",
                    Type = "",
                    Description = "Thông tin tuyển dụng không tồn tại."
                }
            };

            return View(job);
        }
    }
}