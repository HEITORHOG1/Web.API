using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketplaceHybrid.Shared.Models
{
    public class ProdutoResponse
    {
        public List<Cardapio> Cardapio { get; set; }
    }

    public class Cardapio
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string Imagem { get; set; }
        public bool Disponivel { get; set; }
        public int CategoriaId { get; set; }
        public string NomeCategoria { get; set; }
        public int QuantidadeEmEstoque { get; set; }
        public int QuantidadeReservada { get; set; }
        public string CodigoDeBarras { get; set; }
        public List<OpcaoProduto> Opcoes { get; set; }
        public List<Adicional> Adicionais { get; set; }
    }

    public class OpcaoProduto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public bool Obrigatorio { get; set; }
        public List<ValorOpcao> Valores { get; set; }
    }

    public class ValorOpcao
    {
        public int Id { get; set; }
        public int OpcaoProdutoId { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoAdicional { get; set; }
    }

    public class Adicional
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
    }
}
