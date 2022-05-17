using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    public partial class AddItemSchemaFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInSchema",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInSchema",
                table: "Items");
        }
    }
}
