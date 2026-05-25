using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Health_Booking_MVC.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; };
        [Required, StringLength(255)]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Hotline { get; set; }
        public virtual ICollection<Doctor> Doctors { get; set; }
    }
}
