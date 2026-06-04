using System.ComponentModel.DataAnnotations;

namespace Health_Booking_MVC.Models.ViewModels
{
    public class PatientProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string? Avatar { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
