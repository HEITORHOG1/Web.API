using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnderecoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HorarioFuncionamento_Estabelecimentos_EstabelecimentoId",
                table: "HorarioFuncionamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HorarioFuncionamento",
                table: "HorarioFuncionamento");

            migrationBuilder.RenameTable(
                name: "HorarioFuncionamento",
                newName: "HorariosFuncionamento");

            migrationBuilder.RenameIndex(
                name: "IX_HorarioFuncionamento_EstabelecimentoId",
                table: "HorariosFuncionamento",
                newName: "IX_HorariosFuncionamento_EstabelecimentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HorariosFuncionamento",
                table: "HorariosFuncionamento",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EnderecoClientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logradouro = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Numero = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Complemento = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Bairro = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cidade = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CEP = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Principal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnderecoClientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnderecoClientes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EnderecoClientes_UsuarioId",
                table: "EnderecoClientes",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_HorariosFuncionamento_Estabelecimentos_EstabelecimentoId",
                table: "HorariosFuncionamento",
                column: "EstabelecimentoId",
                principalTable: "Estabelecimentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HorariosFuncionamento_Estabelecimentos_EstabelecimentoId",
                table: "HorariosFuncionamento");

            migrationBuilder.DropTable(
                name: "EnderecoClientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HorariosFuncionamento",
                table: "HorariosFuncionamento");

            migrationBuilder.RenameTable(
                name: "HorariosFuncionamento",
                newName: "HorarioFuncionamento");

            migrationBuilder.RenameIndex(
                name: "IX_HorariosFuncionamento_EstabelecimentoId",
                table: "HorarioFuncionamento",
                newName: "IX_HorarioFuncionamento_EstabelecimentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HorarioFuncionamento",
                table: "HorarioFuncionamento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HorarioFuncionamento_Estabelecimentos_EstabelecimentoId",
                table: "HorarioFuncionamento",
                column: "EstabelecimentoId",
                principalTable: "Estabelecimentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
