using Health_Booking_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Health_Booking_MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly HealthBookingDbContext _context;

        public NotificationController(HealthBookingDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Chưa đăng nhập"
                });
            }

            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .Select(x => new
                {
                    x.NotificationId,
                    x.Message,
                    x.CreatedAt
                })
                .ToListAsync();

            int unreadCount = await _context.Notifications
                .CountAsync(x =>
                    x.UserId == userId &&
                    x.IsRead == false);

            return Json(new
            {
                success = true,
                notifications,
                unreadCount
            });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false });
            }

            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId && x.IsRead == false)
                .ToListAsync();

            foreach (var item in notifications)
            {
                item.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
