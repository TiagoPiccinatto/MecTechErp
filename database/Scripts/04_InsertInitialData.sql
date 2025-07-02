USE MecTecERP;
GO

PRINT 'Inserindo dados iniciais...';

-- Categorias Iniciais
IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Serviços de Motor')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Serviços de Motor', 'Serviços relacionados a reparos e manutenção de motores', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Suspensão e Direção')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Suspensão e Direção', 'Serviços e peças para suspensão e sistema de direção', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Freios')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Freios', 'Serviços e peças para o sistema de freios', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Elétrica e Ignição')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Elétrica e Ignição', 'Serviços e componentes elétricos e de ignição', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Óleos e Fluidos')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Óleos e Fluidos', 'Óleos lubrificantes, fluidos de arrefecimento, freio, etc.', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Filtros')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Filtros', 'Filtros de óleo, ar, combustível, cabine', 1, 'SCRIPT_INICIAL');
END

IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nome = 'Pneus e Rodas')
BEGIN
    INSERT INTO Categorias (Nome, Descricao, Ativo, UsuarioCriacao) VALUES
    ('Pneus e Rodas', 'Pneus, rodas, alinhamento e balanceamento', 1, 'SCRIPT_INICIAL');
END

PRINT 'Categorias iniciais inseridas.';

-- Fornecedor Padrão (Exemplo)
IF NOT EXISTS (SELECT 1 FROM Fornecedores WHERE Cnpj = '00.000.000/0001-00')
BEGIN
    INSERT INTO Fornecedores (RazaoSocial, NomeFantasia, Cnpj, Email, Telefone1, Cep, Logradouro, Numero, Bairro, Cidade, Uf, Ativo, UsuarioCriacao)
    VALUES ('Fornecedor Padrão LTDA', 'Peças XYZ', '00.000.000/0001-00', 'contato@pecasxyz.com', '(11) 99999-8888', '01001-000', 'Rua das Peças', '123', 'Centro', 'São Paulo', 'SP', 1, 'SCRIPT_INICIAL');
END

PRINT 'Fornecedor padrão inserido.';

-- Produtos de Exemplo (Opcional, mas útil para testes)
-- Exemplo de um serviço
IF NOT EXISTS (SELECT 1 FROM Produtos WHERE Codigo = 'SERV001')
BEGIN
    DECLARE @CategoriaServicosId INT;
    SELECT @CategoriaServicosId = Id FROM Categorias WHERE Nome = 'Serviços de Motor';

    IF @CategoriaServicosId IS NOT NULL
    BEGIN
        INSERT INTO Produtos (Codigo, Nome, Descricao, CategoriaId, Unidade, PrecoVenda, Ativo, UsuarioCriacao, EstoqueAtual)
        VALUES ('SERV001', 'Troca de Óleo Motor', 'Serviço de troca de óleo do motor e filtro de óleo.', @CategoriaServicosId, 7, 150.00, 1, 'SCRIPT_INICIAL', 9999); -- Unidade 7 = Servico
    END
END

-- Exemplo de uma peça
IF NOT EXISTS (SELECT 1 FROM Produtos WHERE Codigo = 'PECA001')
BEGIN
    DECLARE @CategoriaFiltrosId INT;
    DECLARE @FornecedorPadraoId INT;
    SELECT @CategoriaFiltrosId = Id FROM Categorias WHERE Nome = 'Filtros';
    SELECT @FornecedorPadraoId = Id FROM Fornecedores WHERE Cnpj = '00.000.000/0001-00';

    IF @CategoriaFiltrosId IS NOT NULL AND @FornecedorPadraoId IS NOT NULL
    BEGIN
        INSERT INTO Produtos (Codigo, Nome, Descricao, CategoriaId, FornecedorId, Unidade, PrecoCusto, PrecoVenda, EstoqueAtual, EstoqueMinimo, Ativo, UsuarioCriacao)
        VALUES ('PECA001', 'Filtro de Óleo XPTO123', 'Filtro de óleo para motores XYZ.', @CategoriaFiltrosId, @FornecedorPadraoId, 1, 25.00, 45.00, 50, 10, 1, 'SCRIPT_INICIAL'); -- Unidade 1 = Unidade
    END
END

PRINT 'Produtos de exemplo inseridos.';

PRINT 'Inserção de dados iniciais concluída.';
GO
