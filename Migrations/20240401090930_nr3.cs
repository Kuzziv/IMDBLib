using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMDBLib.Migrations
{
    /// <inheritdoc />
    public partial class nr3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitleView");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TitleView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    endYear = table.Column<int>(type: "int", nullable: false),
                    genres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAdult = table.Column<bool>(type: "bit", nullable: false),
                    originalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    primaryTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    runtimeMinutes = table.Column<int>(type: "int", nullable: false),
                    startYear = table.Column<int>(type: "int", nullable: false),
                    tconst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    titleType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleView", x => x.Id);
                });
        }
    }
}
