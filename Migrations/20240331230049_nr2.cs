using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMDBLib.Migrations
{
    /// <inheritdoc />
    public partial class nr2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TitleView",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tconst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    titleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    primaryTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    originalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAdult = table.Column<bool>(type: "bit", nullable: false),
                    startYear = table.Column<int>(type: "int", nullable: false),
                    endYear = table.Column<int>(type: "int", nullable: false),
                    runtimeMinutes = table.Column<int>(type: "int", nullable: false),
                    genres = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleView", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitleView");
        }
    }
}
