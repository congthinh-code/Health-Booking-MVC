using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Health_Booking_MVC.Migrations
{
    /// <inheritdoc />
    public partial class database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hospitals",
                columns: table => new
                {
                    HospitalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hotline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hospitals", x => x.HospitalId);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.SpecializationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Doctors",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationId = table.Column<int>(type: "int", nullable: false),
                    HospitalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctors", x => x.DoctorId);
                    table.ForeignKey(
                        name: "FK_Doctors_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "HospitalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctors_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_Patients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoctorSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxPatients = table.Column<int>(type: "int", nullable: false),
                    CurrentPatients = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_DoctorSchedules_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_DoctorSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "DoctorSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "DoctorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecords",
                columns: table => new
                {
                    RecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symptoms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecords", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecords_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hospitals",
                columns: new[] { "HospitalId", "Address", "Description", "Hotline", "Image", "Name", "Rating", "WebsiteUrl" },
                values: new object[,]
                {
                    { 1, "106 Nguyễn Huệ, Phường Quy Nhơn, Tỉnh Gia Lai", "Bệnh viện công", "056 3820 289", "images/anhbenhvien/bvdk.jpg", "Bệnh viện đa khoa tỉnh Bình Định", 0, null },
                    { 2, "78 Trần Hưng Đạo, Quy Nhơn, Gia Lai", "Bệnh viện công", "0256 3893 247", "images/anhbenhvien/bvmat.jpg", "Bệnh viện Mắt Bình Định", 0, null },
                    { 3, "114 Trần Hưng Đạo, phường Quy Nhơn, tỉnh Gia Lai", "Bệnh viện công", "0269 6566 366", "images/anhbenhvien/bvquynhon.jpg", "Trung tâm Y tế Quy Nhơn", 0, null },
                    { 4, "Tổ 05, KV05, Phường Quy Nhơn Bắc, tỉnh Gia Lai", "Bệnh viện công", "0965 071 919", "images/anhbenhvien/yhoccotruyen.jpg", "Bệnh viện Y học cổ truyền & PHCN Bình Định", 0, null },
                    { 5, "05A Đường Chế Lan Viên, Quy Nhơn Nam, Gia Lai", "Bệnh viện công", "0256 3532 536", "images/anhbenhvien/bvquyhoa.jpg", "Bệnh viện Phong - Da liễu Trung ương Quy Hoà", 0, null },
                    { 6, "355 Trần Hưng Đạo, Phường Quy Nhơn, Gia Lai", "Bệnh viện tư", "0256 3822 900", "images/anhbenhvien/bvhoabinh.jpg", "Bệnh viện đa khoa Hoà Bình", 0, null },
                    { 7, "66 Đào Tấn, Xã Tuy Phước, Gia Lai", "Bệnh viện công", "0256 3833 315", "images/anhbenhvien/bvtuyphuoc.jpg", "Trung tâm Y tế huyện Tuy Phước", 0, null },
                    { 8, "54 An Dương Vương, Phường Quy Nhơn Nam, Gia Lai", "Bệnh viện công", "0256 3846 363", "images/anhbenhvien/quany13.jpg", "Bệnh viện Quân y 13", 0, null },
                    { 9, "420 Nguyễn Thái Học, Quy Nhơn Nam, Gia Lai", "Bệnh viện tư", "0256 3686 115", "images/anhbenhvien/dktp.jpg", "Bệnh viện đa khoa Thu Phúc", 0, null }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "SpecializationId", "Name" },
                values: new object[,]
                {
                    { 1, "Bác sĩ gia đình" },
                    { 2, "Tiêu hoá gan mật" },
                    { 3, "Nội tổng quát" },
                    { 4, "Nội tiết" },
                    { 5, "Da liễu" },
                    { 6, "Nội tim mạch" },
                    { 7, "Nội thần kinh" },
                    { 8, "Nội cơ xương khớp" },
                    { 9, "Tai mũi họng" },
                    { 10, "Mắt" },
                    { 11, "Nội tiêu hoá" },
                    { 12, "Nội hô hấp" },
                    { 13, "Nội tiết niệu" },
                    { 14, "Ngoại cơ xương khớp" },
                    { 15, "Sản - Phụ khoa" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "Password", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ngotrungnam@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenthithanhminh@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenphucthien@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dodangkhoa@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "levutan@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "baduykhuong@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenvanhoa@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "lethilan@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenhuuphong@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tranduchuy@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dominhquan@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vongocthanh@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyentanphat@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tranhongnhung@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "phamminhquan@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vuthingoc@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nguyenhuudung@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "tranthithuhang@healthbooking.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Doctor" },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patienta@gmail.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Patient" },
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patientb@gmail.com", "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4", "Patient" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "DoctorId", "Avatar", "Description", "ExperienceYears", "FullName", "HospitalId", "Phone", "SpecializationId", "UserId" },
                values: new object[,]
                {
                    { 1, "anhbs3.jpg", "Chuyên khoa Tim Mạch", 10, "BS CKII. Ngô Trung Nam", 1, "0901 234 501", 6, 2 },
                    { 2, "anhbs1.jpg", "Chuyên khoa Tim mạch", 5, "BS CKI. Nguyễn Thị Thanh Minh", 3, "0901 234 502", 6, 3 },
                    { 3, "anhbs5.jpg", "Chuyên khoa Tai Mũi Họng", 8, "BS CKI. Nguyễn Phúc Thiện", 2, "0901 234 503", 9, 4 },
                    { 4, "anhbs6.jpg", "Chuyên khoa Sản - Phụ khoa", 7, "BS CKI. Đỗ Đăng Khoa", 1, "0901 234 504", 15, 5 },
                    { 5, "anhbs7.jpg", "Chuyên khoa Da liễu", 10, "ThS BS. Lê Vũ Tân", 5, "0901 234 505", 5, 6 },
                    { 6, "anhbs8.jpg", "Chuyên khoa Tai Mũi Họng", 9, "BS CKI. Bá Duy Khương", 9, "0901 234 506", 9, 7 },
                    { 7, "anhbs10.jpg", "Chuyên khoa Thần Kinh", 12, "PGS TS. Nguyễn Văn Hoà", 6, "0901 234 507", 7, 8 },
                    { 8, "anhbs9.jpg", "Chuyên khoa Da liễu", 15, "BS CKII. Lê Thị Lan", 5, "0901 234 508", 5, 9 },
                    { 9, "anhbs11.jpg", "Chuyên khoa Tim mạch", 7, "ThS BS. Nguyễn Hữu Phong", 7, "0901 234 509", 6, 10 },
                    { 10, "anhbs12.jpg", "Chuyên khoa Nội tiết niệu", 20, "BS CKI. Trần Đức Huy", 3, "0901 234 510", 13, 11 },
                    { 11, "anhbs13.jpg", "Chuyên khoa Ngoại cơ xương khớp", 15, "ThS BS. Đỗ Minh Quân", 1, "0901 234 511", 14, 12 },
                    { 12, "anhbs2.jpg", "Chuyên khoa Sản", 10, "BS CKII. Võ Ngọc Thanh", 6, "0901 234 512", 15, 13 },
                    { 13, "anhbs14.jpg", "Chuyên khoa Nội hô hấp", 10, "BS CKI. Nguyễn Tấn Phát", 8, "0901 234 513", 12, 14 },
                    { 14, "anhbs4.jpg", "Chuyên khoa Mắt", 8, "BS CKII. Trần Hồng Nhung", 5, "0901 234 514", 10, 15 },
                    { 15, "anhbs15.jpg", "Chuyên khoa Tai Mũi Họng", 15, "ThS BS. Phạm Minh Quân", 6, "0901 234 515", 9, 16 },
                    { 16, "anhbs2.jpg", "Chuyên khoa Da liễu", 10, "BS CKI. Võ Thị Ngọc", 5, "0901 234 516", 5, 17 },
                    { 17, "anhbs18.jpg", "Chuyên khoa Tim mạch", 15, "ThS BS. Nguyễn Hữu Dũng", 6, "0901 234 517", 6, 18 },
                    { 18, "anhbs1.jpg", "Chuyên khoa Nội tiêu hoá ", 10, "ThS BS. Trần Thị Thu Hằng", 7, "0901 234 518", 11, 19 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientId", "Address", "Avatar", "DateOfBirth", "FullName", "Gender", "Phone", "UserId" },
                values: new object[,]
                {
                    { 1, "Phường Quy Nhơn Bắc, Gia Lai", null, new DateTime(1995, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lê Thị B", "Nữ", "0911223344", 20 },
                    { 2, "Phường Quy Nhơn Nam, Gia Lai", null, new DateTime(1990, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyễn Văn B", "Nam", "0911223345", 21 }
                });

            migrationBuilder.InsertData(
                table: "DoctorSchedules",
                columns: new[] { "ScheduleId", "CurrentPatients", "DoctorId", "EndTime", "MaxPatients", "StartTime", "WorkDate" },
                values: new object[] { 1, 1, 1, new TimeSpan(0, 12, 0, 0, 0), 10, new TimeSpan(0, 8, 0, 0, 0), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentDate", "CreatedAt", "DoctorId", "PatientId", "ScheduleId", "Status" },
                values: new object[] { 1, new DateTime(2026, 5, 20, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, 2 });

            migrationBuilder.InsertData(
                table: "MedicalRecords",
                columns: new[] { "RecordId", "AppointmentId", "CreatedAt", "Diagnosis", "Prescription", "Symptoms", "Treatment" },
                values: new object[] { 1, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Viêm họng cấp", "Paracetamol 500mg", "Ho, sốt nhẹ", "Uống thuốc ấm" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId",
                table: "Appointments",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_HospitalId",
                table: "Doctors",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecializationId",
                table: "Doctors",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId",
                table: "DoctorSchedules",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_AppointmentId",
                table: "MedicalRecords",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "DoctorSchedules");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Hospitals");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
