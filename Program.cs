using Health_Booking_MVC.Models;
using Health_Booking_MVC.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HealthBookingDbContext>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<NotificationService>();

//session
builder.Services.AddDistributedMemoryCache(); // Cần thiết để lưu bộ nhớ tạm cho Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn của Session (VD: 30 phút)
    options.Cookie.HttpOnly = true; // Bảo mật cookie chống tấn công XSS
    options.Cookie.IsEssential = true; // Bắt buộc phải có để ứng dụng chạy đúng
});
builder.Services.AddHttpContextAccessor();

// Đăng ký dịch vụ Authentication (Bắt buộc phải có Cookie để lưu trạng thái đăng nhập từ Google)
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie() // Lưu phiên đăng nhập bằng Cookie độc lập hoặc song song với Session của bạn
//.AddGoogle(googleOptions =>
//{
//    // Đọc thông tin từ file appsettings.json đã cấu hình ở Bước 2
//    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


// Tự động chạy Migration cập nhật Database khi khởi động ứng dụng
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HealthBookingDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
