using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Web.Domain.DTOs.ValidacaoPreco;

namespace Web.Domain.DTOs.Produtos
{
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço do produto é obrigatório.")]
        [CustomPriceValidation(ErrorMessage = "O preço deve estar entre R$ 0,01 e R$ 99.999,99")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A imagem é obrigatória.")]
        public IFormFile Imagem { get; set; }

        public bool Disponivel { get; set; } = true;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoriaId { get; set; }

        public string? CodigoDeBarras { get; set; }
    }
}