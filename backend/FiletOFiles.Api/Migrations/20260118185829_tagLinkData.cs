using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiletOFiles.Api.Migrations
{
    /// <inheritdoc />
    public partial class tagLinkData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "additional_data",
                table: "recipe_tag",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additional_data",
                table: "recipe_tag");
        }
    }
}
