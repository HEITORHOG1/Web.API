using IdentityModel.OidcClient;
using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.PaymentMethod;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System.Text.Json;
using Web.API.Services;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.Entities;
using Web.Domain.Interfaces;

namespace Web.Application.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IOptions<MercadoPagoSettings> _mpSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MercadoPagoService> _logger;
        private readonly IAsyncPolicy<object> _retryPolicy;

        public MercadoPagoService(
            IOptions<MercadoPagoSettings> mpSettings,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MercadoPagoService> logger)
        {
            _mpSettings = mpSettings;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // Política de retry para chamadas à API
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Tentativa {retryCount} falhou: {exception.Message}. Tentando novamente em {timeSpan.TotalSeconds} segundos");
                    });

            // Configurar credenciais
            MercadoPagoConfig.AccessToken = _mpSettings.Value.AccessToken;
        }

        public async Task<string> CriarPreferenciaAsync(Pedido pedido)
        {
            try
            {
                // 1) Montar itens
                var items = pedido.Itens.Select(item => new PreferenceItemRequest
                {
                    Title = $"Produto #{item.ProdutoId}",
                    Quantity = item.Quantidade,
                    CurrencyId = "BRL",
                    UnitPrice = item.PrecoUnitario ?? 0m
                }).ToList();

                // 2) Configurar URLs de retorno
                string baseUrl = GetBaseUrl();
                var backUrls = new PreferenceBackUrlsRequest
                {
                    Success = $"{baseUrl}/api/pagamento/sucesso?reference={pedido.ExternalReference}",
                    Failure = $"{baseUrl}/api/pagamento/falha?reference={pedido.ExternalReference}",
                    Pending = $"{baseUrl}/api/pagamento/pendente?reference={pedido.ExternalReference}"
                };

                // 3) Adicionar informações do cliente
                var payer = new PreferencePayerRequest
                {
                    Email = pedido.UsuarioId, // Idealmente seria o email do usuário
                    Name = pedido.UsuarioId,  // Idealmente seria o nome do usuário
                    Identification = new IdentificationRequest
                    {
                        Type = "CPF",
                        Number = "00000000000" // Idealmente seria o CPF do usuário
                    }
                };

                // 4) Criar objeto Preference
                var request = new PreferenceRequest
                {
                    Items = items,
                    ExternalReference = pedido.ExternalReference,
                    BackUrls = backUrls,
                    AutoReturn = "approved",
                    NotificationUrl = $"{baseUrl}/api/pagamento/notificacoes",
                    Payer = payer,
                    StatementDescriptor = $"Pedido #{pedido.Id}",
                    ExpirationDateFrom = DateTime.UtcNow,
                    ExpirationDateTo = DateTime.UtcNow.AddDays(2)
                };

                // 5) Envia para Mercado Pago com retry policy
                var result = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PreferenceClient();
                    return await client.CreateAsync(request);
                });

                // 6) Retorna a URL de pagamento
                var preference = result as Preference;
                return _mpSettings.Value.UseSandbox
                    ? preference.SandboxInitPoint
                    : preference.InitPoint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar preferência de pagamento para o pedido {PedidoId}", pedido.Id);
                throw new Exception($"Erro ao processar pagamento: {ex.Message}", ex);
            }
        }

        public async Task<Payment> ObterPagamentoDoMercadoPagoAsync(string paymentId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    if (!long.TryParse(paymentId, out long paymentIdLong))
                    {
                        throw new ArgumentException("PaymentId deve ser um número válido");
                    }

                    var client = new PaymentClient();
                    return await client.GetAsync(paymentIdLong);
                }) as Payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao consultar pagamento: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelarPagamentoAsync(string paymentId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    if (!long.TryParse(paymentId, out long paymentIdLong))
                    {
                        throw new ArgumentException("PaymentId deve ser um número válido");
                    }

                    var client = new PaymentClient();
                    var request = new PaymentCancelRequest();
                    var payment = await client.CancelAsync(paymentIdLong, request);

                    return payment.Status == "cancelled";
                }) as bool;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao cancelar pagamento: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReembolsarPagamentoAsync(string paymentId, decimal? valor = null)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    if (!long.TryParse(paymentId, out long paymentIdLong))
                    {
                        throw new ArgumentException("PaymentId deve ser um número válido");
                    }

                    var client = new PaymentClient();
                    var request = new PaymentRefundRequest();

                    if (valor.HasValue)
                    {
                        request.Amount = valor.Value;
                        var result = await client.RefundPartialAsync(paymentIdLong, request);
                        return result.Status == "approved";
                    }
                    else
                    {
                        var result = await client.RefundAsync(paymentIdLong);
                        return result.Status == "approved";
                    }
                }) as bool;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reembolsar pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao processar reembolso: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoAsync()
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PaymentMethodClient();
                    var paymentMethods = await client.ListAsync();

                    return paymentMethods.Select(pm => new PaymentMethodDto
                    {
                        Id = pm.Id,
                        Name = pm.Name,
                        Type = pm.PaymentTypeId,
                        Description = pm.Name,
                        PaymentTypeId = pm.PaymentTypeId,
                        IsDefault = false,
                        ThumbnailUrl = pm.SecureThumbnail
                    }).ToList();
                }) as IEnumerable<PaymentMethodDto>;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter métodos de pagamento");
                throw new Exception($"Erro ao consultar métodos de pagamento: {ex.Message}", ex);
            }
        }

        public async Task<Preference> ObterPreferenciaAsync(string preferenceId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PreferenceClient();
                    return await client.GetAsync(preferenceId);
                }) as Preference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter preferência {PreferenceId}", preferenceId);
                throw new Exception($"Erro ao consultar preferência: {ex.Message}", ex);
            }
        }

        public async Task<string> AtualizarPreferenciaAsync(string preferenceId, Pedido pedido)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PreferenceClient();

                    var items = pedido.Itens.Select(item => new PreferenceItemRequest
                    {
                        Title = $"Produto #{item.ProdutoId}",
                        Quantity = item.Quantidade,
                        CurrencyId = "BRL",
                        UnitPrice = item.PrecoUnitario ?? 0m
                    }).ToList();

                    var request = new PreferenceRequest
                    {
                        Items = items
                    };

                    var preference = await client.UpdateAsync(preferenceId, request);
                    return _mpSettings.Value.UseSandbox
                        ? preference.SandboxInitPoint
                        : preference.InitPoint;
                }) as string;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar preferência {PreferenceId}", preferenceId);
                throw new Exception($"Erro ao atualizar preferência: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerificarStatusPagamentoAsync(string paymentId)
        {
            try
            {
                var payment = await ObterPagamentoDoMercadoPagoAsync(paymentId);
                return payment.Status == "approved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status do pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao verificar status do pagamento: {ex.Message}", ex);
            }
        }

        public async Task<string> RegistrarWebhookAsync(string url)
        {
            // Implementação simulada - O Mercado Pago não oferece uma API específica para isso
            // Normalmente é feito manualmente no painel ou por meio de uma API não documentada
            _logger.LogInformation($"URL de webhook registrada: {url}");
            return await Task.FromResult("OK");
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}