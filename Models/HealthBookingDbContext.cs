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

            // ================= SEED DATA =================
            var hasher = new PasswordHasher<User>();

            var user1 = new User { UserId = 1, Email = "admin@healthbooking.com", Role = "Admin" };
            user1.Password = hasher.HashPassword(user1, "Admin@123");

            var user2 = new User { UserId = 2, Email = "bacsiA@gmail.com", Role = "Doctor" };
            user2.Password = hasher.HashPassword(user2, "Doctor@123");

            var user3 = new User { UserId = 3, Email = "benhnhanB@gmail.com", Role = "Patient" };
            user3.Password = hasher.HashPassword(user3, "Patient@123");

            modelBuilder.Entity<User>().HasData(user1, user2, user3);

            modelBuilder.Entity<Hospital>().HasData(
                new Hospital { HospitalId = 1, Name = "Bệnh viện Đa khoa Tâm Anh", Address = "Tân Bình, TP.HCM", Description = "Đạt chuẩn quốc tế", Hotline = "18006858" }
            );

            modelBuilder.Entity<Specialization>().HasData(
                new Specialization { SpecializationId = 1, Name = "Khoa Nhi", Description = "Khám bệnh cho trẻ em" }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { DoctorId = 1, UserId = 2, FullName = "BS. Nguyễn Văn A", Phone = "0901234567", ExperienceYears = 10, Description = "Chuyên khoa Nhi", SpecializationId = 1, HospitalId = 1, Avatar = "default.jpg" }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { PatientId = 1, UserId = 3, FullName = "Lê Thị B", DateOfBirth = new DateTime(1995, 5, 20), Gender = "Nữ", Phone = "0911223344", Address = "Quận 3, TP.HCM" }
            );

            modelBuilder.Entity<DoctorSchedule>().HasData(
                new DoctorSchedule { ScheduleId = 1, DoctorId = 1, WorkDate = new DateTime(2026, 6, 1), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0), MaxPatients = 10, CurrentPatients = 1 }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { AppointmentId = 1, PatientId = 1, DoctorId = 1, ScheduleId = 1, AppointmentDate = new DateTime(2026, 5, 20, 9, 30, 0), Status = AppointmentStatus.Completed }
            );

            modelBuilder.Entity<MedicalRecord>().HasData(
                new MedicalRecord { RecordId = 1, AppointmentId = 1, Symptoms = "Ho, sốt nhẹ", Diagnosis = "Viêm họng cấp", Treatment = "Uống thuốc ấm", Prescription = "Paracetamol 500mg" }
            );
        }
    }
}
