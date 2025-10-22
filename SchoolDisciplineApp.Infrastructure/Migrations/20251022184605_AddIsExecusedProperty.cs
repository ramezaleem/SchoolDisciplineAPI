using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolDisciplineApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsExecusedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up ( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExcused",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down ( MigrationBuilder migrationBuilder )
        {
            migrationBuilder.DropColumn(
                name: "IsExcused",
                table: "AttendanceRecords");
        }
    }
}
