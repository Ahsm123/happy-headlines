using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HappyHeadlines.CommentApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCommentArticleIdToGuid : Migration
    {
        // Postgres can't cast integer to uuid, so drop and re-add; old int ids can't reference Guid articles anyway
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Comments");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
