using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceAPI3.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionName",
                table: "Sessions",
                column: "SessionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sequances_SequanceName",
                table: "Sequances",
                column: "SequanceName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PackageName",
                table: "Packages",
                column: "PackageName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_SessionName",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sequances_SequanceName",
                table: "Sequances");

            migrationBuilder.DropIndex(
                name: "IX_Packages_PackageName",
                table: "Packages");
        }
    }
}
