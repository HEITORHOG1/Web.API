using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using Microsoft.JSInterop;
using System.Text.Json;

namespace MarketplaceHybrid.Shared.Services
{
    public class CarrinhoService : ICarrinhoService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "carrinho";
        private bool _isInitialized = false;

        public event Func<Task> CarrinhoAtualizado;

        public CarrinhoService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<List<CarrinhoItem>> ObterCarrinhoAsync()
        {
            if (!_isInitialized)
            {
                Console.WriteLine("JSInterop ainda não está disponível durante a pré-renderização.");
                return new List<CarrinhoItem>();
            }

            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", StorageKey);
                return string.IsNullOrEmpty(json) ? new List<CarrinhoItem>() : JsonSerializer.Deserialize<List<CarrinhoItem>>(json) ?? new List<CarrinhoItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter carrinho: {ex.Message}");
                return new List<CarrinhoItem>();
            }
        }

        public async Task AdicionarAoCarrinhoAsync(CarrinhoItem item)
        {
            var carrinho = await ObterCarrinhoAsync();
            var existente = carrinho.FirstOrDefault(x => x.ProdutoId == item.ProdutoId);

            if (existente != null)
            {
                existente.Quantidade += item.Quantidade;
            }
            else
            {
                carrinho.Add(item);
            }

            await SalvarCarrinhoAsync(carrinho);
            if (CarrinhoAtualizado != null) await CarrinhoAtualizado.Invoke();
        }

        public async Task RemoverDoCarrinhoAsync(int produtoId)
        {
            var carrinho = await ObterCarrinhoAsync();
            carrinho.RemoveAll(x => x.ProdutoId == produtoId);
            await SalvarCarrinhoAsync(carrinho);
            if (CarrinhoAtualizado != null) await CarrinhoAtualizado.Invoke();
        }

        public async Task SalvarCarrinhoAsync(List<CarrinhoItem> carrinho)
        {
            if (!_isInitialized)
            {
                Console.WriteLine("JSInterop ainda não está disponível durante a pré-renderização.");
                return;
            }

            try
            {
                var json = JsonSerializer.Serialize(carrinho);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", StorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar carrinho: {ex.Message}");
            }
        }

        public async Task LimparCarrinhoAsync()
        {
            if (!_isInitialized)
            {
                Console.WriteLine("JSInterop ainda não está disponível durante a pré-renderização.");
                return;
            }

            try
            {
                await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", StorageKey);
                if (CarrinhoAtualizado != null) await CarrinhoAtualizado.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar carrinho: {ex.Message}");
            }
        }

        public void MarcarComoInicializado()
        {
            _isInitialized = true; // Marcar que o JavaScript está disponível
        }

        public async Task AtualizarCarrinhoAsync(AtualizarQuantidadeDto itemDto)
        {
            var carrinho = await ObterCarrinhoAsync();
            var item = carrinho.FirstOrDefault(x => x.ProdutoId == itemDto.ProdutoId && x.EstabelecimentoId == itemDto.EstabelecimentoId);

            if (item != null)
            {
                item.Quantidade += itemDto.Quantidade;
                await SalvarCarrinhoAsync(carrinho);
                await CarrinhoAtualizado?.Invoke();
            }
        }
    }
}