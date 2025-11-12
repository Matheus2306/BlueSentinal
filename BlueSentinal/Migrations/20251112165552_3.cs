using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueSentinal.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drone Fabrica_Tempo de Atividade_TempoDeAtividadeId",
                table: "Drone Fabrica");

            migrationBuilder.DropTable(
                name: "Tempo de Atividade");

            migrationBuilder.DropIndex(
                name: "IX_Drone Fabrica_TempoDeAtividadeId",
                table: "Drone Fabrica");

            migrationBuilder.DropColumn(
                name: "TempoDeAtividade",
                table: "Drone Fabrica");

            migrationBuilder.DropColumn(
                name: "TempoDeAtividadeId",
                table: "Drone Fabrica");

            migrationBuilder.AddColumn<long>(
                name: "Tempo",
                table: "Drones Usuários",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tempo",
                table: "Drones Usuários");

            migrationBuilder.AddColumn<Guid>(
                name: "TempoDeAtividade",
                table: "Drone Fabrica",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TempoDeAtividadeId",
                table: "Drone Fabrica",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tempo de Atividade",
                columns: table => new
                {
                    TempoDeAtividadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tempo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tempo de Atividade", x => x.TempoDeAtividadeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drone Fabrica_TempoDeAtividadeId",
                table: "Drone Fabrica",
                column: "TempoDeAtividadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drone Fabrica_Tempo de Atividade_TempoDeAtividadeId",
                table: "Drone Fabrica",
                column: "TempoDeAtividadeId",
                principalTable: "Tempo de Atividade",
                principalColumn: "TempoDeAtividadeId");
        }
    }
}
