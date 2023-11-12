using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalId",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryContextId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Context",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Context", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContextItem",
                columns: table => new
                {
                    ContextsId = table.Column<int>(type: "integer", nullable: false),
                    SecondaryItemsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContextItem", x => new { x.ContextsId, x.SecondaryItemsId });
                    table.ForeignKey(
                        name: "FK_ContextItem_Context_ContextsId",
                        column: x => x.ContextsId,
                        principalTable: "Context",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContextItem_Items_SecondaryItemsId",
                        column: x => x.SecondaryItemsId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_PrimaryContextId",
                table: "Items",
                column: "PrimaryContextId");

            migrationBuilder.CreateIndex(
                name: "IX_ContextItem_SecondaryItemsId",
                table: "ContextItem",
                column: "SecondaryItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Context_PrimaryContextId",
                table: "Items",
                column: "PrimaryContextId",
                principalTable: "Context",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Context_PrimaryContextId",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ContextItem");

            migrationBuilder.DropTable(
                name: "Context");

            migrationBuilder.DropIndex(
                name: "IX_Items_PrimaryContextId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "OriginalId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PrimaryContextId",
                table: "Items");
        }
    }
}
