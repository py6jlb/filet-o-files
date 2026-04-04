using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiletOFiles.Api.Migrations
{
    /// <inheritdoc />
    public partial class pdfPreviewFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files");

            migrationBuilder.AddForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files",
                column: "preview_file_id",
                principalTable: "files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files");

            migrationBuilder.AddForeignKey(
                name: "fk_files_files_preview_file_id",
                table: "files",
                column: "preview_file_id",
                principalTable: "files",
                principalColumn: "id");
        }
    }
}
