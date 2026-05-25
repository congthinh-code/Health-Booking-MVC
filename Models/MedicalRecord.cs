using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Health_Booking_MVC.Models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }
        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        [Required]
        public string Diagnosis { get; set; } // Chẩn đoán (Bản chất của Lịch sử khám nằm ở đây)
        public string Symptoms { get; set; }  // Triệu chứng
        public string Treatment { get; set; } // Lời dặn
        public string Prescription { get; set; } // Đơn thuốc
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
