using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Web.Application.Interfaces;
using Web.Domain.DTOs.Endereco;

namespace Web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Cliente")]
    public class EnderecoClienteController : ControllerBase
    {
        private readonly IEnderecoClienteService _enderecoClienteService;
        private readonly IMapper _mapper;
        private readonly ILogger<EnderecoClienteController> _logger;

        public EnderecoClienteController(IEnderecoClienteService enderecoClienteService, IMapper mapper, ILogger<EnderecoClienteController> logger)
        {
            _enderecoClienteService = enderecoClienteService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Requisição para listar todos os endereços de um cliente.");

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Usuário não autenticado.");
                return Unauthorized("Usuário não autenticado.");
            }

            var enderecos = await _enderecoClienteService.GetAllByUsuarioIdAsync(userId);
            _logger.LogInformation($"Endereços encontrados para o usuário {userId}: {enderecos.Count()}");

            return Ok(_mapper.Map<IEnumerable<EnderecoClienteDto>>(enderecos));
        }

        [HttpGet("principal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPrincipal()
        {
            _logger.LogInformation("Requisição para obter o endereço principal de um cliente.");

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Usuário não autenticado.");
                return Unauthorized("Usuário não autenticado.");
            }

            var endereco = await _enderecoClienteService.GetPrincipalByUsuarioIdAsync(userId);
            if (endereco == null)
            {
                _logger.LogWarning($"Endereço principal não encontrado para o usuário {userId}.");
                return NotFound("Nenhum endereço principal encontrado.");
            }
            _logger.LogInformation($"Endereço principal encontrado para o usuário {userId}.");
            return Ok(_mapper.Map<EnderecoClienteDto>(endereco));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] EnderecoClienteDto enderecoDto)
        {
            _logger.LogInformation($"Requisição para criar um novo endereço: {enderecoDto}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Dados inválidos para criação do endereço: {ModelState}");
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Usuário não autenticado.");
                return Unauthorized("Usuário não autenticado.");
            }
            var endereco = await _enderecoClienteService.AddEnderecoAsync(userId, enderecoDto);
            _logger.LogInformation($"Endereço criado com sucesso com ID {endereco.Id}");
            return CreatedAtAction(nameof(GetById), new { id = endereco.Id }, _mapper.Map<EnderecoClienteDto>(endereco));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation($"Requisição para obter o endereço com ID {id}.");
            var endereco = await _enderecoClienteService.GetEnderecoByIdAsync(id);
            if (endereco == null)
            {
                _logger.LogWarning($"Endereço com ID {id} não encontrado.");
                return NotFound("Endereço não encontrado.");
            }
            _logger.LogInformation($"Endereço com ID {id} encontrado.");
            return Ok(_mapper.Map<EnderecoClienteDto>(endereco));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] EnderecoClienteDto enderecoDto)
        {
            _logger.LogInformation($"Requisição para atualizar o endereço com ID {id}.");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"Dados inválidos para atualização do endereço: {ModelState}");
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Usuário não autenticado.");
                return Unauthorized("Usuário não autenticado.");
            }

            try
            {
                await _enderecoClienteService.UpdateEnderecoAsync(id, userId, enderecoDto);
                _logger.LogInformation($"Endereço com ID {id} atualizado com sucesso.");
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Endereço com ID {id} não encontrado.");
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Requisição para remover o endereço com ID {id}.");
            var userId = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Usuário não autenticado.");
                return Unauthorized("Usuário não autenticado.");
            }

            var endereco = await _enderecoClienteService.GetEnderecoByIdAsync(id);
            if (endereco == null)
            {
                _logger.LogWarning($"Endereço com ID {id} não encontrado.");
                return NotFound("Endereço não encontrado.");
            }

            await _enderecoClienteService.DeleteEnderecoAsync(id);
            _logger.LogInformation($"Endereço com ID {id} removido com sucesso.");

            return NoContent();
        }
    }
}