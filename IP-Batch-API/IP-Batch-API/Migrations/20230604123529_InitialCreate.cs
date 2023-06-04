using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IP_Batch_API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IPDetail",
                columns: table => new
                {
                    Ip = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Continent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPDetail", x => x.Ip);
                });

            migrationBuilder.InsertData(
                table: "IPDetail",
                columns: new[] { "Ip", "City", "Continent", "Country", "Latitude", "Longitude" },
                values: new object[] { "0.0.0.0", "city name", "continent name", "country name", 0.0, 0.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IPDetail");
        }
    }
}
