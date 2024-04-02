using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IMDBLib.Migrations
{
    /// <inheritdoc />
    public partial class nr1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crews",
                columns: table => new
                {
                    Nconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PrimaryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeathYear = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crews", x => x.Nconst);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TitleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crew_Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrewNconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crew_Professions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crew_Professions_Crews_CrewNconst",
                        column: x => x.CrewNconst,
                        principalTable: "Crews",
                        principalColumn: "Nconst",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Crew_Professions_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Tconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitleTypeId = table.Column<int>(type: "int", nullable: false),
                    PrimaryTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: false),
                    RuntimeMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Tconst);
                    table.ForeignKey(
                        name: "FK_Titles_TitleTypes_TitleTypeId",
                        column: x => x.TitleTypeId,
                        principalTable: "TitleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreTitle",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "int", nullable: false),
                    TitlesTconst = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreTitle", x => new { x.GenresId, x.TitlesTconst });
                    table.ForeignKey(
                        name: "FK_GenreTitle_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreTitle_Titles_TitlesTconst",
                        column: x => x.TitlesTconst,
                        principalTable: "Titles",
                        principalColumn: "Tconst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Know_For_Titles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrewNconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TitleTconst = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Know_For_Titles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Know_For_Titles_Crews_CrewNconst",
                        column: x => x.CrewNconst,
                        principalTable: "Crews",
                        principalColumn: "Nconst",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Know_For_Titles_Titles_TitleTconst",
                        column: x => x.TitleTconst,
                        principalTable: "Titles",
                        principalColumn: "Tconst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Title_Crews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleTconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Title_Crews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Title_Crews_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Title_Crews_Titles_TitleTconst",
                        column: x => x.TitleTconst,
                        principalTable: "Titles",
                        principalColumn: "Tconst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Title_Genres",
                columns: table => new
                {
                    TitleTconst = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Title_Genres", x => new { x.TitleTconst, x.GenreId });
                    table.ForeignKey(
                        name: "FK_Title_Genres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Title_Genres_Titles_TitleTconst",
                        column: x => x.TitleTconst,
                        principalTable: "Titles",
                        principalColumn: "Tconst",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title_CrewId = table.Column<int>(type: "int", nullable: false),
                    DirectorNconst = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directors_Crews_DirectorNconst",
                        column: x => x.DirectorNconst,
                        principalTable: "Crews",
                        principalColumn: "Nconst",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Directors_Title_Crews_Title_CrewId",
                        column: x => x.Title_CrewId,
                        principalTable: "Title_Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Writers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title_CrewId = table.Column<int>(type: "int", nullable: false),
                    WriterNconst = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Writers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Writers_Crews_WriterNconst",
                        column: x => x.WriterNconst,
                        principalTable: "Crews",
                        principalColumn: "Nconst",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Writers_Title_Crews_Title_CrewId",
                        column: x => x.Title_CrewId,
                        principalTable: "Title_Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crew_Professions_CrewNconst",
                table: "Crew_Professions",
                column: "CrewNconst");

            migrationBuilder.CreateIndex(
                name: "IX_Crew_Professions_JobId",
                table: "Crew_Professions",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_DirectorNconst",
                table: "Directors",
                column: "DirectorNconst");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_Title_CrewId",
                table: "Directors",
                column: "Title_CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreTitle_TitlesTconst",
                table: "GenreTitle",
                column: "TitlesTconst");

            migrationBuilder.CreateIndex(
                name: "IX_Know_For_Titles_CrewNconst",
                table: "Know_For_Titles",
                column: "CrewNconst");

            migrationBuilder.CreateIndex(
                name: "IX_Know_For_Titles_TitleTconst",
                table: "Know_For_Titles",
                column: "TitleTconst");

            migrationBuilder.CreateIndex(
                name: "IX_Title_Crews_JobId",
                table: "Title_Crews",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Title_Crews_TitleTconst",
                table: "Title_Crews",
                column: "TitleTconst",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Title_Genres_GenreId",
                table: "Title_Genres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Titles_TitleTypeId",
                table: "Titles",
                column: "TitleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Writers_Title_CrewId",
                table: "Writers",
                column: "Title_CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_Writers_WriterNconst",
                table: "Writers",
                column: "WriterNconst");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Crew_Professions");

            migrationBuilder.DropTable(
                name: "Directors");

            migrationBuilder.DropTable(
                name: "GenreTitle");

            migrationBuilder.DropTable(
                name: "Know_For_Titles");

            migrationBuilder.DropTable(
                name: "Title_Genres");

            migrationBuilder.DropTable(
                name: "Writers");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Crews");

            migrationBuilder.DropTable(
                name: "Title_Crews");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "TitleTypes");
        }
    }
}
