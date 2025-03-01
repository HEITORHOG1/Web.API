using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Domain.Entities;
using Web.Infrastructure.Data.Models;

namespace Web.Infrastructure.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Estabelecimento> Estabelecimentos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<UsuarioEstabelecimento> UsuariosEstabelecimentos { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<OpcaoProduto> OpcoesProduto { get; set; }
        public DbSet<ValorOpcaoProduto> ValoresOpcaoProduto { get; set; }
        public DbSet<AdicionalProduto> AdicionaisProduto { get; set; }
        public DbSet<ItemPedidoOpcao> ItensPedidoOpcoes { get; set; }
        public DbSet<ItemPedidoAdicional> ItensPedidoAdicionais { get; set; }
        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<NotaFiscalProduto> NotasFiscaisProdutos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<ImagemProduto> ImagensProduto { get; set; }
        public DbSet<HorarioFuncionamento> HorariosFuncionamento { get; set; }
        public DbSet<EnderecoCliente> EnderecoClientes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ================================================
            // Configurações da entidade RefreshToken
            // ================================================
            builder.Entity<RefreshToken>()
                .HasKey(rt => rt.Id);

            builder.Entity<HorarioFuncionamento>()
                .HasKey(h => h.Id);

            builder.Entity<HorarioFuncionamento>()
                .HasOne(h => h.Estabelecimento)
                .WithMany(e => e.HorariosFuncionamento)
                .HasForeignKey(h => h.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HorarioFuncionamento>()
                .Property(h => h.DiaSemana)
                .IsRequired();

            builder.Entity<HorarioFuncionamento>()
                .Property(h => h.HoraAbertura)
                .IsRequired();

            builder.Entity<HorarioFuncionamento>()
                .Property(h => h.HoraFechamento)
                .IsRequired();

            // ================================================
            // Configurações da entidade Estabelecimento
            // ================================================
            builder.Entity<Estabelecimento>()
                .HasKey(e => e.Id);

            // ================================================
            // Configurações da entidade UsuarioEstabelecimento
            // ================================================
            builder.Entity<UsuarioEstabelecimento>()
                .HasKey(ue => ue.Id);

            builder.Entity<UsuarioEstabelecimento>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UsuarioEstabelecimento>()
                .HasOne<Estabelecimento>()
                .WithMany()
                .HasForeignKey(ue => ue.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UsuarioEstabelecimento>()
                .HasOne(ue => ue.Usuario)
                .WithMany()
                .HasForeignKey(ue => ue.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UsuarioEstabelecimento>()
                .HasOne(ue => ue.Estabelecimento)
                .WithMany()
                .HasForeignKey(ue => ue.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade ErrorLog
            // ================================================
            builder.Entity<ErrorLog>()
                .HasKey(e => e.Id);

            // ================================================
            // Configurações da entidade Categoria
            // ================================================
            builder.Entity<Categoria>()
                .HasKey(c => c.Id);

            builder.Entity<Categoria>()
                .HasOne(c => c.Estabelecimento)
                .WithMany(e => e.Categorias)
                .HasForeignKey(c => c.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade Produto
            // ================================================
            builder.Entity<Produto>()
                .HasKey(p => p.Id);

            builder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Produto>()
                .HasOne(p => p.Estabelecimento)
                .WithMany(e => e.Produtos)
                .HasForeignKey(p => p.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade MovimentacaoEstoque
            // ================================================
            builder.Entity<MovimentacaoEstoque>()
                .HasKey(me => me.Id);

            builder.Entity<MovimentacaoEstoque>()
                .HasOne(me => me.Produto)
                .WithMany()
                .HasForeignKey(me => me.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MovimentacaoEstoque>()
                .HasOne(me => me.Estabelecimento)
                .WithMany()
                .HasForeignKey(me => me.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade Pedido
            // ================================================
            builder.Entity<Pedido>()
                .HasKey(p => p.Id);

            builder.Entity<Pedido>()
                .HasOne(p => p.Estabelecimento)
                .WithMany(e => e.Pedidos)
                .HasForeignKey(p => p.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Pedido>()
                .HasOne(p => p.Entrega)
                .WithOne(e => e.Pedido)
                .HasForeignKey<Entrega>(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade ItemPedido
            // ================================================
            builder.Entity<ItemPedido>()
                .HasKey(ip => ip.Id);

            builder.Entity<ItemPedido>()
                .HasOne(ip => ip.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(ip => ip.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemPedido>()
                .HasOne(ip => ip.Produto)
                .WithMany()
                .HasForeignKey(ip => ip.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade CarrinhoItem
            // ================================================
            builder.Entity<CarrinhoItem>()
                .HasKey(ci => ci.Id);

            builder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Produto)
                .WithMany()
                .HasForeignKey(ci => ci.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CarrinhoItem>()
                .HasOne(ci => ci.Estabelecimento)
                .WithMany()
                .HasForeignKey(ci => ci.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade Entregador
            // ================================================
            builder.Entity<Entregador>()
                .HasKey(e => e.Id);

            builder.Entity<Entregador>()
                .HasOne(e => e.Estabelecimento)
                .WithMany()
                .HasForeignKey(e => e.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade Entrega
            // ================================================
            builder.Entity<Entrega>()
                .HasKey(e => e.Id);

            builder.Entity<Entrega>()
                .HasOne(e => e.Pedido)
                .WithOne(p => p.Entrega)
                .HasForeignKey<Entrega>(e => e.PedidoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Entrega>()
                .HasOne(e => e.Entregador)
                .WithMany(en => en.Entregas)
                .HasForeignKey(e => e.EntregadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade OpcaoProduto
            // ================================================
            builder.Entity<OpcaoProduto>()
                .HasKey(op => op.Id);

            builder.Entity<OpcaoProduto>()
                .HasOne(op => op.Produto)
                .WithMany(p => p.Opcoes)
                .HasForeignKey(op => op.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade ValorOpcaoProduto
            // ================================================
            builder.Entity<ValorOpcaoProduto>()
                .HasKey(vop => vop.Id);

            builder.Entity<ValorOpcaoProduto>()
                .HasOne(vop => vop.OpcaoProduto)
                .WithMany(op => op.Valores)
                .HasForeignKey(vop => vop.OpcaoProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade AdicionalProduto
            // ================================================
            builder.Entity<AdicionalProduto>()
                .HasKey(ap => ap.Id);

            builder.Entity<AdicionalProduto>()
                .HasOne(ap => ap.Produto)
                .WithMany(p => p.Adicionais)
                .HasForeignKey(ap => ap.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ================================================
            // Configurações da entidade ItemPedidoOpcao
            // ================================================
            builder.Entity<ItemPedidoOpcao>()
                .HasKey(ipo => ipo.Id);

            builder.Entity<ItemPedidoOpcao>()
                .HasOne(ipo => ipo.ItemPedido)
                .WithMany(ip => ip.OpcoesSelecionadas)
                .HasForeignKey(ipo => ipo.ItemPedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemPedidoOpcao>()
                .HasOne(ipo => ipo.ValorOpcaoProduto)
                .WithMany()
                .HasForeignKey(ipo => ipo.ValorOpcaoProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade ItemPedidoAdicional
            // ================================================
            builder.Entity<ItemPedidoAdicional>()
                .HasKey(ipa => ipa.Id);

            builder.Entity<ItemPedidoAdicional>()
                .HasOne(ipa => ipa.ItemPedido)
                .WithMany(ip => ip.AdicionaisSelecionados)
                .HasForeignKey(ipa => ipa.ItemPedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemPedidoAdicional>()
                .HasOne(ipa => ipa.AdicionalProduto)
                .WithMany()
                .HasForeignKey(ipa => ipa.AdicionalProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NotaFiscal>()
                .HasKey(nf => nf.Id);

            builder.Entity<NotaFiscalProduto>()
                .HasKey(nfp => nfp.Id);

            // ================================================
            // Configurações da entidade NotaFiscal
            // ================================================
            builder.Entity<NotaFiscal>()
                .HasKey(nf => nf.Id);

            builder.Entity<NotaFiscal>()
                .HasOne(nf => nf.Estabelecimento)
                .WithMany(e => e.NotasFiscais)
                .HasForeignKey(nf => nf.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<NotaFiscal>()
                .HasOne(nf => nf.Fornecedor)
                .WithMany(f => f.NotasFiscais)
                .HasForeignKey(nf => nf.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade NotaFiscalProduto
            // ================================================
            builder.Entity<NotaFiscalProduto>()
                .HasKey(nfp => nfp.Id);

            builder.Entity<NotaFiscalProduto>()
                .HasOne(nfp => nfp.NotaFiscal)
                .WithMany(nf => nf.Produtos)
                .HasForeignKey(nfp => nfp.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<NotaFiscalProduto>()
                .HasOne(nfp => nfp.Produto)
                .WithMany()
                .HasForeignKey(nfp => nfp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ================================================
            // Configurações da entidade Fornecedor
            // ================================================
            builder.Entity<Fornecedor>()
                .HasKey(f => f.Id);

            builder.Entity<Fornecedor>()
                .HasIndex(f => f.CNPJ)
                .IsUnique()
                .HasDatabaseName("IX_Fornecedor_CNPJ_Unique");

            builder.Entity<Fornecedor>()
                .HasIndex(f => f.Nome)
                .HasDatabaseName("IX_Fornecedor_Nome_Search");

            // No OnModelCreating
            builder.Entity<ImagemProduto>()
                .HasKey(i => i.Id);

            builder.Entity<ImagemProduto>()
                .HasOne(i => i.Produto)
                .WithMany(p => p.Imagens)
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ImagemProduto>()
                .HasIndex(i => i.ProdutoId)
                .HasDatabaseName("IX_ImagemProduto_ProdutoId");

            builder.Entity<ImagemProduto>()
                .HasOne(i => i.Estabelecimento)
                .WithMany()
                .HasForeignKey(i => i.EstabelecimentoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ImagemProduto>()
                .HasIndex(i => new { i.EstabelecimentoId, i.ProdutoId })
                .HasDatabaseName("IX_ImagemProduto_Estabelecimento_Produto");

            // ================================================
            // Índices Otimizados - Apenas Novos
            // ================================================

            // RefreshToken
            builder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("IX_RefreshToken_Token_Unique");

            // Estabelecimento
            builder.Entity<Estabelecimento>()
                .HasIndex(e => e.CNPJ)
                .IsUnique()
                .HasDatabaseName("IX_Estabelecimento_CNPJ_Unique");

            builder.Entity<Estabelecimento>()
                .HasIndex(e => e.NomeFantasia)
                .HasDatabaseName("IX_Estabelecimento_NomeFantasia_Search");

            // Produto
            builder.Entity<Produto>()
                .HasIndex(p => p.Nome)
                .HasDatabaseName("IX_Produto_Nome_Search");

            builder.Entity<Produto>()
                .HasIndex(p => p.Disponivel)
                .HasDatabaseName("IX_Produto_Disponivel_Status");

            // MovimentacaoEstoque
            builder.Entity<MovimentacaoEstoque>()
                .HasIndex(me => me.DataMovimentacao)
                .HasDatabaseName("IX_MovimentacaoEstoque_Data_Search");

            // Pedido
            builder.Entity<Pedido>()
                .HasIndex(p => new { p.Status, p.DataCriacao })
                .HasDatabaseName("IX_Pedido_Status_Data_Search");

            builder.Entity<Pedido>()
                .HasIndex(p => p.DataCriacao)
                .HasDatabaseName("IX_Pedido_Data_Search");

            // Entregador
            builder.Entity<Entregador>()
                .HasIndex(e => e.Documento)
                .IsUnique()
                .HasDatabaseName("IX_Entregador_Documento_Unique");

            // NotaFiscal
            builder.Entity<NotaFiscal>()
                .HasIndex(nf => nf.Numero)
                .IsUnique()
                .HasDatabaseName("IX_NotaFiscal_Numero_Unique");

            // Entrega
            builder.Entity<Entrega>()
                .HasIndex(e => e.Status)
                .HasDatabaseName("IX_Entrega_Status_Search");

            // CarrinhoItem
            builder.Entity<CarrinhoItem>()
                .HasIndex(ci => new { ci.UsuarioId, ci.EstabelecimentoId })
                .HasDatabaseName("IX_CarrinhoItem_Usuario_Estabelecimento_Search");
        }
    }
}