using FluentValidation;
using MercadoPago.Resource.User;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.DTOs;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;
using Web.Domain.Enums;
using Web.Domain.Geo;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class EstabelecimentoService : IEstabelecimentoService
    {
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IUsuarioEstabelecimentoRepository _usuarioEstabelecimentoRepository;
        private readonly EstabelecimentoValidator _estabelecimentoValidator;
        private readonly IHorarioFuncionamentoService _horarioFuncionamentoService;
        private readonly IGeocodingService _geocodingService;
        private readonly IImageUploadService _imageUploadService;
        public EstabelecimentoService(IEstabelecimentoRepository estabelecimentoRepository, IUsuarioEstabelecimentoRepository usuarioEstabelecimentoRepository, EstabelecimentoValidator estabelecimentoValidator, IHorarioFuncionamentoService horarioFuncionamentoService, IGeocodingService geocodingService, IImageUploadService imageUploadService)
        {
            _estabelecimentoRepository = estabelecimentoRepository;
            _usuarioEstabelecimentoRepository = usuarioEstabelecimentoRepository;
            _estabelecimentoValidator = estabelecimentoValidator;
            _horarioFuncionamentoService = horarioFuncionamentoService;
            _geocodingService = geocodingService;
            _imageUploadService = imageUploadService;
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllByUserIdAsync(string userId)
        {
            return await _estabelecimentoRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Estabelecimento> GetByIdAsync(int id)
        {
            return await _estabelecimentoRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Estabelecimento estabelecimento)
        {
            var validationResult = _estabelecimentoValidator.Validate(estabelecimento);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await _estabelecimentoRepository.AddAsync(estabelecimento);
        }

        public async Task UpdateAsync(UpdateEstabelecimento estabelecimento)
        {
            string? imagePath = null;

            // Verificar se a imagem foi alterada e se não está null
            if (estabelecimento.UrlImagem != null)
            {
                imagePath = await _imageUploadService.UploadImageAsync(estabelecimento.UrlImagem);
            }

            Estabelecimento _estabelecimento = new Estabelecimento
            {
                Id = estabelecimento.Id,
                UsuarioId = estabelecimento.UsuarioId,
                RazaoSocial = estabelecimento.RazaoSocial,
                NomeFantasia = estabelecimento.NomeFantasia,
                CNPJ = estabelecimento.CNPJ,
                Telefone = estabelecimento.Telefone,
                Endereco = estabelecimento.Endereco,
                Status = estabelecimento.Status,
                DataCadastro = estabelecimento.DataCadastro,
                Cep = estabelecimento.Cep,
                Numero = estabelecimento.Numero,
                TaxaEntregaFixa = estabelecimento.TaxaEntregaFixa,
                UrlImagem = imagePath,
                Descricao = estabelecimento.Descricao
            };

            // Obter coordenadas via API de geocodificação
            if (!string.IsNullOrWhiteSpace(_estabelecimento.Endereco))
            {
                try
                {
                    var enderecoCompleto = $"{_estabelecimento.Endereco}, {_estabelecimento.Cep}";
                    var (latitude, longitude) = await _geocodingService.GetCoordinatesAsync(enderecoCompleto);

                    // Preencher as coordenadas no estabelecimento
                    _estabelecimento.Latitude = latitude;
                    _estabelecimento.Longitude = longitude;
                }
                catch (Exception ex)
                {
                    // Log do erro de geocodificação, mas o fluxo continua
                    throw new Exception($"Erro ao obter coordenadas do endereço: {ex.Message}");
                }
            }

            try
            {
                await _estabelecimentoRepository.UpdateAsyncNess(_estabelecimento);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _estabelecimentoRepository.DeleteAsync(id);
        }

        public async Task<bool> IsProprietarioAsync(string userId, int estabelecimentoId)
        {
            return await _usuarioEstabelecimentoRepository.IsProprietarioAsync(userId, estabelecimentoId);
        }

        public async Task<UsuarioEstabelecimento> GetVinculoAsync(string userId, int estabelecimentoId)
        {
            return await _usuarioEstabelecimentoRepository.GetVinculoAsync(userId, estabelecimentoId);
        }

        public async Task<Estabelecimento> AddWithUserAsync(CreateEstabelecimentoDto estabelecimentoDto, string userId)
        {
            var estabelecimento = new Estabelecimento
            {
                UsuarioId = userId,
                RazaoSocial = estabelecimentoDto.RazaoSocial,
                NomeFantasia = estabelecimentoDto.NomeFantasia,
                CNPJ = estabelecimentoDto.CNPJ,
                Telefone = estabelecimentoDto.Telefone,
                Endereco = estabelecimentoDto.Endereco,
                Status = estabelecimentoDto.Status,
                Cep = estabelecimentoDto.Cep,
                Numero = estabelecimentoDto.Numero,
                DataCadastro = DateTime.UtcNow,
                TaxaEntregaFixa = estabelecimentoDto.TaxaEntregaFixa,
                UrlImagem = null,
                Descricao = estabelecimentoDto.Descricao
            };

            // Obter coordenadas via API de geocodificação
            if (!string.IsNullOrWhiteSpace(estabelecimento.Endereco))
            {
                try
                {
                    var enderecoCompleto = $"{estabelecimento.Endereco}, {estabelecimento.Cep}";
                    var (latitude, longitude) = await _geocodingService.GetCoordinatesAsync(enderecoCompleto);

                    // Preencher as coordenadas no estabelecimento
                    estabelecimento.Latitude = latitude;
                    estabelecimento.Longitude = longitude;
                }
                catch (Exception ex)
                {
                    // Log do erro de geocodificação, mas o fluxo continua
                    throw new Exception($"Erro ao obter coordenadas do endereço: {ex.Message}");
                }
            }

            // Adicionar o estabelecimento ao banco
            await _estabelecimentoRepository.AddAsync(estabelecimento);

            // Criar o vínculo do usuário com o estabelecimento
            var usuarioEstabelecimento = new UsuarioEstabelecimento
            {
                UsuarioId = userId,
                EstabelecimentoId = estabelecimento.Id,
                NivelAcesso = NivelAcesso.Proprietario,
                DataCadastro = DateTime.UtcNow,
                Ativo = true
            };

            await _usuarioEstabelecimentoRepository.AddAsync(usuarioEstabelecimento);

            return estabelecimento;
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllByProprietarioIdAsync(string proprietarioId)
        {
            return await _estabelecimentoRepository.GetAllByProprietarioIdAsync(proprietarioId);
        }

        public async Task<IEnumerable<UsuarioComRoleENivelAcessoDto>> GetAllUsersByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _estabelecimentoRepository.GetAllUsersByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task<IEnumerable<Estabelecimento>> GetProximosAsync(double latitude, double longitude, double raioKm)
        {
            return await _estabelecimentoRepository.GetProximosAsync(latitude, longitude, raioKm);
        }

        public async Task<bool> EstaDentroDaAreaEntregaAsync(int estabelecimentoId, double latitude, double longitude)
        {
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(estabelecimentoId);

            if (estabelecimento == null || estabelecimento.RaioEntregaKm == null || estabelecimento.RaioEntregaKm <= 0)
                return false; // Retorna falso se o estabelecimento não tiver um raio de entrega configurado

            var distancia = GeoHelper.CalculateDistance(estabelecimento.Latitude, estabelecimento.Longitude, latitude, longitude);
            return distancia <= estabelecimento.RaioEntregaKm;
        }

        public async Task<bool> EstaAbertoAsync(int estabelecimentoId, DayOfWeek diaSemana, TimeSpan horaAtual)
        {
            return await _horarioFuncionamentoService.EstaAbertoAsync(estabelecimentoId, diaSemana, horaAtual);
        }

        public async Task<IEnumerable<Estabelecimento>> GetAllActiveAsync()
        {
            return await _estabelecimentoRepository.GetAllActiveAsync();
        }
    }
}