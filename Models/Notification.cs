using System.ComponentModel.DataAnnotations;

namespace Health_Booking_MVC.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
