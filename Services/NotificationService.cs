using Health_Booking_MVC.Models;

namespace Health_Booking_MVC.Services
{
    public class NotificationService
    {
        private readonly HealthBookingDbContext _context;

        public NotificationService(HealthBookingDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
