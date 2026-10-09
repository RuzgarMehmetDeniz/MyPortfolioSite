using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioSite.Migrations
{
    /// <inheritdoc />
    public partial class AddOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Overrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepoName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CoverUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DemoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExtraImages = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overrides", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Overrides_RepoName",
                table: "Overrides",
                column: "RepoName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Overrides");
        }
    }
}
