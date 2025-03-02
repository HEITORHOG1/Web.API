using MercadoPago.Client;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Client.PaymentMethod;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Options;
using Polly;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Web.Domain.DTOs.MercadoPago;
using Web.Domain.Entities;
using PollyPolicy = Polly.Policy;

namespace Web.API.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IOptions<MercadoPagoSettings> _mpSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MercadoPagoService> _logger;
        private readonly IAsyncPolicy _retryPolicy;

        public MercadoPagoService(
            IOptions<MercadoPagoSettings> mpSettings,
            IHttpContextAccessor httpContextAccessor,
            ILogger<MercadoPagoService> logger)
        {
            _mpSettings = mpSettings;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // Política de retry para chamadas à API utilizando o alias PollyPolicy
            _retryPolicy = PollyPolicy.Handle<Exception>()
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
                    Email = pedido.UsuarioId,
                    Name = pedido.UsuarioId,
                    Identification = new IdentificationRequest
                    {
                        Type = "CPF",
                        Number = "00000000000"
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

                // 5) Enviar para o Mercado Pago com retry policy
                var preference = await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PreferenceClient();
                    return await client.CreateAsync(request);
                });

                // 6) Retorna a URL de pagamento
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
                        throw new ArgumentException("PaymentId deve ser um número válido");

                    var client = new PaymentClient();
                    return await client.GetAsync(paymentIdLong);
                });
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
                        throw new ArgumentException("PaymentId deve ser um número válido");

                    var client = new PaymentClient();
                    // Usando RequestOptions vazio pois PaymentCancelRequest não é aceito
                    var payment = await client.CancelAsync(paymentIdLong, new RequestOptions());
                    return payment.Status == "cancelled";
                });
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
                        throw new ArgumentException("PaymentId deve ser um número válido");

                    var client = new PaymentClient();

                    if (valor.HasValue)
                    {
                        // Caso de reembolso parcial: como o SDK não suporta passar o valor via RequestOptions,
                        // optamos por realizar uma chamada manual à API REST do MercadoPago.
                        return await ProcessarReembolsoParcialManualAsync(paymentIdLong, valor.Value);
                    }
                    else
                    {
                        var result = await client.RefundAsync(paymentIdLong);
                        return result.Status == "approved";
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reembolsar pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao processar reembolso: {ex.Message}", ex);
            }
        }

        // Implementação manual para reembolso parcial usando HttpClient
        private async Task<bool> ProcessarReembolsoParcialManualAsync(long paymentIdLong, decimal valor)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                var content = new StringContent(
                    JsonSerializer.Serialize(new { amount = valor }),
                    Encoding.UTF8,
                    "application/json");

                // Note que o endpoint pode variar conforme a documentação da API do MercadoPago.
                var response = await httpClient.PostAsync(
                    $"https://api.mercadopago.com/v1/payments/{paymentIdLong}/refund",
                    content);

                response.EnsureSuccessStatusCode();

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(resultJson).RootElement;
                // Verifica se o status retornado indica sucesso
                return result.GetProperty("status").GetString()?.ToLower() == "approved";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar reembolso parcial manual para pagamento {PaymentId}", paymentIdLong);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoAsync()
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<IEnumerable<PaymentMethodDto>>(async () =>
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
                });
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
                });
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
                });
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
            try
            {
                return await _retryPolicy.ExecuteAsync<string>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var content = new StringContent(
                        JsonSerializer.Serialize(new { url }),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(
                        "https://api.mercadopago.com/v1/webhooks",
                        content);

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();
                    var webhookData = JsonDocument.Parse(result);
                    return webhookData.RootElement.GetProperty("id").GetString();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar webhook no Mercado Pago");
                throw new Exception("Falha ao registrar webhook", ex);
            }
        }

        public async Task<bool> ValidarWebhookNotificacaoAsync(string notificationId, string topic)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    if (topic != "payment" && topic != "merchant_order")
                    {
                        _logger.LogWarning("Tópico de notificação não suportado: {topic}", topic);
                        return false;
                    }

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var response = await httpClient.GetAsync(
                        $"https://api.mercadopago.com/v1/{topic}s/{notificationId}");

                    return response.IsSuccessStatusCode;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar notificação webhook {id}", notificationId);
                return false;
            }
        }

        public async Task<CustomerDto> CriarClienteAsync(ClienteDto cliente)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<CustomerDto>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var customerRequest = new
                    {
                        email = cliente.Email,
                        first_name = cliente.Nome,
                        last_name = cliente.Sobrenome,
                        phone = new
                        {
                            area_code = cliente.DddTelefone,
                            number = cliente.NumeroTelefone
                        },
                        identification = new
                        {
                            type = "CPF",
                            number = cliente.Cpf
                        },
                        address = new
                        {
                            zip_code = cliente.Cep,
                            street_name = cliente.Logradouro,
                            street_number = cliente.Numero
                        }
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(customerRequest),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(
                        "https://api.mercadopago.com/v1/customers",
                        content);

                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CustomerDto>(resultJson);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente no Mercado Pago");
                throw new Exception("Falha ao criar cliente", ex);
            }
        }

        public async Task<CustomerDto> ObterClienteAsync(string clienteId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<CustomerDto>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var response = await httpClient.GetAsync(
                        $"https://api.mercadopago.com/v1/customers/{clienteId}");

                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CustomerDto>(resultJson);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cliente do Mercado Pago");
                throw new Exception("Falha ao obter cliente", ex);
            }
        }

        public async Task<string> SalvarCartaoAsync(string clienteId, CartaoCreditoDto cartao)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<string>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var cardRequest = new
                    {
                        token = cartao.Token,
                        payment_method_id = cartao.PaymentMethodId
                    };

                    var content = new StringContent(
                        JsonSerializer.Serialize(cardRequest),
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.PostAsync(
                        $"https://api.mercadopago.com/v1/customers/{clienteId}/cards",
                        content);

                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    var cardData = JsonDocument.Parse(resultJson);
                    return cardData.RootElement.GetProperty("id").GetString();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cartão no Mercado Pago");
                throw new Exception("Falha ao salvar cartão", ex);
            }
        }

        public async Task<IEnumerable<CartaoSalvoDto>> ObterCartoesClienteAsync(string clienteId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<IEnumerable<CartaoSalvoDto>>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var response = await httpClient.GetAsync(
                        $"https://api.mercadopago.com/v1/customers/{clienteId}/cards");

                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<CartaoSalvoDto>>(resultJson);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter cartões do cliente no Mercado Pago");
                throw new Exception("Falha ao obter cartões", ex);
            }
        }

        public async Task<IEnumerable<PaymentMethodDto>> ObterMetodosPagamentoPorPaisAsync(string countryId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<IEnumerable<PaymentMethodDto>>(async () =>
                {
                    var client = new PaymentMethodClient();
                    var paymentMethods = await client.ListAsync();

                    // Removido filtro por Countries, pois essa propriedade não existe na versão atual.
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter métodos de pagamento por país");
                throw new Exception($"Erro ao consultar métodos de pagamento para o país {countryId}", ex);
            }
        }

        public async Task<RefundResponse> ProcessarReembolsoTotalAsync(string paymentId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    if (!long.TryParse(paymentId, out long paymentIdLong))
                        throw new ArgumentException("PaymentId deve ser um número válido");

                    var client = new PaymentClient();
                    var result = await client.RefundAsync(paymentIdLong);

                    return new RefundResponse
                    {
                        Id = result.Id.ToString(),
                        PaymentId = paymentId,
                        Status = result.Status,
                        Amount = result.Amount ?? 0m,
                        DateCreated = result.DateCreated ?? DateTime.MinValue
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar reembolso total para pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao processar reembolso total: {ex.Message}", ex);
            }
        }

        // O método ProcessarReembolsoParcialAsync utiliza uma implementação manual via HttpClient
        public async Task<RefundResponse> ProcessarReembolsoParcialAsync(string paymentId, decimal valor)
        {
            try
            {
                if (!long.TryParse(paymentId, out long paymentIdLong))
                    throw new ArgumentException("PaymentId deve ser um número válido");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                var content = new StringContent(
                    JsonSerializer.Serialize(new { amount = valor }),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.PostAsync(
                    $"https://api.mercadopago.com/v1/payments/{paymentIdLong}/refund",
                    content);

                response.EnsureSuccessStatusCode();

                var resultJson = await response.Content.ReadAsStringAsync();
                var result = JsonDocument.Parse(resultJson).RootElement;

                return new RefundResponse
                {
                    Id = result.GetProperty("id").GetString(),
                    PaymentId = paymentId,
                    Status = result.GetProperty("status").GetString(),
                    Amount = result.TryGetProperty("amount", out JsonElement amt) ? amt.GetDecimal() : 0m,
                    DateCreated = result.TryGetProperty("date_created", out JsonElement dt) ? dt.GetDateTime() : DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar reembolso parcial para pagamento {PaymentId}", paymentId);
                throw new Exception($"Erro ao processar reembolso parcial: {ex.Message}", ex);
            }
        }

        public async Task<PreferenceDetailsDto> ObterDetalhesPreferenciaAsync(string preferenceId)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var client = new PreferenceClient();
                    var preference = await client.GetAsync(preferenceId);

                    return new PreferenceDetailsDto
                    {
                        Id = preference.Id,
                        InitPoint = preference.InitPoint,
                        SandboxInitPoint = preference.SandboxInitPoint,
                        ExternalReference = preference.ExternalReference,
                        Items = preference.Items.Select(i => new PreferenceItemDto
                        {
                            Id = i.Id,
                            Title = i.Title,
                            Description = i.Description,
                            PictureUrl = i.PictureUrl,
                            CategoryId = i.CategoryId,
                            Quantity = i.Quantity,
                            CurrencyId = i.CurrencyId,
                            UnitPrice = i.UnitPrice
                        }).ToList(),
                        Payer = new PreferencePayerDto
                        {
                            Email = preference.Payer.Email,
                            Name = preference.Payer.Name
                        },
                        DateCreated = preference.DateCreated,
                        ExpirationDateFrom = preference.ExpirationDateFrom,
                        ExpirationDateTo = preference.ExpirationDateTo
                    };
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter detalhes da preferência {PreferenceId}", preferenceId);
                throw new Exception($"Erro ao consultar preferência: {ex.Message}", ex);
            }
        }

        public async Task<TransactionReportDto> GerarRelatorioTransacoesAsync(DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync<TransactionReportDto>(async () =>
                {
                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _mpSettings.Value.AccessToken);

                    var from = dataInicio.ToString("yyyy-MM-dd");
                    var to = dataFim.ToString("yyyy-MM-dd");

                    var response = await httpClient.GetAsync(
                        $"https://api.mercadopago.com/v1/reports/payment?begin_date={from}T00:00:00Z&end_date={to}T23:59:59Z");

                    response.EnsureSuccessStatusCode();

                    var resultJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TransactionReportDto>(resultJson);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de transações");
                throw new Exception("Falha ao gerar relatório de transações", ex);
            }
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
