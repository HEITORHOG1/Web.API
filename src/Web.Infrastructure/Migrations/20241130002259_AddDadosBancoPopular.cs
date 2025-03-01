using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDadosBancoPopular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 0;");

            // Inserir Estabelecimentos
            migrationBuilder.Sql(@"
                INSERT INTO Estabelecimentos (UsuarioId, RazaoSocial, NomeFantasia, CNPJ, Telefone, Endereco, HorarioFuncionamento, Status, DataCadastro) VALUES
                ('a043e175-d836-49a0-aee4-647219143f76', 'Restaurante Bom Sabor LTDA', 'Restaurante Bom Sabor', '11.111.111/0001-11', '(11) 1111-1111', 'Rua das Flores, 123', 'Seg-Sex: 08h-22h', 1, NOW()),
                ('b06b73c4-5d3d-411c-ab67-52d001ec5c84', 'Lanchonete Express LTDA', 'Lanche Express', '22.222.222/0001-22', '(22) 2222-2222', 'Av Brasil, 456', 'Seg-Sab: 10h-23h', 1, NOW()),
                ('a043e175-d836-49a0-aee4-647219143f76', 'Pizzaria Bella Italia LTDA', 'Bella Italia', '33.333.333/0001-33', '(33) 3333-3333', 'Rua Roma, 789', 'Ter-Dom: 18h-00h', 1, NOW()),
                ('b06b73c4-5d3d-411c-ab67-52d001ec5c84', 'Cafeteria Aroma LTDA', 'Café Aroma', '44.444.444/0001-44', '(44) 4444-4444', 'Av Paris, 321', 'Seg-Sab: 07h-20h', 1, NOW()),
                ('a043e175-d836-49a0-aee4-647219143f76', 'Doceria Doce Sabor LTDA', 'Doce Sabor', '55.555.555/0001-55', '(55) 5555-5555', 'Rua dos Doces, 654', 'Seg-Dom: 09h-21h', 1, NOW())
            ");

            // Inserir Fornecedores vinculados aos estabelecimentos
            migrationBuilder.Sql(@"
                INSERT INTO Fornecedores (Nome, CNPJ, Endereco, Telefone, Email, Ativo, DataCreate, EstabelecimentoId) VALUES
                ('Distribuidora de Alimentos SA', '12.345.678/0001-01', 'Rua dos Atacadistas, 100', '(11) 2222-1111', 'contato@distalimentos.com', 1, NOW(), 1),
                ('Bebidas & CIA LTDA', '23.456.789/0001-02', 'Av das Bebidas, 200', '(11) 2222-2222', 'vendas@bebidascia.com', 1, NOW(), 1),
                ('Hortifruti Express', '34.567.890/0001-03', 'Rua das Frutas, 300', '(11) 2222-3333', 'comercial@hortiexpress.com', 1, NOW(), 1),
                ('Carnes Premium SA', '45.678.901/0001-04', 'Av dos Frigoríficos, 400', '(11) 2222-4444', 'vendas@carnespremium.com', 1, NOW(), 2),
                ('Laticínios Qualidade LTDA', '56.789.012/0001-05', 'Rua do Leite, 500', '(11) 2222-5555', 'contato@laticinios.com', 1, NOW(), 2),
                ('Massas Italianas IMP', '67.890.123/0001-06', 'Av da Itália, 600', '(11) 2222-6666', 'import@massasitalianas.com', 1, NOW(), 3),
                ('Queijos & Frios SA', '78.901.234/0001-07', 'Rua dos Queijos, 700', '(11) 2222-7777', 'vendas@queijosefrios.com', 1, NOW(), 3),
                ('Café Premium LTDA', '89.012.345/0001-08', 'Av do Café, 800', '(11) 2222-8888', 'comercial@cafepremium.com', 1, NOW(), 4),
                ('Doces & Cia', '90.123.456/0001-09', 'Rua dos Doces, 900', '(11) 2222-9999', 'vendas@docesecia.com', 1, NOW(), 4),
                ('Confeitaria Insumos', '01.234.567/0001-10', 'Av dos Confeitos, 1000', '(11) 3333-1111', 'contato@confeitariainsumos.com', 1, NOW(), 5),
                ('Embalagens Especiais', '12.345.678/0001-11', 'Rua das Embalagens, 1100', '(11) 3333-2222', 'vendas@embalagensbrasil.com', 1, NOW(), 5)
            ");

            // Inserir Categorias por estabelecimento
            migrationBuilder.Sql(@"
                INSERT INTO Categorias (Nome, Descricao, Ativo, EstabelecimentoId, DataCadastro) VALUES
                ('Entradas', 'Pratos para iniciar sua refeição', 1, 1, NOW()),
                ('Pratos Principais', 'Pratos principais do cardápio', 1, 1, NOW()),
                ('Sobremesas', 'Deliciosas sobremesas', 1, 1, NOW()),
                ('Bebidas Alcoólicas', 'Cervejas, vinhos e destilados', 1, 1, NOW()),
                ('Bebidas não Alcoólicas', 'Refrigerantes, sucos e águas', 1, 1, NOW()),
                ('Lanches', 'Hambúrgueres e sanduíches', 1, 2, NOW()),
                ('Porções', 'Porções para compartilhar', 1, 2, NOW()),
                ('Combos', 'Combinações especiais', 1, 2, NOW()),
                ('Bebidas', 'Todas as bebidas', 1, 2, NOW()),
                ('Pizzas Tradicionais', 'Sabores clássicos', 1, 3, NOW()),
                ('Pizzas Premium', 'Sabores especiais', 1, 3, NOW()),
                ('Pizzas Doces', 'Sobremesas em forma de pizza', 1, 3, NOW()),
                ('Cafés Especiais', 'Cafés gourmet e especiais', 1, 4, NOW()),
                ('Doces Tradicionais', 'Doces caseiros', 1, 5, NOW()),
                ('Bolos', 'Bolos diversos', 1, 5, NOW()),
                ('Tortas', 'Tortas doces e salgadas', 1, 5, NOW())
            ");

            // Inserir Categorias por estabelecimento
            migrationBuilder.Sql(@"
        INSERT INTO Categorias (Nome, Descricao, Ativo, EstabelecimentoId, DataCadastro) VALUES
        -- Categorias do Restaurante Bom Sabor (EstabelecimentoId = 1)
        ('Entradas', 'Pratos para iniciar sua refeição', 1, 1, NOW()),
        ('Pratos Principais', 'Pratos principais do cardápio', 1, 1, NOW()),
        ('Sobremesas', 'Deliciosas sobremesas', 1, 1, NOW()),
        ('Bebidas Alcoólicas', 'Cervejas, vinhos e destilados', 1, 1, NOW()),
        ('Bebidas não Alcoólicas', 'Refrigerantes, sucos e águas', 1, 1, NOW()),

        -- Categorias da Lanchonete Express (EstabelecimentoId = 2)
        ('Lanches', 'Hambúrgueres e sanduíches', 1, 2, NOW()),
        ('Porções', 'Porções para compartilhar', 1, 2, NOW()),
        ('Combos', 'Combinações especiais', 1, 2, NOW()),
        ('Bebidas', 'Todas as bebidas', 1, 2, NOW()),
        ('Sobremesas', 'Sobremesas especiais', 1, 2, NOW()),

        -- Categorias da Pizzaria Bella Italia (EstabelecimentoId = 3)
        ('Pizzas Tradicionais', 'Sabores clássicos', 1, 3, NOW()),
        ('Pizzas Premium', 'Sabores especiais', 1, 3, NOW()),
        ('Pizzas Doces', 'Sobremesas em forma de pizza', 1, 3, NOW()),
        ('Bebidas', 'Bebidas diversas', 1, 3, NOW()),
        ('Sobremesas', 'Sobremesas italianas', 1, 3, NOW()),

        -- Categorias da Cafeteria Aroma (EstabelecimentoId = 4)
        ('Cafés Especiais', 'Cafés gourmet e especiais', 1, 4, NOW()),
        ('Cafés Tradicionais', 'Cafés do dia a dia', 1, 4, NOW()),
        ('Bebidas Geladas', 'Frapuccinos e bebidas geladas', 1, 4, NOW()),
        ('Salgados', 'Opções para seu café', 1, 4, NOW()),
        ('Doces', 'Doces e sobremesas', 1, 4, NOW()),

        -- Categorias da Doceria Doce Sabor (EstabelecimentoId = 5)
        ('Doces Tradicionais', 'Doces caseiros', 1, 5, NOW()),
        ('Bolos', 'Bolos diversos', 1, 5, NOW()),
        ('Tortas', 'Tortas doces e salgadas', 1, 5, NOW()),
        ('Chocolates', 'Chocolates especiais', 1, 5, NOW()),
        ('Bebidas', 'Bebidas para acompanhar', 1, 5, NOW())
    ");

            // Continuar Produtos - Lanchonete Express (EstabelecimentoId = 2)
            migrationBuilder.Sql(@"
        -- Combos (CategoriaId = 8)
        INSERT INTO Produtos (Nome, Descricao, Preco, Imagem, Disponivel, CategoriaId, EstabelecimentoId, DataCadastro, QuantidadeEmEstoque, CodigoDeBarras) VALUES
        ('Combo Família', 'X-Burger + Batata + 4 Refrigerantes', 89.90, 'combo1.jpg', 1, 8, 2, NOW(), 30, '7891234567022'),
        ('Combo Duplo', '2 X-Burgers + Batata + 2 Refrigerantes', 69.90, 'combo2.jpg', 1, 8, 2, NOW(), 40, '7891234567023'),
        ('Combo Kids', 'Hambúrguer Kids + Batata + Refrigerante + Brinquedo', 39.90, 'combo3.jpg', 1, 8, 2, NOW(), 25, '7891234567024'),

        -- Bebidas Lanchonete (CategoriaId = 9)
        ('Refrigerante 350ml', 'Lata', 6.90, 'refri1.jpg', 1, 9, 2, NOW(), 200, '7891234567025'),
        ('Suco Natural', 'Laranja ou Limão', 8.90, 'suco1.jpg', 1, 9, 2, NOW(), 50, '7891234567026'),
        ('Milk Shake', 'Chocolate, Morango ou Baunilha', 15.90, 'milkshake.jpg', 1, 9, 2, NOW(), 40, '7891234567027'),

        -- Sobremesas Lanchonete (CategoriaId = 10)
        ('Sorvete', 'Duas bolas com calda', 12.90, 'sorvete1.jpg', 1, 10, 2, NOW(), 60, '7891234567028'),
        ('Brownie', 'Com sorvete', 16.90, 'brownie.jpg', 1, 10, 2, NOW(), 45, '7891234567029'),
        ('Sundae', 'Com calda e amendoim', 14.90, 'sundae.jpg', 1, 10, 2, NOW(), 55, '7891234567030')
    ");

            // Inserir Produtos - Pizzaria Bella Italia (EstabelecimentoId = 3)
            migrationBuilder.Sql(@"
        -- Pizzas Tradicionais (CategoriaId = 11)
        INSERT INTO Produtos (Nome, Descricao, Preco, Imagem, Disponivel, CategoriaId, EstabelecimentoId, DataCadastro, QuantidadeEmEstoque, CodigoDeBarras) VALUES
        ('Margherita', 'Molho, mussarela, tomate e manjericão', 49.90, 'margherita.jpg', 1, 11, 3, NOW(), 40, '7891234567031'),
        ('Calabresa', 'Molho, mussarela e calabresa', 45.90, 'calabresa.jpg', 1, 11, 3, NOW(), 35, '7891234567032'),
        ('Portuguesa', 'Molho, mussarela, presunto, ovo e cebola', 52.90, 'portuguesa.jpg', 1, 11, 3, NOW(), 30, '7891234567033'),

        -- Pizzas Premium (CategoriaId = 12)
        ('Trufada', 'Molho, mussarela de búfala, cogumelos e azeite trufado', 79.90, 'trufada.jpg', 1, 12, 3, NOW(), 20, '7891234567034'),
        ('Parma', 'Molho, mussarela, presunto parma e rúcula', 69.90, 'parma.jpg', 1, 12, 3, NOW(), 25, '7891234567035'),
        ('Salmão', 'Molho, cream cheese, salmão e cebolinha', 75.90, 'salmao_pizza.jpg', 1, 12, 3, NOW(), 15, '7891234567036'),

        -- Pizzas Doces (CategoriaId = 13)
        ('Chocolate', 'Chocolate ao leite e granulado', 45.90, 'chocolate.jpg', 1, 13, 3, NOW(), 30, '7891234567037'),
        ('Romeu e Julieta', 'Goiabada com queijo', 47.90, 'romeu.jpg', 1, 13, 3, NOW(), 25, '7891234567038'),
        ('Banana', 'Banana com canela e doce de leite', 46.90, 'banana.jpg', 1, 13, 3, NOW(), 28, '7891234567039'),

        -- Bebidas Pizzaria (CategoriaId = 14)
        ('Refrigerante 2L', 'Cola ou Guaraná', 12.90, 'refri2l.jpg', 1, 14, 3, NOW(), 100, '7891234567040'),
        ('Vinho da Casa', 'Tinto ou Branco 750ml', 49.90, 'vinho_casa.jpg', 1, 14, 3, NOW(), 50, '7891234567041'),
        ('Cerveja 600ml', 'Pilsen', 14.90, 'cerveja600.jpg', 1, 14, 3, NOW(), 80, '7891234567042'),

        -- Sobremesas Pizzaria (CategoriaId = 15)
        ('Tiramisù', 'Clássico italiano', 25.90, 'tiramisu.jpg', 1, 15, 3, NOW(), 30, '7891234567043'),
        ('Cannoli', 'Recheio de ricota', 18.90, 'cannoli.jpg', 1, 15, 3, NOW(), 40, '7891234567044'),
        ('Gelato', 'Sorvete italiano', 16.90, 'gelato.jpg', 1, 15, 3, NOW(), 35, '7891234567045')
    ");
            // Inserir Produtos - Cafeteria Aroma (EstabelecimentoId = 4)
            migrationBuilder.Sql(@"
        -- Cafés Especiais (CategoriaId = 16)
        INSERT INTO Produtos (Nome, Descricao, Preco, Imagem, Disponivel, CategoriaId, EstabelecimentoId, DataCadastro, QuantidadeEmEstoque, CodigoDeBarras) VALUES
        ('Cappuccino Italiano', 'Café espresso com leite vaporizado e espuma', 12.90, 'cappuccino.jpg', 1, 16, 4, NOW(), 100, '7891234567046'),
        ('Latte Macchiato', 'Café com leite vaporizado em camadas', 13.90, 'latte.jpg', 1, 16, 4, NOW(), 100, '7891234567047'),
        ('Mocha', 'Café com chocolate e leite vaporizado', 14.90, 'mocha.jpg', 1, 16, 4, NOW(), 100, '7891234567048'),

        -- Cafés Tradicionais (CategoriaId = 17)
        ('Café Espresso', 'Café puro tradicional', 6.90, 'espresso.jpg', 1, 17, 4, NOW(), 200, '7891234567049'),
        ('Café com Leite', 'Café coado com leite quente', 8.90, 'cafe_leite.jpg', 1, 17, 4, NOW(), 150, '7891234567050'),
        ('Café Americano', 'Café espresso com água', 7.90, 'americano.jpg', 1, 17, 4, NOW(), 180, '7891234567051'),

        -- Bebidas Geladas (CategoriaId = 18)
        ('Frappuccino', 'Bebida gelada de café com chantilly', 16.90, 'frappuccino.jpg', 1, 18, 4, NOW(), 80, '7891234567052'),
        ('Café Gelado', 'Cold brew com gelo', 11.90, 'cafe_gelado.jpg', 1, 18, 4, NOW(), 90, '7891234567053'),
        ('Smoothie', 'Bebida cremosa de frutas', 15.90, 'smoothie.jpg', 1, 18, 4, NOW(), 70, '7891234567054'),

        -- Salgados (CategoriaId = 19)
        ('Croissant', 'Croissant francês na manteiga', 8.90, 'croissant.jpg', 1, 19, 4, NOW(), 50, '7891234567055'),
        ('Pão de Queijo', 'Pão de queijo mineiro', 5.90, 'pao_queijo.jpg', 1, 19, 4, NOW(), 100, '7891234567056'),
        ('Misto Quente', 'Queijo e presunto no pão francês', 12.90, 'misto_cafe.jpg', 1, 19, 4, NOW(), 60, '7891234567057'),

        -- Doces Cafeteria (CategoriaId = 20)
        ('Bolo de Cenoura', 'Fatia com cobertura de chocolate', 9.90, 'bolo_cenoura.jpg', 1, 20, 4, NOW(), 40, '7891234567058'),
        ('Cookie', 'Cookie com gotas de chocolate', 6.90, 'cookie.jpg', 1, 20, 4, NOW(), 80, '7891234567059'),
        ('Cheesecake', 'Fatia com calda de frutas vermelhas', 13.90, 'cheesecake.jpg', 1, 20, 4, NOW(), 30, '7891234567060')
    ");

            // Inserir Produtos - Doceria Doce Sabor (EstabelecimentoId = 5)
            migrationBuilder.Sql(@"
        -- Doces Tradicionais (CategoriaId = 21)
        INSERT INTO Produtos (Nome, Descricao, Preco, Imagem, Disponivel, CategoriaId, EstabelecimentoId, DataCadastro, QuantidadeEmEstoque, CodigoDeBarras) VALUES
        ('Brigadeiro', 'Brigadeiro tradicional', 3.90, 'brigadeiro.jpg', 1, 21, 5, NOW(), 200, '7891234567061'),
        ('Beijinho', 'Docinho de coco', 3.90, 'beijinho.jpg', 1, 21, 5, NOW(), 180, '7891234567062'),
        ('Cajuzinho', 'Docinho de amendoim', 3.90, 'cajuzinho.jpg', 1, 21, 5, NOW(), 150, '7891234567063'),

        -- Bolos (CategoriaId = 22)
        ('Bolo de Chocolate', 'Bolo recheado com brigadeiro', 89.90, 'bolo_choco.jpg', 1, 22, 5, NOW(), 15, '7891234567064'),
        ('Bolo Red Velvet', 'Bolo vermelho com cream cheese', 99.90, 'red_velvet.jpg', 1, 22, 5, NOW(), 10, '7891234567065'),
        ('Bolo de Festa', 'Bolo personalizado', 129.90, 'bolo_festa.jpg', 1, 22, 5, NOW(), 8, '7891234567066'),

        -- Tortas (CategoriaId = 23)
        ('Torta Holandesa', 'Torta com chocolate e creme', 69.90, 'torta_holandesa.jpg', 1, 23, 5, NOW(), 20, '7891234567067'),
        ('Torta de Limão', 'Torta de limão com merengue', 59.90, 'torta_limao.jpg', 1, 23, 5, NOW(), 25, '7891234567068'),
        ('Torta de Morango', 'Torta com morangos frescos', 79.90, 'torta_morango.jpg', 1, 23, 5, NOW(), 15, '7891234567069'),

        -- Chocolates (CategoriaId = 24)
        ('Trufa', 'Trufa de chocolate belga', 6.90, 'trufa.jpg', 1, 24, 5, NOW(), 100, '7891234567070'),
        ('Bombom Recheado', 'Bombom artesanal', 5.90, 'bombom.jpg', 1, 24, 5, NOW(), 120, '7891234567071'),
        ('Barra Artesanal', 'Barra de chocolate 70%', 19.90, 'barra.jpg', 1, 24, 5, NOW(), 80, '7891234567072'),

        -- Bebidas Doceria (CategoriaId = 25)
        ('Chocolate Quente', 'Chocolate quente cremoso', 12.90, 'chocolate_quente.jpg', 1, 25, 5, NOW(), 50, '7891234567073'),
        ('Café Especial', 'Café gourmet', 8.90, 'cafe_doceria.jpg', 1, 25, 5, NOW(), 100, '7891234567074'),
        ('Milk-shake', 'Milk-shake de chocolate', 16.90, 'milkshake_doceria.jpg', 1, 25, 5, NOW(), 40, '7891234567075')
    ");

            // Opções para produtos do Restaurante Bom Sabor
            migrationBuilder.Sql(@"
        -- Opções para Pratos Principais
        INSERT INTO OpcoesProduto (ProdutoId, Nome, Obrigatorio) VALUES
        -- Filé à Parmegiana (ProdutoId = 4)
        (4, 'Ponto da carne', 1),
        (4, 'Acompanhamento', 1),
        -- Salmão Grelhado (ProdutoId = 5)
        (5, 'Acompanhamento', 1),

        -- Opções para Bebidas Alcoólicas
        -- Vinho Tinto (ProdutoId = 10)
        (10, 'Temperatura', 1),
        -- Cerveja Artesanal (ProdutoId = 11)
        (11, 'Temperatura', 1),

        -- Opções para Bebidas não Alcoólicas
        -- Suco de Laranja (ProdutoId = 13)
        (13, 'Gelo', 0),
        (13, 'Tamanho', 1),
        -- Refrigerante (ProdutoId = 14)
        (14, 'Temperatura', 1)
    ");

            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 0;");

            migrationBuilder.Sql("DELETE FROM ValoresOpcaoProduto");
            migrationBuilder.Sql("DELETE FROM OpcoesProduto");
            migrationBuilder.Sql("DELETE FROM Produtos");
            migrationBuilder.Sql("DELETE FROM Categorias");
            migrationBuilder.Sql("DELETE FROM Fornecedores");
            migrationBuilder.Sql("DELETE FROM Estabelecimentos");
            migrationBuilder.Sql("DELETE FROM Produtos WHERE EstabelecimentoId IN (2, 3)");
            migrationBuilder.Sql("DELETE FROM Produtos WHERE EstabelecimentoId IN (4, 5)");
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}