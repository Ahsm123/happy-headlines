using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyHeadlines.ArticleApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Articles_PublishDate",
                table: "Articles",
                column: "PublishDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_PublishDate",
                table: "Articles");
        }
    }
}
