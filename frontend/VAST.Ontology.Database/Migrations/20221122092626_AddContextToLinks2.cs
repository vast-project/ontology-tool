using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddContextToLinks2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks",
                column: "PrimaryContextId",
                principalTable: "Contexts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLinks_Contexts_PrimaryContextId",
                table: "ItemLinks",
                column: "PrimaryContextId",
                principalTable: "Contexts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
