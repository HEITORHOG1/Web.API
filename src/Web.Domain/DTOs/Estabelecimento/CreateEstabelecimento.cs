﻿using Microsoft.AspNetCore.Http;

namespace Web.Domain.DTOs.Estabelecimento
{
    public class CreateEstabelecimento
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CNPJ { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public bool Status { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public decimal? TaxaEntregaFixa { get; set; }
        public IFormFile? UrlImagem { get; set; }
        public string? Descricao { get; set; }
    }
}