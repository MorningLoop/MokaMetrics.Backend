using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MokaMetrics.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMachineCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Machines",
                type: "varchar",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Machines");
        }
    }
}
