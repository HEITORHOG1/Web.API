namespace Web.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public int EstabelecimentoId { get; private set; }
        public DateTime DataCadastro { get; private set; }

        // Propriedades de navegação (mantenha apenas para uso no domínio)
        public virtual ICollection<Produto> Produtos { get; private set; }
        public virtual Estabelecimento Estabelecimento { get; private set; }

        //Construtor
        public Categoria(string nome, string descricao, int estabelecimentoId)
        {
            Nome = nome;
            Descricao = descricao;
            EstabelecimentoId = estabelecimentoId;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
            Produtos = new List<Produto>();
        }
        //métodos de negócio
        public void AlterarAtividade(bool ativo)
        {
            Ativo = ativo;
        }

        public void AlterarDescricao(string descricao)
        {
            Descricao = descricao;
        }
        public void AlterarNome(string nome)
        {
            Nome = nome;
        }

        // Construtor vazio para o EF Core (se necessário)
        protected Categoria()
        {
            // EF Only
        }
    }
}