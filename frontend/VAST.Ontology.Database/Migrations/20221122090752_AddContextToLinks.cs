using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddContextToLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContextItem_Context_ContextsId",
                table: "ContextItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Context_PrimaryContextId",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Context",
                table: "Context");

            migrationBuilder.RenameTable(
                name: "Context",
                newName: "Contexts");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryContextId",
                table: "ItemLinks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contexts",
                table: "Contexts",  
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLinks_PrimaryContextId",
                table: "ItemLinks",
                column: "PrimaryContextId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContextItem_Contexts_ContextsId",
                table: "ContextItem",
                column: "ContextsId",
                principalTable: "Contexts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks",
                column: "PrimaryContextId",
                principalTable: "Contexts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Contexts_PrimaryContextId",
                table: "Items",
                column: "PrimaryContextId",
                principalTable: "Contexts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContextItem_Contexts_ContextsId",
                table: "ContextItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Contexts_PrimaryContextId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_ItemLinks_PrimaryContextId",
                table: "ItemLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contexts",
                table: "Contexts");

            migrationBuilder.DropColumn(
                name: "PrimaryContextId",
                table: "ItemLinks");

            migrationBuilder.RenameTable(
                name: "Contexts",
                newName: "Context");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Context",
                table: "Context",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContextItem_Context_ContextsId",
                table: "ContextItem",
                column: "ContextsId",
                principalTable: "Context",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Context_PrimaryContextId",
                table: "Items",
                column: "PrimaryContextId",
                principalTable: "Context",
                principalColumn: "Id");
        }
    }
}
