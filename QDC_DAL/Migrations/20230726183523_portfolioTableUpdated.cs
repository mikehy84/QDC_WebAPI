using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QDC_DAL.Migrations
{
    /// <inheritdoc />
    public partial class portfolioTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioItems");

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjImg = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IsArchive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsArchive = table.Column<bool>(type: "bit", nullable: false),
                    ProjDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjImg = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ProjName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.Id);
                });
        }
    }
}
