using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstabelecimentoGeo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HorarioFuncionamento",
                table: "Estabelecimentos",
                newName: "Numero");

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "Estabelecimentos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Estabelecimentos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Estabelecimentos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RaioEntregaKm",
                table: "Estabelecimentos",
                type: "double",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HorarioFuncionamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EstabelecimentoId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraAbertura = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    HoraFechamento = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorarioFuncionamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorarioFuncionamento_Estabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "Estabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HorarioFuncionamento_EstabelecimentoId",
                table: "HorarioFuncionamento",
                column: "EstabelecimentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorarioFuncionamento");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "Estabelecimentos");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Estabelecimentos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Estabelecimentos");

            migrationBuilder.DropColumn(
                name: "RaioEntregaKm",
                table: "Estabelecimentos");

            migrationBuilder.RenameColumn(
                name: "Numero",
                table: "Estabelecimentos",
                newName: "HorarioFuncionamento");
        }
    }
}