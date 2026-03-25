using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitOfMeasureSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UnitsOfMeasure",
                columns: new[] { "Id", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, "Штука", "шт" },
                    { 2, "Кілограм", "кг" },
                    { 3, "Літр", "л" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UnitsOfMeasure",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
