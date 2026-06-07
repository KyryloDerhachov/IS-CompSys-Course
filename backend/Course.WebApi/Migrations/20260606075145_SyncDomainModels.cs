using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SyncDomainModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wastage_items");

            migrationBuilder.DropTable(
                name: "wastage_reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wastage_reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReportNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalLossAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wastage_reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wastage_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Reason = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    WastageReportId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wastage_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wastage_items_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wastage_items_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_wastage_items_wastage_reports_WastageReportId",
                        column: x => x.WastageReportId,
                        principalTable: "wastage_reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wastage_items_BatchId",
                table: "wastage_items",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_wastage_items_ProductId",
                table: "wastage_items",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_wastage_items_WastageReportId",
                table: "wastage_items",
                column: "WastageReportId");

            migrationBuilder.CreateIndex(
                name: "IX_wastage_reports_ReportNumber",
                table: "wastage_reports",
                column: "ReportNumber",
                unique: true);
        }
    }
}
