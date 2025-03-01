namespace Web.Domain.Enums
{
    public static class UserRoles
    {
        public const string Cliente = "Cliente";
        public const string Proprietario = "Proprietario";
        public const string Funcionario = "Funcionario";
        public const string Administrador = "Administrador";
        public const string Gerente = "Gerente";
        public const string Atendente = "Atendente";
        public const string Cozinheiro = "Cozinheiro";
        public const string Caixa = "Caixa";
        public const string Garcom = "Garcom";
        public const string Entregador = "Entregador";

        public static readonly IReadOnlyDictionary<string, string[]> Permissions = new Dictionary<string, string[]>
        {
            {
                Proprietario, new[]
                {
                "estabelecimento.gerenciar",
                "funcionarios.gerenciar",
                "cardapio.gerenciar",
                "financeiro.visualizar",
                "relatorios.visualizar",
                "fornecedores.gerenciar",
                "fornecedores.visualizar"
            }},
            {
                Gerente, new[]
                {
                "pedidos.gerenciar",
                "cardapio.gerenciar",
                "estoque.gerenciar",
                "funcionarios.visualizar",
                "fornecedores.gerenciar",
                "fornecedores.visualizar"
            }},
            {
                Cliente, new[]
                {
                "carrinho.gerenciar",
                "pedidos.visualizar",
                "pedidos.criar"
            }}
        };
    }
}