using AutoMapper;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.DTOs.Categorias;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly CategoriaValidator _validationRules;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IMapper _mapper;

        public CategoriaService(ICategoriaRepository categoriaRepository, CategoriaValidator validationRules, IEstabelecimentoService estabelecimentoService, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _validationRules = validationRules;
            _estabelecimentoService = estabelecimentoService;
            _mapper = mapper;
        }

        public async Task<CategoriaDto> CreateCategoriaAsync(int estabelecimentoId, string userId, CategoriaCreateDto categoriaDto)
        {
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para adicionar categorias neste estabelecimento.");

            var exists = await ExistsAsync(estabelecimentoId, categoriaDto.Nome);
            if (exists)
                throw new InvalidOperationException("Já existe uma categoria com este nome.");

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            //categoria.EstabelecimentoId = estabelecimentoId; //Removido
            categoria = new Categoria(categoria.Nome, categoria.Descricao, estabelecimentoId); //Adicionado
            await _categoriaRepository.AddAsync(categoria);
            return _mapper.Map<CategoriaDto>(categoria);
        }

        public async Task UpdateCategoriaAsync(int estabelecimentoId, int id, string userId, CategoriaUpdateDto categoriaDto)
        {
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar categorias neste estabelecimento.");

            var existingCategoria = await _categoriaRepository.GetByIdAsync(id, estabelecimentoId);
            if (existingCategoria == null)
                throw new KeyNotFoundException("Categoria não encontrada");

            _mapper.Map(categoriaDto, existingCategoria);
            await _categoriaRepository.UpdateAsync(existingCategoria);
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            var categorias = await _categoriaRepository.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
            return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        }

        public async Task<CategoriaDto> GetCategoriaByIdAsync(int estabelecimentoId, int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id, estabelecimentoId);
            if (categoria == null)
                throw new KeyNotFoundException("Categoria não encontrada");
            return _mapper.Map<CategoriaDto>(categoria);
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        }

        public async Task DeleteCategoriaAsync(int estabelecimentoId, int id, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para remover categorias neste estabelecimento.");

            var existingCategoria = await _categoriaRepository.GetByIdAsync(id, estabelecimentoId);
            if (existingCategoria == null)
                throw new KeyNotFoundException("Categoria não encontrada");

            await _categoriaRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _categoriaRepository.ExistsAsync(estabelecimentoId, nome);
        }

        public async Task<IEnumerable<CategoriaDto>> GetCategoriasByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            var categorias = await _categoriaRepository.GetCategoriasByEstabelecimentoIdAsync(estabelecimentoId);
            return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        }
    }
}