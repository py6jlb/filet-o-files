using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiletOFiles.Api.Migrations
{
    /// <inheritdoc />
    public partial class pdfPreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "preview_file_id",
                table: "files",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_files_preview_file_id",
                table: "files",
                column: "preview_file_id");

            migrationBuilder.AddForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files",
                column: "preview_file_id",
                principalTable: "files",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "ix_files_preview_file_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "preview_file_id",
                table: "files");
        }
    }
}
