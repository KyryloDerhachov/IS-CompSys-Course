using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Add_NameUKR_To_Role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameUKR",
                table: "roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_roles_NameUKR",
                table: "roles",
                column: "NameUKR",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_NameUKR",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "NameUKR",
                table: "roles");
        }
    }
}
