using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Unique_Indexes_On_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_roles_NameUKR",
                table: "roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_roles_NameUKR",
                table: "roles",
                column: "NameUKR",
                unique: true);
        }
    }
}
