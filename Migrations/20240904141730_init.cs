using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI3.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PackageDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sheet = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FacesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VoicesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageId);
                    table.ForeignKey(
                        name: "FK_Packages_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sequances",
                columns: table => new
                {
                    SequanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SequanceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SequanceDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sheet = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FacesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VoicesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Package_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sequances", x => x.SequanceId);
                    table.ForeignKey(
                        name: "FK_Sequances_Packages_Package_Id",
                        column: x => x.Package_Id,
                        principalTable: "Packages",
                        principalColumn: "PackageId");
                    table.ForeignKey(
                        name: "FK_Sequances_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SessionPlace = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SessionDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sheet = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FacesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    VoicesFolder = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Sequance_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sessions_Sequances_Sequance_Id",
                        column: x => x.Sequance_Id,
                        principalTable: "Sequances",
                        principalColumn: "SequanceId");
                    table.ForeignKey(
                        name: "FK_Sessions_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    AttendanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Session_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.AttendanceId);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Sessions_Session_Id",
                        column: x => x.Session_Id,
                        principalTable: "Sessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_Session_Id",
                table: "AttendanceRecords",
                column: "Session_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_User_Id",
                table: "AttendanceRecords",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_User_Id",
                table: "Packages",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sequances_Package_Id",
                table: "Sequances",
                column: "Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sequances_User_Id",
                table: "Sequances",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Sequance_Id",
                table: "Sessions",
                column: "Sequance_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_User_Id",
                table: "Sessions",
                column: "User_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Sequances");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
