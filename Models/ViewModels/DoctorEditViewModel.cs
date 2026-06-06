using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Health_Booking_MVC.Models
{
    public class DoctorEditViewModel
    {
        public int DoctorId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Phone { get; set; }

        public int ExperienceYears { get; set; }

        public int SpecializationId { get; set; }

        public int HospitalId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả chuyên môn")]
        public string Description { get; set; }
        public IFormFile? AvatarFile { get; set; }

        public string? CurrentAvatar { get; set; }
    }
}