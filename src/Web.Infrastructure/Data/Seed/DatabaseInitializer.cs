using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Infrastructure.Data.Context;

namespace Web.Infrastructure.Data.Seed
{
    public static class DatabaseInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            await ApplyMigrations(serviceProvider);
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new[]
            {
                UserRoles.Cliente,
                UserRoles.Proprietario,
                UserRoles.Funcionario,
                UserRoles.Administrador,
                UserRoles.Gerente,
                UserRoles.Atendente,
                UserRoles.Cozinheiro,
                UserRoles.Caixa,
                UserRoles.Garcom,
                UserRoles.Entregador
            };

            // Criar roles e suas permissões
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new IdentityRole(role);
                    await roleManager.CreateAsync(identityRole);

                    // Adicionar claims de permissão para a role
                    if (UserRoles.Permissions.TryGetValue(role, out var permissions))
                    {
                        foreach (var permission in permissions)
                        {
                            await roleManager.AddClaimAsync(identityRole, new Claim("permission", permission));
                        }
                    }
                }
                else
                {
                    // Atualizar claims de permissão para a role existente
                    var identityRole = await roleManager.FindByNameAsync(role);
                    var existingClaims = await roleManager.GetClaimsAsync(identityRole);

                    if (UserRoles.Permissions.TryGetValue(role, out var permissions))
                    {
                        foreach (var permission in permissions)
                        {
                            if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
                            {
                                await roleManager.AddClaimAsync(identityRole, new Claim("permission", permission));
                            }
                        }
                    }
                }
            }

            // Criar usuários padrão
            var defaultUsers = new[]
            {
                new DefaultUserInfo
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Password = "Admin@1234",
                    Role = UserRoles.Administrador,
                    NomeUsuario = "Administrador do Sistema",
                    CPF_CNPJ = "000.000.000-00",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "proprietario",
                    Email = "proprietario@example.com",
                    Password = "Proprietario@1234",
                    Role = UserRoles.Proprietario,
                    NomeUsuario = "Proprietário Teste",
                    CPF_CNPJ = "111.111.111-11",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "gerente",
                    Email = "gerente@example.com",
                    Password = "Gerente@1234",
                    Role = UserRoles.Gerente,
                    NomeUsuario = "Gerente Teste",
                    CPF_CNPJ = "222.222.222-22",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "atendente",
                    Email = "atendente@example.com",
                    Password = "Atendente@1234",
                    Role = UserRoles.Atendente,
                    NomeUsuario = "Atendente Teste",
                    CPF_CNPJ = "333.333.333-33",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "cozinheiro",
                    Email = "cozinheiro@example.com",
                    Password = "Cozinheiro@1234",
                    Role = UserRoles.Cozinheiro,
                    NomeUsuario = "Cozinheiro Teste",
                    CPF_CNPJ = "444.444.444-44",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "caixa",
                    Email = "caixa@example.com",
                    Password = "Caixa@1234",
                    Role = UserRoles.Caixa,
                    NomeUsuario = "Caixa Teste",
                    CPF_CNPJ = "555.555.555-55",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "garcom",
                    Email = "garcom@example.com",
                    Password = "Garcom@1234",
                    Role = UserRoles.Garcom,
                    NomeUsuario = "Garçom Teste",
                    CPF_CNPJ = "666.666.666-66",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "entregador",
                    Email = "entregador@example.com",
                    Password = "Entregador@1234",
                    Role = UserRoles.Entregador,
                    NomeUsuario = "Entregador Teste",
                    CPF_CNPJ = "777.777.777-77",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "funcionario",
                    Email = "funcionario@example.com",
                    Password = "Funcionario@1234",
                    Role = UserRoles.Funcionario,
                    NomeUsuario = "Funcionário Teste",
                    CPF_CNPJ = "888.888.888-88",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "cliente",
                    Email = "cliente@example.com",
                    Password = "Cliente@1234",
                    Role = UserRoles.Cliente,
                    NomeUsuario = "Cliente Teste",
                    CPF_CNPJ = "999.999.999-99",
                    Ativo = true,
                },
                // Usuários adicionais para teste
                new DefaultUserInfo
                {
                    Username = "cliente2",
                    Email = "cliente2@example.com",
                    Password = "Cliente@1234",
                    Role = UserRoles.Cliente,
                    NomeUsuario = "Cliente Teste 2",
                    CPF_CNPJ = "123.456.789-00",
                    Ativo = true
                },
                new DefaultUserInfo
                {
                    Username = "proprietario2",
                    Email = "proprietario2@example.com",
                    Password = "Proprietario@1234",
                    Role = UserRoles.Proprietario,
                    NomeUsuario = "Proprietário Teste 2",
                    CPF_CNPJ = "987.654.321-00",
                    Ativo = true
                }
            };

            // Criar todos os usuários padrão
            foreach (var userInfo in defaultUsers)
            {
                await CreateDefaultUser(userManager, userInfo);
            }
        }

        private static async Task ApplyMigrations(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            try
            {
                // Verifica se existem migrations pendentes
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var pendingMigrationsList = pendingMigrations.ToList();

                if (pendingMigrationsList.Any())
                {
                    logger.LogInformation("Começando a aplicar {Count} migrations pendentes...", pendingMigrationsList.Count);

                    foreach (var migration in pendingMigrationsList)
                    {
                        logger.LogInformation("Aplicando migration: {Migration}", migration);
                    }

                    // Aplica todas as migrations pendentes
                    await context.Database.MigrateAsync();

                    logger.LogInformation("Migrations aplicadas com sucesso!");
                }
                else
                {
                    logger.LogInformation("Nenhuma migration pendente encontrada.");
                }

                // Lista todas as migrations já aplicadas
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                logger.LogInformation("Migrations já aplicadas: {Migrations}",
                    string.Join(", ", appliedMigrations));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao aplicar migrations");
                throw;
            }
        }

        private class DefaultUserInfo
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string NomeUsuario { get; set; }
            public string CPF_CNPJ { get; set; }
            public bool Ativo { get; set; }
        }

        private static async Task CreateDefaultUser(UserManager<ApplicationUser> userManager, DefaultUserInfo userInfo)
        {
            if (await userManager.FindByNameAsync(userInfo.Username) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userInfo.Username,
                    Email = userInfo.Email,
                    NomeUsuario = userInfo.NomeUsuario,
                    CPF_CNPJ = userInfo.CPF_CNPJ,
                    EmailConfirmed = true,
                    Ativo = userInfo.Ativo,
                    DataDeCadastro = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, userInfo.Password);
                if (result.Succeeded)
                {
                    // Apenas adiciona o role, sem adicionar claims de permissão
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                }
            }
        }

        private static async Task SeedEstabelecimentos(AppDbContext context)
        {
            if (!context.Estabelecimentos.Any())
            {
                var estabelecimentos = new List<Estabelecimento>
                {
                    new Estabelecimento
                    {
                        UsuarioId = "proprietario",
                        RazaoSocial = "Estabelecimento 1 LTDA",
                        NomeFantasia = "Estabelecimento 1",
                        CNPJ = "00.000.000/0001-00",
                        Telefone = "(11) 1111-1111",
                        Endereco = "Rua 1, 123",
                        Status = true,
                        DataCadastro = DateTime.UtcNow
                    },
                    new Estabelecimento
                    {
                        UsuarioId = "proprietario2",
                        RazaoSocial = "Estabelecimento 2 LTDA",
                        NomeFantasia = "Estabelecimento 2",
                        CNPJ = "00.000.000/0002-00",
                        Telefone = "(22) 2222-2222",
                        Endereco = "Rua 2, 456",
                        Status = true,
                        DataCadastro = DateTime.UtcNow
                    }
                };

                context.Estabelecimentos.AddRange(estabelecimentos);
                await context.SaveChangesAsync();
            }
        }

        //private static async Task SeedCategorias(AppDbContext context)
        //{
        //    if (!context.Categorias.Any())
        //    {
        //        var categorias = new List<Categoria>
        //        {
        //            new Categoria
        //            {
        //                Nome = "Bebidas",
        //                Descricao = "Todas as bebidas disponíveis",
        //                Ativo = true,
        //                EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 1").Id,
        //                DataCadastro = DateTime.UtcNow
        //            },
        //            new Categoria
        //            {
        //                Nome = "Lanches",
        //                Descricao = "Diversos tipos de lanches",
        //                Ativo = true,
        //                EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 1").Id,
        //                DataCadastro = DateTime.UtcNow
        //            },
        //            new Categoria
        //            {
        //                Nome = "Sobremesas",
        //                Descricao = "Sobremesas deliciosas",
        //                Ativo = true,
        //                EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 2").Id,
        //                DataCadastro = DateTime.UtcNow
        //            }
        //        };

        //        context.Categorias.AddRange(categorias);
        //        await context.SaveChangesAsync();
        //    }
        //}

        private static async Task SeedProdutos(AppDbContext context)
        {
            if (!context.Produtos.Any())
            {
                var produtos = new List<Produto>
                {
                    new Produto
                    {
                        Nome = "Coca-Cola",
                        Descricao = "Refrigerante de cola",
                        Preco = 5.00m,
                        Imagem = "coca-cola.jpg",
                        Disponivel = true,
                        CategoriaId = context.Categorias.First(c => c.Nome == "Bebidas").Id,
                        EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 1").Id,
                        DataCadastro = DateTime.UtcNow,
                        QuantidadeEmEstoque = 100,
                        QuantidadeReservada = 0
                    },
                    new Produto
                    {
                        Nome = "Hambúrguer",
                        Descricao = "Hambúrguer com queijo e bacon",
                        Preco = 15.00m,
                        Imagem = "hamburguer.jpg",
                        Disponivel = true,
                        CategoriaId = context.Categorias.First(c => c.Nome == "Lanches").Id,
                        EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 1").Id,
                        DataCadastro = DateTime.UtcNow,
                        QuantidadeEmEstoque = 50,
                        QuantidadeReservada = 0
                    },
                    new Produto
                    {
                        Nome = "Sorvete",
                        Descricao = "Sorvete de chocolate",
                        Preco = 10.00m,
                        Imagem = "sorvete.jpg",
                        Disponivel = true,
                        CategoriaId = context.Categorias.First(c => c.Nome == "Sobremesas").Id,
                        EstabelecimentoId = context.Estabelecimentos.First(e => e.NomeFantasia == "Estabelecimento 2").Id,
                        DataCadastro = DateTime.UtcNow,
                        QuantidadeEmEstoque = 30,
                        QuantidadeReservada = 0
                    }
                };

                context.Produtos.AddRange(produtos);
                await context.SaveChangesAsync();
            }
        }
    }
}