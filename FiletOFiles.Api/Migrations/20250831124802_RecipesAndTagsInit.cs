using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiletOFiles.Api.Migrations
{
    /// <inheritdoc />
    public partial class RecipesAndTagsInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tags",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Recipes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Descriptions",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Recipes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Descriptions",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Recipes");
        }
    }
}
