﻿using MarketplaceHybrid.Shared.Configurations;
using MarketplaceHybrid.Shared.Models;
using MarketplaceHybrid.Shared.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Services
{
    public class HorarioFuncionamentoService : IHorarioFuncionamentoService
    {
        private readonly HttpClient _httpClient;

        public HorarioFuncionamentoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> EstaAbertoAsync(int estabelecimentoId, int diaSemana, string horaAtual)
        {
            var response = await _httpClient.GetFromJsonAsync<HorarioFuncionamentoResponse>(
                $"{Endpoints.EstaAberto}?estabelecimentoId={estabelecimentoId}&diaSemana={diaSemana}&horaAtual={horaAtual}");

            return response?.Aberto ?? false;
        }
    }
}
