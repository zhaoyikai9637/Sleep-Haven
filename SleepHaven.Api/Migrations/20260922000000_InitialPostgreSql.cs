using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SleepHaven.Api.Data;

#nullable disable

namespace SleepHaven.Api.Migrations;

[DbContext(typeof(CatalogDbContext))]
[Migration("20260922000000_InitialPostgreSql")]
public partial class InitialPostgreSql : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "products",
            columns: table => new
            {
                Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                Currency = table.Column<int>(type: "integer", nullable: false),
                ProductType = table.Column<int>(type: "integer", nullable: false),
                Season = table.Column<int>(type: "integer", nullable: false),
                Material = table.Column<int>(type: "integer", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                ThumbnailUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                LandscapeUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_products", product => product.Id);
            });

        migrationBuilder.CreateTable(
            name: "favorites",
            columns: table => new
            {
                ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                ProductId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_favorites", favorite => new { favorite.ClientId, favorite.ProductId });
                table.ForeignKey(
                    name: "FK_favorites_products_ProductId",
                    column: favorite => favorite.ProductId,
                    principalTable: "products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_favorites_ProductId",
            table: "favorites",
            column: "ProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "favorites");
        migrationBuilder.DropTable(name: "products");
    }
}
