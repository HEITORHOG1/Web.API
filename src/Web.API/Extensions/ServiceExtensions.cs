using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Infrastructure.Data.Context;

namespace Web.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManageEstabelecimento", policy =>
                     policy.RequireAssertion(context =>
                         context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Administrador") &&
                         context.User.HasClaim("permission", "estabelecimento.gerenciar")
                     ));

                // Política para gerenciar categorias
                options.AddPolicy("ManageCategorias", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "cardapio.gerenciar")
                    ));

                // Política para visualizar categorias
                options.AddPolicy("ViewCategorias", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente") ||
                         context.User.IsInRole("Atendente")) &&
                        context.User.HasClaim("permission", "cardapio.visualizar")
                    ));

                // Política para ações do cliente
                options.AddPolicy("ManageCarrinho", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Cliente") &&
                        context.User.HasClaim("permission", "carrinho.gerenciar")
                    ));

                options.AddPolicy("ViewPedidos", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Cliente") &&
                        context.User.HasClaim("permission", "pedidos.visualizar")
                    ));

                options.AddPolicy("RealizarCompras", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Cliente") &&
                        context.User.HasClaim("permission", "pedidos.criar")
                    ));

                // Política para gerenciamento de fornecedores
                options.AddPolicy("ManageFornecedores", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "fornecedores.gerenciar")
                    ));

                options.AddPolicy("ViewFornecedores", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "fornecedores.visualizar")
                    ));

                // Política para gerenciamento de estoque
                options.AddPolicy("ManageEstoque", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "estoque.gerenciar")
                    ));

                options.AddPolicy("ViewEstoque", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "estoque.visualizar")
                    ));

                // Política para gerenciamento de notas fiscais
                options.AddPolicy("ManageNotasFiscais", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "notasfiscais.gerenciar")
                    ));

                options.AddPolicy("ViewNotasFiscais", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "notasfiscais.visualizar")
                    ));

                // Política para gerenciamento de produtos
                options.AddPolicy("ManageProdutos", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "produtos.gerenciar")
                    ));

                options.AddPolicy("ViewProdutos", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente")) &&
                        context.User.HasClaim("permission", "produtos.visualizar")
                    ));

                // Política para gerenciamento de pedidos
                options.AddPolicy("ManagePedidos", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente") ||
                         context.User.IsInRole("Cliente")) &&
                        context.User.HasClaim("permission", "pedidos.gerenciar")
                    ));

                options.AddPolicy("ViewPedidos", policy =>
                    policy.RequireAssertion(context =>
                        (context.User.IsInRole("Proprietario") ||
                         context.User.IsInRole("Gerente") ||
                         context.User.IsInRole("Cliente")) &&
                        context.User.HasClaim("permission", "pedidos.visualizar")
                    ));
                options.AddPolicy("ManageCategorias", policy =>
                        policy.RequireAssertion(context =>
                            (context.User.IsInRole("Proprietario") || context.User.IsInRole("Gerente")) &&
                            context.User.HasClaim("permission", "cardapio.gerenciar")
                        ));

                // Políticas baseadas em Roles
                options.AddPolicy("RequireProprietario", policy =>
                    policy.RequireRole(UserRoles.Proprietario));

                options.AddPolicy("RequireGerente", policy =>
                    policy.RequireRole(UserRoles.Gerente));

                options.AddPolicy("RequireAdministrador", policy =>
                    policy.RequireRole(UserRoles.Administrador));

                options.AddPolicy("RequireCliente", policy =>
                    policy.RequireRole(UserRoles.Cliente));

                // Políticas baseadas em Permissões
                options.AddPolicy("GerenciarEstabelecimento", policy =>
                    policy.RequireClaim("permission", "estabelecimento.gerenciar"));

                options.AddPolicy("GerenciarFuncionarios", policy =>
                    policy.RequireClaim("permission", "funcionarios.gerenciar"));

                options.AddPolicy("GerenciarCardapio", policy =>
                    policy.RequireClaim("permission", "cardapio.gerenciar"));

                options.AddPolicy("VisualizarFinanceiro", policy =>
                    policy.RequireClaim("permission", "financeiro.visualizar"));

                options.AddPolicy("VisualizarRelatorios", policy =>
                    policy.RequireClaim("permission", "relatorios.visualizar"));

                options.AddPolicy("GerenciarPedidos", policy =>
                    policy.RequireClaim("permission", "pedidos.gerenciar"));

                options.AddPolicy("GerenciarEstoque", policy =>
                    policy.RequireClaim("permission", "estoque.gerenciar"));

                options.AddPolicy("GerenciarFornecedores", policy =>
                    policy.RequireClaim("permission", "fornecedores.gerenciar"));

                options.AddPolicy("VisualizarFornecedores", policy =>
                    policy.RequireClaim("permission", "fornecedores.visualizar"));

                options.AddPolicy("GerenciarNotasFiscais", policy =>
                    policy.RequireClaim("permission", "notasfiscais.gerenciar"));

                options.AddPolicy("VisualizarNotasFiscais", policy =>
                    policy.RequireClaim("permission", "notasfiscais.visualizar"));

                options.AddPolicy("GerenciarProdutos", policy =>
                    policy.RequireClaim("permission", "produtos.gerenciar"));

                options.AddPolicy("VisualizarProdutos", policy =>
                    policy.RequireClaim("permission", "produtos.visualizar"));
            });
        }

        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Aumentar o timeout para operações iniciais
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 33)),
                    mySqlOptions => mySqlOptions
                        .EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null)
                        .CommandTimeout(300)
                )
            );
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            });
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Verifique que a chave corresponde exatamente à usada na geração do token
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<ApplicationUser>>();

                        var userId = context.Principal.FindFirstValue("UserId");
                        if (string.IsNullOrEmpty(userId))
                        {
                            context.Fail("User ID claim is missing in token");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user == null || !user.Ativo)
                        {
                            context.Fail("Usuário inválido ou inativo");
                            return;
                        }
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }
    }
}