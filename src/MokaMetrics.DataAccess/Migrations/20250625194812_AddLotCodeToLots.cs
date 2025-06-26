using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MokaMetrics.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLotCodeToLots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LotCode",
                table: "Lots",
                type: "varchar",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotCode",
                table: "Lots");
        }
    }
}
