namespace Web.Domain.Enums
{
    /// <summary>
    /// Representa os diferentes níveis de acesso que um usuário pode ter em relação a um estabelecimento.
    /// </summary>
    public enum NivelAcesso
    {
        /// <summary>
        /// Papel de cliente, utilizado para usuários que consomem os serviços ou produtos do estabelecimento.
        /// </summary>
        Cliente = 1,

        /// <summary>
        /// Papel de proprietário, responsável por gerenciar um ou mais estabelecimentos.
        /// </summary>
        Proprietario = 2,

        /// <summary>
        /// Papel de funcionário, representando trabalhadores que possuem permissões específicas para executar tarefas no estabelecimento.
        /// </summary>
        Funcionario = 3,

        /// <summary>
        /// Papel de administrador, com permissões completas para gerenciar o sistema como um todo.
        /// </summary>
        Administrador = 4,

        /// <summary>
        /// Papel de gerente, com controle e responsabilidade sobre operações de um ou mais estabelecimentos.
        /// </summary>
        Gerente = 5,

        /// <summary>
        /// Papel de atendente, focado em tarefas de atendimento ao cliente e operações diárias.
        /// </summary>
        Atendente = 6,

        /// <summary>
        /// Papel de cozinheiro, responsável pela preparação dos alimentos.
        /// </summary>
        Cozinheiro = 7,

        /// <summary>
        /// Papel de caixa, responsável por operações de pagamento e fechamento de contas.
        /// </summary>
        Caixa = 8,

        /// <summary>
        /// Papel de garçom, responsável por servir os clientes.
        /// </summary>
        Garcom = 9,

        /// <summary>
        /// Papel de entregador, responsável por entregar pedidos aos clientes.
        /// </summary>
        Entregador = 10
    }
}