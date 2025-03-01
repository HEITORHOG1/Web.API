using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Options;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.Entities;

namespace Web.API.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IOptions<MercadoPagoSettings> _mpSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MercadoPagoService(IOptions<MercadoPagoSettings> mpSettings, IHttpContextAccessor httpContextAccessor)
        {
            _mpSettings = mpSettings;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> CriarPreferenciaAsync(Pedido pedido)
        {
            // 1) Configurar credenciais
            MercadoPagoConfig.AccessToken = _mpSettings.Value.AccessToken;

            // 2) Montar itens
            var items = pedido.Itens.Select(item => new PreferenceItemRequest
            {
                Title = "Produto " + item.ProdutoId,
                Quantity = item.Quantidade,
                CurrencyId = "BRL",
                UnitPrice = item.PrecoUnitario ?? 0m // Assegura que não é null
            }).ToList();

            // 3) Configurar URLs de retorno
            string baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var backUrls = new PreferenceBackUrlsRequest
            {
                Success = $"{baseUrl}/api/pagamento/sucesso",
                Failure = $"{baseUrl}/api/pagamento/falha",
                Pending = $"{baseUrl}/api/pagamento/pendente"
            };

            // 4) Criar objeto Preference
            var request = new PreferenceRequest
            {
                Items = items,
                ExternalReference = pedido.ExternalReference, // Usando ExternalReference
                BackUrls = backUrls,
                AutoReturn = "approved", // Retorna automaticamente quando aprovado
                NotificationUrl = $"{baseUrl}/api/pagamento/notificacoes" // URL do webhook
            };

            // 5) Envia para Mercado Pago
            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            // 6) Retorna a URL de pagamento (init_point ou sandbox_init_point)
            return preference.InitPoint;
        }

        public async Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId)
        {
            MercadoPagoConfig.AccessToken = _mpSettings.Value.AccessToken;

            var client = new PaymentClient();
            long paymentIdLong;

            if (!long.TryParse(paymentId, out paymentIdLong))
            {
                throw new ArgumentException("paymentId deve ser um número válido.");
            }

            Payment payment = await client.GetAsync(paymentIdLong);
            return payment;
        }
    }
}