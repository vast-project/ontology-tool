using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VAST.Ontology.Database.Migrations
{
    public partial class AddNegativeVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Negative",
                table: "Votes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Negative",
                table: "Votes");
        }
    }
}
