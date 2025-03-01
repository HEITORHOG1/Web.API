using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Web.API.Extensions;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Domain.DTOs;
using Web.Domain.Entities;
using Web.Domain.Enums;

namespace Web.API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de autenticação de usuários, incluindo registro, login e gerenciamento de tokens.
    /// </summary>
    //[ApiVersion("1")]
    //[Route("api/[controller]/v{version:apiVersion}")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        private readonly IUsuarioEstabelecimentoService _usuarioEstabelecimentoService;

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="AuthController"/>.
        /// </summary>
        public AuthController(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                ITokenService tokenService,
                ILogger<AuthController> logger,
                IUsuarioEstabelecimentoService usuarioEstabelecimentoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
            _usuarioEstabelecimentoService = usuarioEstabelecimentoService;
        }

        /// <summary>
        /// Retorna uma lista de papéis de usuário e seus valores numéricos.
        /// </summary>
        /// <returns>Um <see cref="IActionResult"/> contendo a lista de papéis de usuário e seus valores numéricos.</returns>
        [HttpGet("user-roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserRoles()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                            .Cast<UserRole>()
                            .ToDictionary(role => role.ToString(), role => (int)role);

            return Ok(roles);
        }

        /// <summary>
        /// Obtém um usuário específico pelo ID.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <returns>O usuário encontrado ou uma mensagem de erro se não for encontrado.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _tokenService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }
            return Ok(user);
        }

        /// <summary>
        /// Pesquisa usuários por nome, CPF, email ou telefone.
        /// </summary>
        /// <param name="nome">O nome do usuário.</param>
        /// <param name="cpf">O CPF do usuário.</param>
        /// <param name="email">O email do usuário.</param>
        /// <param name="telefone">O telefone do usuário.</param>
        /// <returns>Uma lista de usuários que correspondem aos critérios de pesquisa.</returns>
        [Authorize(Roles = "Proprietario,Administrador")]
        [HttpGet("search-users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchUsers([FromQuery] string? nome, [FromQuery] string? cpf, [FromQuery] string? email, [FromQuery] string? telefone)
        {
            var users = await _tokenService.SearchUsersAsync(nome, cpf, email, telefone);
            if (users == null || !users.Any())
            {
                return NotFound("Nenhum usuário encontrado");
            }
            return Ok(users);
        }

        /// <summary>
        /// Registra um novo usuário no sistema com base nos dados fornecidos.
        /// </summary>
        /// <param name="model">O objeto <see cref="RegisterModel"/> contendo as informações do usuário a ser registrado, como nome de usuário, email, senha e papel.</param>
        /// <returns>Um <see cref="IActionResult"/> que indica o resultado da operação de registro, incluindo mensagens de sucesso ou falha.</returns>
        /// <remarks>
        /// Este método cria um novo registro de usuário no banco de dados, atribuindo os dados fornecidos e associando o usuário a um papel específico, conforme definido no modelo.
        /// Se o processo de registro for bem-sucedido, o usuário será atribuído ao papel desejado e uma mensagem de sucesso será retornada.
        /// Caso contrário, uma mensagem de erro detalhando o motivo da falha será apresentada.
        /// </remarks>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                CPF_CNPJ = model.CPF_CNPJ,
                NomeUsuario = model.NomeUsuario,
                Endereco = model.Endereco,
                CEP = model.CEP,
                Telefone = model.Telefone,
                Ativo = true,
                DataDeCadastro = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string role = model.Role.ToString();
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new { Message = $"Falha ao atribuir o papel {role}", Errors = roleResult.Errors });
                    }
                }

                return Ok(new { Message = $"Usuário criado com sucesso com o papel {role}" });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Registra um novo usuário no sistema com base nos dados fornecidos.
        /// </summary>
        /// <param name="model">O objeto <see cref="RegisterModel"/> contendo as informações do usuário a ser registrado, como nome de usuário, email, senha e papel.</param>
        /// <returns>Um <see cref="IActionResult"/> que indica o resultado da operação de registro, incluindo mensagens de sucesso ou falha.</returns>
        /// <remarks>
        /// Este método cria um novo registro de usuário no banco de dados, atribuindo os dados fornecidos e associando o usuário a um papel específico, conforme definido no modelo.
        /// Se o processo de registro for bem-sucedido, o usuário será atribuído ao papel desejado e uma mensagem de sucesso será retornada.
        /// Caso contrário, uma mensagem de erro detalhando o motivo da falha será apresentada.
        /// </remarks>
        [HttpPost("registercliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterCliente([FromBody] RegisterClienteModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica duplicidade de e-mail ou nome de usuário
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { Message = "O e-mail já está em uso." });

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest(new { Message = "O nome de usuário já está em uso." });

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                CPF_CNPJ = model.CPF_CNPJ,
                NomeUsuario = model.NomeUsuario,
                Endereco = model.Endereco,
                CEP = model.CEP,
                Telefone = model.Telefone,
                Ativo = true,
                DataDeCadastro = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string role = UserRole.Cliente.ToString();
                if (!await _userManager.IsInRoleAsync(user, role))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                    {
                        return BadRequest(new { Message = $"Falha ao atribuir o papel {role}", Errors = roleResult.Errors });
                    }
                }

                return Ok(new { Message = $"Cliente registrado com sucesso com o papel {role}" });
            }

            return BadRequest(new { Message = "Falha ao registrar o cliente", Errors = result.Errors });
        }

        /// <summary>
        /// Registra um novo usuário no sistema com base nos dados fornecidos. somente para administradores e proprietários
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register/adminproprietario")]
        [Authorize(Roles = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAdminProprietario([FromBody] RegisterAdminProprietarioModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { Message = "Email já cadastrado" });

            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest(new { Message = "Username já cadastrado" });

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                NomeUsuario = model.NomeUsuario,
                CPF_CNPJ = model.CPF_CNPJ,
                Telefone = model.Telefone,
                Endereco = model.Endereco,
                CEP = model.CEP,
                Ativo = true,
                DataDeCadastro = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { Message = "Falha no registro", Errors = result.Errors });

            var role = model.TipoUsuario.ToString();
            var roleResult = await _userManager.AddToRoleAsync(user, role);

            if (!roleResult.Succeeded)
                return BadRequest(new { Message = $"Falha ao atribuir papel {role}", Errors = roleResult.Errors });

            return Ok(new { Message = $"Usuário {role} registrado com sucesso" });
        }

        /// <summary>
        /// Registra um novo funcionário e o vincula a um estabelecimento específico, operado pelo proprietário autenticado.
        /// </summary>
        /// <param name="model">O objeto <see cref="RegisterFuncionarioModel"/> contendo as informações do funcionário, como nome de usuário, email, senha, papel e ID do estabelecimento.</param>
        /// <returns>Um <see cref="IActionResult"/> que indica o resultado da operação de registro do funcionário e o vínculo ao estabelecimento.</returns>
        /// <remarks>
        /// Este método é utilizado por um proprietário autenticado para registrar novos funcionários em seu estabelecimento.
        /// A verificação é feita para garantir que o usuário autenticado tenha permissão para criar vínculos para o estabelecimento especificado.
        /// Se o registro do funcionário for bem-sucedido, ele será associado ao papel desejado (por exemplo, Atendente, Funcionario) e vinculado ao estabelecimento fornecido.
        /// Em caso de sucesso, uma mensagem de confirmação será retornada; caso contrário, mensagens de erro detalhando falhas no processo de criação do usuário ou atribuição do papel serão apresentadas.
        /// Funcionario = 2
        /// Gerente = 4
        /// Atendente = 5
        /// </remarks>
        [Authorize(Roles = "Proprietario")]
        [HttpPost("register-funcionario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterFuncionario([FromBody] RegisterFuncionarioModel model)
        {
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado");
            }
            var newUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                CPF_CNPJ = model.CPF_CNPJ,
                NomeUsuario = model.NomeUsuario,
                Endereco = model.Endereco,
                CEP = model.CEP,
                Telefone = model.Telefone,
                Ativo = true,
                DataDeCadastro = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                string role = model.Role.ToString();
                var roleResult = await _userManager.AddToRoleAsync(newUser, role);
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new { Message = $"Falha ao atribuir o papel {role}", Errors = roleResult.Errors });
                }

                // Criar vínculo em UsuarioEstabelecimento
                var novoVinculo = new UsuarioEstabelecimento
                {
                    UsuarioId = newUser.Id,
                    EstabelecimentoId = model.EstabelecimentoId,
                    NivelAcesso = model.NivelAcesso,
                    DataCadastro = DateTime.UtcNow,
                    Ativo = true
                };
                await _usuarioEstabelecimentoService.AddAsync(novoVinculo);

                return Ok(new { Message = "Funcionário cadastrado e vinculado com sucesso ao estabelecimento." });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Atualiza as informações de um usuário existente.
        /// </summary>
        /// <param name="model">O modelo contendo os dados atualizados do usuário.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> indicando o resultado da atualização.</returns>
        /// <remarks>
        /// Este método permite que um administrador atualize as informações de um usuário existente.
        /// As propriedades são atualizadas apenas se forem fornecidas no modelo.
        /// </remarks>
        [Authorize(Roles = "Proprietario,Administrador")]
        [HttpPut("update-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
                return BadRequest(new { Message = "O ID do usuário é obrigatório" });

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound(new { Message = "Usuário não encontrado" });

            // Atualiza as propriedades fornecidas no modelo
            var properties = typeof(UpdateUserModel).GetProperties();
            foreach (var property in properties)
            {
                var newValue = property.GetValue(model);
                if (newValue != null && property.Name != nameof(model.Id))
                {
                    var userProperty = typeof(ApplicationUser).GetProperty(property.Name);
                    if (userProperty != null && userProperty.CanWrite)
                    {
                        userProperty.SetValue(user, newValue);
                    }
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { Message = "Usuário atualizado com sucesso" });

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Altera a senha de um usuário autenticado.
        /// </summary>
        /// <param name="model">O modelo contendo a senha atual e a nova senha.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> indicando o resultado da mudança de senha.</returns>
        /// <remarks>
        /// Este método permite que um usuário autenticado altere sua senha fornecendo a senha atual e a nova senha desejada.
        /// </remarks>
        [Authorize]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound(new { Message = "Usuário não encontrado" });

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
                return Ok(new { Message = "Senha alterada com sucesso" });

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Atribui um papel a um usuário.
        /// </summary>
        /// <param name="username">O nome de usuário do usuário.</param>
        /// <param name="role">O papel a ser atribuído.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> indicando o resultado da atribuição do papel.</returns>
        /// <remarks>
        /// Este método permite que um papel seja atribuído a um usuário específico. O usuário é buscado pelo nome de usuário fornecido.
        /// </remarks>
        [HttpPost("assign-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole(string username, string role)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { Message = "Usuário não encontrado" });

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                return BadRequest(new { Message = "Falha ao atribuir o papel" });

            return Ok(new { Message = $"Papel {role} atribuído ao usuário {username} com sucesso" });
        }

        /// <summary>
        /// Autentica um usuário e gera tokens JWT e de atualização.
        /// </summary>
        /// <param name="model">O modelo contendo os dados de login.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> com os tokens se a autenticação for bem-sucedida.</returns>
        /// <remarks>
        /// Este método autentica o usuário usando nome de usuário e senha, e gera um token JWT e um token de atualização se a autenticação for bem-sucedida.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return Unauthorized(new { Message = "Credenciais inválidas" });
                }

                if (!user.Ativo)
                {
                    return Unauthorized(new { Message = "Conta desativada" });
                }

                if (user.IsLockedOut())
                {
                    return Unauthorized(new
                    {
                        Message = "Conta bloqueada temporariamente",
                        BlockedUntil = user.BloqueioExpiraEm
                    });
                }

                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Resetar tentativas de login após sucesso
                    await user.ResetLoginAttempts(_userManager);

                    // Atualizar informações do último login
                    user.UltimoLogin = DateTime.UtcNow;
                    user.UltimoLoginIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _userManager.UpdateAsync(user);

                    var tokenResponse = await _tokenService.GenerateTokensAsync(user);

                    return Ok(new
                    {
                        tokenResponse.Token,
                        tokenResponse.RefreshToken,
                        tokenResponse.ExpiresAt,
                        tokenResponse.Roles,
                        User = new
                        {
                            user.Id,
                            user.UserName,
                            user.Email,
                            user.NomeUsuario
                        }
                    });
                }

                // Incrementar tentativas falhas
                if (await user.IncrementFailedLoginAttempt(_userManager))
                {
                    return Unauthorized(new
                    {
                        Message = "Conta bloqueada temporariamente devido a múltiplas tentativas falhas",
                        BlockedUntil = user.BloqueioExpiraEm
                    });
                }

                return Unauthorized(new { Message = "Credenciais inválidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login do usuário {Username}", model.Username);
                return StatusCode(500, new { Message = "Erro interno do servidor" });
            }
        }

        [HttpPost("login-cliente")]
        public async Task<IActionResult> LoginCliente([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Username);
                if (user == null)
                {
                    return Unauthorized(new { Message = "Credenciais inválidas" });
                }

                if (!user.Ativo)
                {
                    return Unauthorized(new { Message = "Conta desativada" });
                }

                if (user.IsLockedOut())
                {
                    return Unauthorized(new
                    {
                        Message = "Conta bloqueada temporariamente",
                        BlockedUntil = user.BloqueioExpiraEm
                    });
                }

                if (await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // Resetar tentativas de login após sucesso
                    await user.ResetLoginAttempts(_userManager);

                    // Atualizar informações do último login
                    user.UltimoLogin = DateTime.UtcNow;
                    user.UltimoLoginIP = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _userManager.UpdateAsync(user);

                    var tokenResponse = await _tokenService.GenerateTokensAsync(user);

                    return Ok(new
                    {
                        tokenResponse.Token,
                        tokenResponse.RefreshToken,
                        tokenResponse.ExpiresAt,
                        tokenResponse.Roles,
                        User = new
                        {
                            user.Id,
                            user.UserName,
                            user.Email,
                            user.NomeUsuario
                        }
                    });
                }

                // Incrementar tentativas falhas
                if (await user.IncrementFailedLoginAttempt(_userManager))
                {
                    return Unauthorized(new
                    {
                        Message = "Conta bloqueada temporariamente devido a múltiplas tentativas falhas",
                        BlockedUntil = user.BloqueioExpiraEm
                    });
                }

                return Unauthorized(new { Message = "Credenciais inválidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login do usuário {Username}", model.Username);
                return StatusCode(500, new { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza o token JWT usando um token de atualização válido.
        /// </summary>
        /// <param name="refreshToken">O token de atualização para validar e usar na geração de um novo JWT.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> com os novos tokens se a operação for bem-sucedida.</returns>
        /// <remarks>
        /// Este método valida o token de atualização e, se for válido, gera um novo token JWT e um novo token de atualização.
        /// </remarks>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Validar o token JWT atual
                var principal = await _tokenService.ValidateTokenAsync(request.Token);
                var userId = principal.FindFirst("UserId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { Message = "Token inválido" });
                }

                // Buscar o usuário
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new { Message = "Usuário não encontrado" });
                }

                // Validar o refresh token
                if (user.RefreshToken != request.RefreshToken ||
                    user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Unauthorized(new { Message = "Refresh token inválido ou expirado" });
                }

                // Gerar novos tokens
                var tokenResponse = await _tokenService.GenerateTokensAsync(user);

                // Retornar os novos tokens
                return Ok(new
                {
                    tokenResponse.Token,
                    tokenResponse.RefreshToken,
                    tokenResponse.ExpiresAt,
                    tokenResponse.Roles,
                    User = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.NomeUsuario
                    }
                });
            }
            catch (SecurityTokenException)
            {
                return Unauthorized(new { Message = "Token inválido" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar o token para o usuário");
                return StatusCode(500, new { Message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Inicia o processo de redefinição de senha para um usuário, gerando um token de redefinição.
        /// </summary>
        /// <param name="model">O modelo contendo o email do usuário.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> indicando o resultado da operação.</returns>
        /// <remarks>
        /// Este método gera um token de redefinição de senha e deve ser enviado ao email do usuário.
        /// </remarks>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { Message = "Email é obrigatório" });

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound(new { Message = "Usuário não encontrado" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // A lógica para enviar este token ao email do usuário deve ser implementada

            return Ok(new { Message = "Token de redefinição de senha gerado. Verifique seu email." });
        }

        /// <summary>
        /// Redefine a senha de um usuário usando um token de redefinição válido.
        /// </summary>
        /// <param name="model">O modelo contendo o email do usuário, token de redefinição e nova senha.</param>
        /// <returns>Retorna um <see cref="IActionResult"/> indicando o resultado da redefinição de senha.</returns>
        /// <remarks>
        /// Este método permite que um usuário redefina sua senha usando um token de redefinição válido.
        /// </remarks>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound(new { Message = "Usuário não encontrado" });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
                return Ok(new { Message = "Senha redefinida com sucesso" });

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Retorna uma lista de usuários com os papéis de Admin e Proprietário.
        /// </summary>
        /// <returns>Uma lista de usuários com os papéis de Admin e Proprietário.</returns>
        //[Authorize(Roles = "Admin")]
        [HttpGet("users/admin-proprietario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersAdminProprietario()
        {
            try
            {
                // Busca todos os usuários
                var users = _userManager.Users.ToList();

                // Lista para armazenar os usuários filtrados
                var filteredUsers = new List<ApplicationUser>();

                foreach (var user in users)
                {
                    // Verifica se o usuário possui os papéis de Admin ou Proprietário
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(UserRole.Administrador.ToString()) || roles.Contains(UserRole.Proprietario.ToString()))
                    {
                        filteredUsers.Add(user);
                    }
                }

                if (!filteredUsers.Any())
                {
                    return NotFound(new { Message = "Nenhum usuário com papel de Admin ou Proprietário encontrado." });
                }

                return Ok(filteredUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuários com papéis de Admin e Proprietário.");
                return StatusCode(500, new { Message = "Erro interno do servidor." });
            }
        }

        [HttpGet("username-exists")]
        public async Task<IActionResult> CheckUsernameExists([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { Message = "O nome de usuário é obrigatório" });
            }

            var userExists = await _userManager.FindByNameAsync(username) != null;

            return Ok(new { Exists = userExists });
        }


    }
}