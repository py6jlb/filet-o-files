using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiletOFiles.Api.Migrations.OpenIdDb
{
    /// <inheritdoc />
    public partial class initOpenId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenIddictApplications",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    application_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    client_id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    client_secret = table.Column<string>(type: "TEXT", nullable: true),
                    client_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    concurrency_token = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    consent_type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    display_name = table.Column<string>(type: "TEXT", nullable: true),
                    display_names = table.Column<string>(type: "TEXT", nullable: true),
                    json_web_key_set = table.Column<string>(type: "TEXT", nullable: true),
                    permissions = table.Column<string>(type: "TEXT", nullable: true),
                    post_logout_redirect_uris = table.Column<string>(type: "TEXT", nullable: true),
                    properties = table.Column<string>(type: "TEXT", nullable: true),
                    redirect_uris = table.Column<string>(type: "TEXT", nullable: true),
                    requirements = table.Column<string>(type: "TEXT", nullable: true),
                    settings = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_applications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictScopes",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    concurrency_token = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    descriptions = table.Column<string>(type: "TEXT", nullable: true),
                    display_name = table.Column<string>(type: "TEXT", nullable: true),
                    display_names = table.Column<string>(type: "TEXT", nullable: true),
                    name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "TEXT", nullable: true),
                    resources = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_scopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictAuthorizations",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    application_id = table.Column<string>(type: "TEXT", nullable: true),
                    concurrency_token = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    properties = table.Column<string>(type: "TEXT", nullable: true),
                    scopes = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_authorizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_authorizations_open_iddict_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "OpenIddictTokens",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    application_id = table.Column<string>(type: "TEXT", nullable: true),
                    authorization_id = table.Column<string>(type: "TEXT", nullable: true),
                    concurrency_token = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    creation_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    payload = table.Column<string>(type: "TEXT", nullable: true),
                    properties = table.Column<string>(type: "TEXT", nullable: true),
                    redemption_date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    reference_id = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_open_iddict_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_applications_application_id",
                        column: x => x.application_id,
                        principalTable: "OpenIddictApplications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_open_iddict_tokens_open_iddict_authorizations_authorization_id",
                        column: x => x.authorization_id,
                        principalTable: "OpenIddictAuthorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_applications_client_id",
                table: "OpenIddictApplications",
                column: "client_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_authorizations_application_id_status_subject_type",
                table: "OpenIddictAuthorizations",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_scopes_name",
                table: "OpenIddictScopes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_application_id_status_subject_type",
                table: "OpenIddictTokens",
                columns: new[] { "application_id", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_authorization_id",
                table: "OpenIddictTokens",
                column: "authorization_id");

            migrationBuilder.CreateIndex(
                name: "ix_open_iddict_tokens_reference_id",
                table: "OpenIddictTokens",
                column: "reference_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenIddictScopes");

            migrationBuilder.DropTable(
                name: "OpenIddictTokens");

            migrationBuilder.DropTable(
                name: "OpenIddictAuthorizations");

            migrationBuilder.DropTable(
                name: "OpenIddictApplications");
        }
    }
}
