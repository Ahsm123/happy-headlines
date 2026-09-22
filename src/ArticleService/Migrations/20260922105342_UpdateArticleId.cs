using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArticleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // int identity -> uuid is not a cast; the column is dropped and recreated.
            migrationBuilder.DropPrimaryKey(name: "PK_Articles", table: "Articles");
            migrationBuilder.DropColumn(name: "Id", table: "Articles");
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Articles",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");
            migrationBuilder.AddPrimaryKey(name: "PK_Articles", table: "Articles", column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(name: "PK_Articles", table: "Articles");
            migrationBuilder.DropColumn(name: "Id", table: "Articles");
            migrationBuilder.AddColumn<int>(name: "Id", table: "Articles", type: "integer", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy",
                    Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            migrationBuilder.AddPrimaryKey(name: "PK_Articles", table: "Articles", column: "Id");
        }
    }
}
