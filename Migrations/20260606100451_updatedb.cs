using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health_Booking_MVC.Migrations
{
    /// <inheritdoc />
    public partial class updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BookingSource",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecializationId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "AppointmentId",
                keyValue: 1,
                columns: new[] { "BookingSource", "HospitalId", "SpecializationId" },
                values: new object[] { "Doctor", null, null });

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 1,
                column: "WebsiteUrl",
                value: "https://benhvienbinhdinh.com.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 2,
                column: "WebsiteUrl",
                value: "https://www.bvmatbinhdinh.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 3,
                column: "WebsiteUrl",
                value: "https://benhvienquynhon.gov.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 4,
                column: "WebsiteUrl",
                value: "https://yhctphcnquynhon.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 5,
                column: "WebsiteUrl",
                value: "https://bvquyhoa.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 6,
                column: "WebsiteUrl",
                value: "https://hoabinhhospital.com.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 7,
                column: "WebsiteUrl",
                value: "https://ttyttuyphuoc.com.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 8,
                column: "WebsiteUrl",
                value: "https://benhvienquany13.vn/");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 9,
                column: "WebsiteUrl",
                value: "https://thuphuc.vn/");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 15,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 16,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 17,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 18,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 19,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 20,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 21,
                column: "Password",
                value: "$2a$11$.IaAh9ncKKq/7u608sBTKOIEUTv2hWuEy/AVXnLttucniyNC1rKtC");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_HospitalId",
                table: "Appointments",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SpecializationId",
                table: "Appointments",
                column: "SpecializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Hospitals_HospitalId",
                table: "Appointments",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Specializations_SpecializationId",
                table: "Appointments",
                column: "SpecializationId",
                principalTable: "Specializations",
                principalColumn: "SpecializationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Hospitals_HospitalId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Specializations_SpecializationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_HospitalId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SpecializationId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BookingSource",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 1,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 2,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 3,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 4,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 5,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 6,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 7,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 8,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hospitals",
                keyColumn: "HospitalId",
                keyValue: 9,
                column: "WebsiteUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 10,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 11,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 12,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 13,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 14,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 15,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 16,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 17,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 18,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 19,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 20,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 21,
                column: "Password",
                value: "$2a$11$mC8m7LqC9rQo3JvFpUqRBeL1fK8bVmxN5F2v==H7e5z6x8y9z0u1v2w3x4");
        }
    }
}
