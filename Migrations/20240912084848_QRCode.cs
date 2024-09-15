using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI3.Migrations
{
    public partial class QRCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_User_Id",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Sessions_Session_Id",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "User_Id",
                table: "AttendanceRecords",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Session_Id",
                table: "AttendanceRecords",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_User_Id",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_Session_Id",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_SessionId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AttendanceRecords",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateTable(
                name: "SessionQRCodes",
                columns: table => new
                {
                    QRCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionQRCodes", x => x.QRCodeId);
                    table.ForeignKey(
                        name: "FK_SessionQRCodes_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessionQRCodes_SessionId",
                table: "SessionQRCodes",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_UserId",
                table: "AttendanceRecords",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Sessions_SessionId",
                table: "AttendanceRecords",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_UserId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Sessions_SessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "SessionQRCodes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AttendanceRecords",
                newName: "User_Id");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "AttendanceRecords",
                newName: "Session_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_UserId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_User_Id");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_SessionId",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_Session_Id");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AttendanceRecords",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_User_Id",
                table: "AttendanceRecords",
                column: "User_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Sessions_Session_Id",
                table: "AttendanceRecords",
                column: "Session_Id",
                principalTable: "Sessions",
                principalColumn: "SessionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
