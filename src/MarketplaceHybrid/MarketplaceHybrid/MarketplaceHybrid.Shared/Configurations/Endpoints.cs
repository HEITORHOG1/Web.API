namespace MarketplaceHybrid.Shared.Configurations
{
    public static class Endpoints
    {
        public const string GetEstabelecimentoById = BaseEndPoint.BaseUrl + "/Estabelecimento/estabelecimento-especifico";
        public const string GetProdutosByEstabelecimentoId = BaseEndPoint.BaseUrl + "/Estabelecimento/produtos-estabelecimento-especifico";
        public const string GetProdutoById = BaseEndPoint.BaseUrl + "/Estabelecimento/{0}/produtos/{1}";
        public const int EstabelecimentoID = 51;
        public const string AuthLogin = BaseEndPoint.BaseUrl + "/Auth/login-cliente";
        public const string AuthRegister = BaseEndPoint.BaseUrl + "/Auth/registercliente";
        public const string CheckUsernameExists = "/Auth/username-exists";
        public const string EstaAberto = BaseEndPoint.BaseUrl + "/HorarioFuncionamento/esta-aberto";

        public const string FinalizarPedido = BaseEndPoint.BaseUrl + "/Cliente/finalizar-compra";
        public const string GetCarrinhoItens = BaseEndPoint.BaseUrl + "/Carrinho/itens";

        public const string AddEnderecoCliente = BaseEndPoint.BaseUrl + "/EnderecoCliente";
        public const string GetEnderecoPrincipalCliente = BaseEndPoint.BaseUrl + "/EnderecoCliente/Principal";
        public const string GetEnderecoCliente = BaseEndPoint.BaseUrl + "/EnderecoCliente";
    }
}