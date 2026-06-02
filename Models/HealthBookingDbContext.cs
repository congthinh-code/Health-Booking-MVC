using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System;

namespace Health_Booking_MVC.Models
{
    public class HealthBookingDbContext : DbContext
    {
        public HealthBookingDbContext(DbContextOptions<HealthBookingDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ràng buộc quan hệ 1-1
            modelBuilder.Entity<User>().HasOne(u => u.Patient).WithOne(p => p.User).HasForeignKey<Patient>(p => p.UserId);
            modelBuilder.Entity<User>().HasOne(u => u.Doctor).WithOne(d => d.User).HasForeignKey<Doctor>(d => d.UserId);

            // ================= FIX LỖI CASCADE CYCLES =================
            // Khi xóa một Bác sĩ, không tự động xóa Lịch hẹn, tránh tạo vòng lặp
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany() // Hoặc .WithMany() nếu trong class Doctor bạn không khai báo List<Appointment>
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict); // Đổi từ Cascade sang Restrict

            // Tương tự, nếu bảng MedicalRecord hoặc bảng khác cũng bị dính, bạn có thể chặn thêm:
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= SEED DATA =================
            var hasher = new PasswordHasher<User>();

            var user1 = new User
            {
                UserId = 1,
                Email = "admin@healthbooking.com",
                Role = "Admin",
                Password = "AQAAAAIAAYagAAAAEJz9bWfS27vGxPhBToTfXmP5KzEwNk8d/V+Sg7XbVmxN6F1v==" // Khóa cứng chuỗi Hash của "Admin.123"
            };

            var user2 = new User
            {
                UserId = 2,
                Email = "bacsitest@gmail.com",
                Role = "Doctor",
                Password = "AQAAAAIAAYagAAAAEOfkM98uXvPlFhGfT0YmX7Z8KwW1Nk6d/M9Xg8bVmxN5F2v==" // Khóa cứng chuỗi Hash của "Doctor.123"
            };

            var user3 = new User
            {
                UserId = 3,
                Email = "benhnhantest@gmail.com",
                Role = "Patient",
                Password = "AQAAAAIAAYagAAAAELmKP78uXvPlFhGfT0YmX7Z8KwW1Nk6d/M9Xg8bVmxN5F3v==" // Khóa cứng chuỗi Hash của "Patient.123"
            };

            modelBuilder.Entity<User>().HasData(user1, user2, user3);

            modelBuilder.Entity<Hospital>().HasData(
                new Hospital { 
                    HospitalId = 1, Name = "Bệnh viện đa khoa tỉnh Bình Định", 
                    Address = "106 Nguyễn Huệ, Phường Quy Nhơn, Tỉnh Gia Lai", 
                    Description = "Bệnh viện công", 
                    Hotline = "056 3820 289", 
                    Image = "bvdk.jpg" 
                },
                new Hospital
                {
                    HospitalId = 2,
                    Name = "Bệnh viện Mắt Bình Định",
                    Address = "78 Trần Hưng Đạo, Quy Nhơn, Gia Lai",
                    Description = "Bệnh viện công",
                    Hotline = "0256 3893 247",
                    Image = "bvmat.jpg"
                },
                new Hospital
                {
                    HospitalId = 3,
                    Name = "Trung tâm Y tế Quy Nhơn",
                    Address = "114 Trần Hưng Đạo, phường Quy Nhơn, tỉnh Gia Lai",
                    Description = "Bệnh viện công",
                    Hotline = "0269 6566 366",
                    Image = "bvquynhon.jpg"
                },
                new Hospital
                {
                    HospitalId = 4,
                    Name = "Bệnh viện Y học cổ truyền & PHCN Bình Định",
                    Address = "Tổ 05, KV05, Phường Quy Nhơn Bắc, tỉnh Gia Lai",
                    Description = "Bệnh viện công",
                    Hotline = "0965 071 919",
                    Image = "yhoccotruyen.jpg"
                },
                new Hospital
                {
                    HospitalId = 5,
                    Name = "Bệnh viện đa khoa Hoà Bình",
                    Address = "355 Trần Hưng Đạo, Phường Quy Nhơn, Gia Lai",
                    Description = "Bệnh viện công",
                    Hotline = "0256 3822 900",
                    Image = "bvhoabinh.jpg"
                }
            );

            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { 
                    SpecializationId = 1, 
                    Name = "Bác sĩ gia đình", 
                },
                new Specialization
                {
                    SpecializationId = 2,
                    Name = "Tiêu hoá gan mật",
                },
                new Specialization
                {
                    SpecializationId = 3,
                    Name = "Nội tổng quát",
                },
                new Specialization
                {
                    SpecializationId = 4,
                    Name = "Nội tiết",
                },
                new Specialization
                {
                    SpecializationId = 5,
                    Name = "Da liễu",
                },
                new Specialization
                {
                    SpecializationId = 6,
                    Name = "Nội tim mạch",
                },
                new Specialization
                {
                    SpecializationId = 7,
                    Name = "Nội thần kinh",
                },
                new Specialization
                {
                    SpecializationId = 8,
                    Name = "Nội cơ xương khớp",
                },
                new Specialization
                {
                    SpecializationId = 9,
                    Name = "Tai mũi họng",
                },
                new Specialization
                {
                    SpecializationId = 10,
                    Name = "Mắt",
                },
                new Specialization
                {
                    SpecializationId = 11,
                    Name = "Nội tiêu hoá",
                },
                new Specialization
                {
                    SpecializationId = 12,
                    Name = "Nội truyền nhiễm",
                },
                new Specialization
                {
                    SpecializationId = 13,
                    Name = "Nội hô hấp",
                },
                new Specialization
                {
                    SpecializationId = 14,
                    Name = "Nội tiết niệu",
                },
                new Specialization
                {
                    SpecializationId = 15,
                    Name = "Khoa Nhi",
                },
                new Specialization
                {
                    SpecializationId = 16,
                    Name = "Sản - Phụ khoa",
                }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { 
                    DoctorId = 1, 
                    UserId = 2, 
                    FullName = "BS. Nguyễn Văn A", 
                    Phone = "0901 234 567", 
                    ExperienceYears = 10, 
                    Description = "Chuyên khoa Tim Mạch", 
                    SpecializationId = 6, 
                    HospitalId = 1, 
                    Avatar = "anhbs1.jpg" 
                }
                //new Doctor
                //{
                //    DoctorId = 2,
                //    UserId = 4,
                //    FullName = "BS. Nguyễn Văn B",
                //    Phone = "0901 234 568",
                //    ExperienceYears = 5,
                //    Description = "Chuyên khoa Nhi",
                //    SpecializationId = 15,
                //    HospitalId = 3,
                //    Avatar = "anhbs3.jpg"
                //},
                //new Doctor
                //{
                //    DoctorId = 3,
                //    UserId = 5,
                //    FullName = "BS. Nguyễn Văn C",
                //    Phone = "0901 234 569",
                //    ExperienceYears = 8,
                //    Description = "Chuyên khoa Tai Mũi Họng",
                //    SpecializationId = 9,
                //    HospitalId = 2,
                //    Avatar = "anhbs5.jpg"
                //},
                //new Doctor
                //{
                //    DoctorId = 4,
                //    UserId = 6,
                //    FullName = "BS. Nguyễn Thị D",
                //    Phone = "0901 234 566",
                //    ExperienceYears = 7,
                //    Description = "Chuyên khoa Sản - Phụ khoa",
                //    SpecializationId = 16,
                //    HospitalId = 1,
                //    Avatar = "anhbs2.jpg"
                //},
                //new Doctor
                //{
                //    DoctorId = 5,
                //    UserId = 7,
                //    FullName = "BS. Nguyễn Thị E",
                //    Phone = "0901 234 565",
                //    ExperienceYears = 10,
                //    Description = "Chuyên khoa Da liễu",
                //    SpecializationId = 5,
                //    HospitalId = 5,
                //    Avatar = "anhbs4.jpg"
                //}
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { 
                    PatientId = 1, 
                    UserId = 3, 
                    FullName = "Lê Thị B", 
                    DateOfBirth = new DateTime(1995, 5, 20), 
                    Gender = "Nữ", 
                    Phone = "0911223344", 
                    Address = "Phường Quy Nhơn Bắc, Gia Lai" 
                }
            );

            modelBuilder.Entity<DoctorSchedule>().HasData(
                new DoctorSchedule { 
                    ScheduleId = 1, 
                    DoctorId = 1, 
                    WorkDate = new DateTime(2026, 6, 1), 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(12, 0, 0), 
                    MaxPatients = 10, 
                    CurrentPatients = 1 }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { 
                    AppointmentId = 1, 
                    PatientId = 1, 
                    DoctorId = 1, 
                    ScheduleId = 1, 
                    AppointmentDate = new DateTime(2026, 5, 20, 9, 30, 0), 
                    Status = AppointmentStatus.Completed 
                }
            );

            modelBuilder.Entity<MedicalRecord>().HasData(
                new MedicalRecord { 
                    RecordId = 1, 
                    AppointmentId = 1, 
                    Symptoms = "Ho, sốt nhẹ", 
                    Diagnosis = "Viêm họng cấp", 
                    Treatment = "Uống thuốc ấm", 
                    Prescription = "Paracetamol 500mg" 
                }
            );
        }
    }
}
