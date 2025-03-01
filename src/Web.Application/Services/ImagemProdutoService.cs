using FluentValidation;
using Web.Application.Interfaces;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class ImagemProdutoService : IImagemProdutoService
    {
        private readonly IImagemProdutoRepository _imagemProdutoRepository;
        private readonly IValidator<ImagemProduto> _validator;

        public ImagemProdutoService(
            IImagemProdutoRepository imagemProdutoRepository,
            IValidator<ImagemProduto> validator)
        {
            _imagemProdutoRepository = imagemProdutoRepository;
            _validator = validator;
        }

        public async Task AddImagemProdutoAsync(ImagemProduto imagemProduto)
        {
            var validationResult = await _validator.ValidateAsync(imagemProduto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _imagemProdutoRepository.AddAsync(imagemProduto);
        }

        public async Task UpdateImagemProdutoAsync(ImagemProduto imagemProduto)
        {
            var validationResult = await _validator.ValidateAsync(imagemProduto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _imagemProdutoRepository.UpdateAsync(imagemProduto);
        }

        public async Task DeleteImagemProdutoAsync(int id)
        {
            await _imagemProdutoRepository.DeleteAsync(id);
        }

        public async Task<ImagemProduto> GetImagemProdutoByIdAsync(int id, int produtoId, int estabelecimentoId)
        {
            var imagem = await _imagemProdutoRepository.GetByIdAsync(id);

            if (imagem == null || imagem.ProdutoId != produtoId || imagem.EstabelecimentoId != estabelecimentoId)
            {
                return null;
            }

            return imagem;
        }
    }
}