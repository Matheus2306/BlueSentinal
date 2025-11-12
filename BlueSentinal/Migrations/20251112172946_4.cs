using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSentinal.Migrations
{
    /// <inheritdoc />
    public partial class _4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tempo",
                table: "Drones Usuários",
                newName: "TempoEmMili");

            migrationBuilder.AddColumn<decimal>(
                name: "tempoEmHoras",
                table: "Drones Usuários",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tempoEmHoras",
                table: "Drones Usuários");

            migrationBuilder.RenameColumn(
                name: "TempoEmMili",
                table: "Drones Usuários",
                newName: "Tempo");
        }
    }
}
