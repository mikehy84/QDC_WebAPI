using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QDC_DAL.Migrations
{
    /// <inheritdoc />
    public partial class portfolioTableUpdated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProjName",
                table: "Portfolios",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProjImg",
                table: "Portfolios",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "ProjDesc",
                table: "Portfolios",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Portfolios",
                newName: "ProjName");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Portfolios",
                newName: "ProjImg");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Portfolios",
                newName: "ProjDesc");
        }
    }
}
