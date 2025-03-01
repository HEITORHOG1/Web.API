using AutoMapper;
using FluentValidation;
using System.Globalization;
using Web.API.Services;
using Web.Application.Interfaces;
using Web.Application.Validators;
using Web.Domain.DTOs.Categorias;
using Web.Domain.DTOs.Produtos;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IValidator<Produto> _produtoValidator;
        private readonly IEstabelecimentoService _estabelecimentoService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IImagemProdutoService _imagemProdutoService;
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoqueService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMapper _mapper; // Campo IMapper

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IValidator<Produto> produtoValidator,
            IEstabelecimentoService estabelecimentoService,
            IImageUploadService imageUploadService,
            IImagemProdutoService imagemProdutoService,
            IMovimentacaoEstoqueService movimentacaoEstoqueService,
            ICategoriaService categoriaService,
             IMapper mapper) //Adicionado mapper
        {
            _produtoRepository = produtoRepository;
            _produtoValidator = produtoValidator;
            _estabelecimentoService = estabelecimentoService;
            _imageUploadService = imageUploadService;
            _imagemProdutoService = imagemProdutoService;
            _movimentacaoEstoqueService = movimentacaoEstoqueService;
            _categoriaService = categoriaService;
            _mapper = mapper; // Atribuição do mapper
        }

        public async Task<Produto> GetProdutoByIdAsync(int estabelecimentoId, int id)
        {
            return await _produtoRepository.GetProdutoByIdAsync(estabelecimentoId, id);
        }

        public async Task AddProdutoAsync(int estabelecimentoId, Produto produto)
        {
            // Validação antes de adicionar o produto
            var validationResult = await _produtoValidator.ValidateAsync(produto);
            if (!validationResult.IsValid)
            {
                // Você pode lançar uma exceção personalizada ou retornar um resultado indicando falha na validação
                throw new ValidationException(validationResult.Errors);
            }

            await _produtoRepository.AddProdutoAsync(estabelecimentoId, produto);
        }

        public async Task UpdateProdutoAsync(int estabelecimentoId, Produto produto)
        {
            // Validação antes de atualizar o produto
            var validationResult = await _produtoValidator.ValidateAsync(produto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _produtoRepository.UpdateProdutoAsync(estabelecimentoId, produto);
        }

        public async Task RemoveProdutoAsync(int estabelecimentoId, int id)
        {
            await _produtoRepository.RemoveProdutoAsync(estabelecimentoId, id);
        }

        public async Task<List<ProdutoDto>> GetProdutosByEstabelecimentoIdAsync(int estabelecimentoId)
        {
            return await _produtoRepository.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task VenderProdutoAsync(int estabelecimentoId, int produtoId, int quantidade)
        {
            var produto = await _produtoRepository.GetProdutoByIdAsync(estabelecimentoId, produtoId);

            if (produto == null)
                throw new Exception("Produto não encontrado.");

            if (produto.QuantidadeEmEstoque < quantidade)
                throw new Exception("Estoque insuficiente.");

            // Atualiza a quantidade em estoque
            produto.QuantidadeEmEstoque -= quantidade;
            await _produtoRepository.UpdateProdutoAsync(estabelecimentoId, produto);

            // Registra a movimentação de saída
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                Quantidade = quantidade,
                Tipo = TipoMovimentacao.Saida,
                Observacao = "Venda de produto"
            };
            await _movimentacaoEstoqueService.AddMovimentacaoAsync(movimentacao);
        }

        public async Task EntradaProdutoAsync(int estabelecimentoId, int produtoId, int quantidade, string observacao = null)
        {
            var produto = await _produtoRepository.GetProdutoByIdAsync(estabelecimentoId, produtoId);

            if (produto == null)
                throw new Exception("Produto não encontrado.");

            if (quantidade <= 0)
                throw new Exception("A quantidade deve ser maior que zero.");

            // Atualiza a quantidade em estoque
            produto.QuantidadeEmEstoque += quantidade;
            await _produtoRepository.UpdateProdutoAsync(estabelecimentoId, produto);

            // Registra a movimentação de entrada
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                Quantidade = quantidade,
                Tipo = TipoMovimentacao.Entrada,
                Observacao = observacao ?? "Entrada de produto"
            };
            await _movimentacaoEstoqueService.AddMovimentacaoAsync(movimentacao);
        }

        public async Task<List<Produto>> GetProdutosByCategoriaIdAsync(int estabelecimentoId, int categoriaId)
        {
            return await _produtoRepository.GetProdutosByCategoriaIdAsync(estabelecimentoId, categoriaId);
        }

        public async Task<CategoriaDto> GetCategoriaByIdAsync(int estabelecimentoId, int categoriaId)
        {
            return _mapper.Map<CategoriaDto>(await _categoriaService.GetCategoriaByIdAsync(estabelecimentoId, categoriaId));
        }

        public async Task<bool> ExistsAsync(int estabelecimentoId, string nome)
        {
            return await _produtoRepository.ExistsAsync(estabelecimentoId, nome);
        }

        public async Task<List<Produto>> ProdutoCadastradoNoEstabelecimentoAsync(int estabelecimentoId, string nomeProduto)
        {
            return await _produtoRepository.ProdutoCadastradoNoEstabelecimentoAsync(estabelecimentoId, nomeProduto);
        }

        public async Task<ProdutoDto> CreateProdutoAsync(int estabelecimentoId, string userId, ProdutoCreateDto produtoDto)
        {
            // Validar estabelecimento e permissões
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para adicionar produtos neste estabelecimento.");

            // Verificar existência do produto
            var exists = await ExistsAsync(estabelecimentoId, produtoDto.Nome);
            if (exists)
                throw new InvalidOperationException("Já existe um produto com este nome.");

            // Validar categoria
            var categoria = await GetCategoriaByIdAsync(estabelecimentoId, produtoDto.CategoriaId);
            if (categoria == null || categoria.EstabelecimentoId != estabelecimentoId)
                throw new InvalidOperationException("A categoria especificada não existe ou não pertence a este estabelecimento.");

            // Upload da imagem
            var imagePath = await _imageUploadService.UploadImageAsync(produtoDto.Imagem);

            // Criar produto
            var produto = new Produto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                Preco = PriceFormatter.FormatPrice(produtoDto.Preco),
                Imagem = imagePath,
                Disponivel = produtoDto.Disponivel,
                CategoriaId = produtoDto.CategoriaId,
                EstabelecimentoId = estabelecimentoId,
                DataCadastro = DateTime.UtcNow,
                QuantidadeEmEstoque = 0,
                CodigoDeBarras = produtoDto.CodigoDeBarras
            };

            // Validar e adicionar produto
            var validationResult = await _produtoValidator.ValidateAsync(produto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _produtoRepository.AddProdutoAsync(estabelecimentoId, produto);

            // Adicionar imagem do produto
            await _imagemProdutoService.AddImagemProdutoAsync(new ImagemProduto
            {
                ProdutoId = produto.Id,
                Url = imagePath,
                EstabelecimentoId = estabelecimentoId
            });

            // Retornar DTO
            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Imagem = produto.Imagem,
                Disponivel = produto.Disponivel,
                CategoriaId = produto.CategoriaId,
                DataCadastro = produto.DataCadastro,
                CodigoDeBarras = produto.CodigoDeBarras
            };
        }

        public async Task<List<ProdutoDto>> GetProdutosAsync(int estabelecimentoId, string userId)
        {
            var vinculo = await _estabelecimentoService.GetVinculoAsync(userId, estabelecimentoId);
            if (vinculo == null)
                throw new UnauthorizedAccessException("Você não tem permissão para visualizar produtos deste estabelecimento.");

            return await _produtoRepository.GetAllByEstabelecimentoIdAsync(estabelecimentoId);
        }

        public async Task<ProdutoDto> GetProdutoByIdDtoAsync(int estabelecimentoId, int id, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado");

            var produto = await _produtoRepository.GetProdutoByIdAsync(estabelecimentoId, id);
            if (produto == null)
                throw new KeyNotFoundException("Produto não encontrado");

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                Imagem = produto.Imagem,
                Disponivel = produto.Disponivel,
                CategoriaId = produto.CategoriaId,
                DataCadastro = produto.DataCadastro,
                QuantidadeEmEstoque = produto.QuantidadeEmEstoque,
                CodigoDeBarras = produto.CodigoDeBarras
            };
        }

        public async Task UpdateProdutoAsync(int estabelecimentoId, int id, string userId, ProdutoUpdateDto produtoDto)
        {
            var estabelecimento = await _estabelecimentoService.GetByIdAsync(estabelecimentoId);
            if (estabelecimento == null || estabelecimento.UsuarioId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar produtos neste estabelecimento.");

            var existingProduto = await _produtoRepository.GetProdutoByIdAsync(estabelecimentoId, id);
            if (existingProduto == null)
                throw new KeyNotFoundException("Produto não encontrado");

            var categoria = await GetCategoriaByIdAsync(estabelecimentoId, produtoDto.CategoriaId);
            if (categoria == null || categoria.EstabelecimentoId != estabelecimentoId)
                throw new InvalidOperationException("A categoria especificada não existe ou não pertence a este estabelecimento.");

            if (produtoDto.Imagem != null)
            {
                var imagePath = await _imageUploadService.UploadImageAsync(produtoDto.Imagem);
                existingProduto.Imagem = imagePath;

                await _imagemProdutoService.AddImagemProdutoAsync(new ImagemProduto
                {
                    ProdutoId = existingProduto.Id,
                    Url = imagePath,
                    EstabelecimentoId = estabelecimentoId
                });
            }

            if (!decimal.TryParse(produtoDto.Preco.ToString(CultureInfo.InvariantCulture),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal precoInput))
            {
                throw new InvalidOperationException("Formato de preço inválido");
            }

            existingProduto.Preco = PriceFormatter.FormatPrice(precoInput);
            existingProduto.Nome = produtoDto.Nome;
            existingProduto.Descricao = produtoDto.Descricao;
            existingProduto.Disponivel = produtoDto.Disponivel;
            existingProduto.CategoriaId = produtoDto.CategoriaId;
            existingProduto.CodigoDeBarras = produtoDto.CodigoDeBarras;

            await _produtoRepository.UpdateProdutoAsync(estabelecimentoId, existingProduto);
        }
    }
}