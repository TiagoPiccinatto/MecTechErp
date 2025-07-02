USE MecTecERP;
GO

PRINT 'Criando índices...';

-- Clientes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clientes_CpfCnpj' AND object_id = OBJECT_ID('Clientes'))
    CREATE INDEX IX_Clientes_CpfCnpj ON Clientes(CpfCnpj) WHERE CpfCnpj IS NOT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clientes_NomeRazaoSocial' AND object_id = OBJECT_ID('Clientes'))
    CREATE INDEX IX_Clientes_NomeRazaoSocial ON Clientes(NomeRazaoSocial);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clientes_Email' AND object_id = OBJECT_ID('Clientes'))
    CREATE INDEX IX_Clientes_Email ON Clientes(Email) WHERE Email IS NOT NULL;
GO

-- Veiculos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Veiculos_Placa' AND object_id = OBJECT_ID('Veiculos'))
    CREATE UNIQUE INDEX IX_Veiculos_Placa ON Veiculos(Placa); -- Placa deve ser única
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Veiculos_ClienteId' AND object_id = OBJECT_ID('Veiculos'))
    CREATE INDEX IX_Veiculos_ClienteId ON Veiculos(ClienteId);
GO

-- Produtos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_Codigo' AND object_id = OBJECT_ID('Produtos'))
    CREATE UNIQUE INDEX IX_Produtos_Codigo ON Produtos(Codigo); -- Código do produto deve ser único
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_Nome' AND object_id = OBJECT_ID('Produtos'))
    CREATE INDEX IX_Produtos_Nome ON Produtos(Nome);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_CategoriaId' AND object_id = OBJECT_ID('Produtos'))
    CREATE INDEX IX_Produtos_CategoriaId ON Produtos(CategoriaId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Produtos_FornecedorId' AND object_id = OBJECT_ID('Produtos'))
    CREATE INDEX IX_Produtos_FornecedorId ON Produtos(FornecedorId) WHERE FornecedorId IS NOT NULL;
GO

-- Fornecedores
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Fornecedores_Cnpj' AND object_id = OBJECT_ID('Fornecedores'))
    CREATE INDEX IX_Fornecedores_Cnpj ON Fornecedores(Cnpj) WHERE Cnpj IS NOT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Fornecedores_RazaoSocial' AND object_id = OBJECT_ID('Fornecedores'))
    CREATE INDEX IX_Fornecedores_RazaoSocial ON Fornecedores(RazaoSocial);
GO

-- OrdensServico
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServico_Numero' AND object_id = OBJECT_ID('OrdensServico'))
    CREATE UNIQUE INDEX IX_OrdensServico_Numero ON OrdensServico(Numero); -- Número da OS deve ser único
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServico_ClienteId' AND object_id = OBJECT_ID('OrdensServico'))
    CREATE INDEX IX_OrdensServico_ClienteId ON OrdensServico(ClienteId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServico_VeiculoId' AND object_id = OBJECT_ID('OrdensServico'))
    CREATE INDEX IX_OrdensServico_VeiculoId ON OrdensServico(VeiculoId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServico_DataEntrada' AND object_id = OBJECT_ID('OrdensServico'))
    CREATE INDEX IX_OrdensServico_DataEntrada ON OrdensServico(DataEntrada);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServico_Status' AND object_id = OBJECT_ID('OrdensServico'))
    CREATE INDEX IX_OrdensServico_Status ON OrdensServico(Status);
GO

-- OrdensServicoItens
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServicoItens_OrdemServicoId' AND object_id = OBJECT_ID('OrdensServicoItens'))
    CREATE INDEX IX_OrdensServicoItens_OrdemServicoId ON OrdensServicoItens(OrdemServicoId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServicoItens_ProdutoId' AND object_id = OBJECT_ID('OrdensServicoItens'))
    CREATE INDEX IX_OrdensServicoItens_ProdutoId ON OrdensServicoItens(ProdutoId) WHERE ProdutoId IS NOT NULL;
GO

-- OrdensServicoFotos
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrdensServicoFotos_OrdemServicoId' AND object_id = OBJECT_ID('OrdensServicoFotos'))
    CREATE INDEX IX_OrdensServicoFotos_OrdemServicoId ON OrdensServicoFotos(OrdemServicoId);
GO

-- MovimentacoesEstoque
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_ProdutoId' AND object_id = OBJECT_ID('MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_ProdutoId ON MovimentacoesEstoque(ProdutoId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_DataMovimentacao' AND object_id = OBJECT_ID('MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_DataMovimentacao ON MovimentacoesEstoque(DataMovimentacao);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_Tipo' AND object_id = OBJECT_ID('MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_Tipo ON MovimentacoesEstoque(Tipo);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_OrdemServicoItemId' AND object_id = OBJECT_ID('MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_OrdemServicoItemId ON MovimentacoesEstoque(OrdemServicoItemId) WHERE OrdemServicoItemId IS NOT NULL;
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MovimentacoesEstoque_InventarioId' AND object_id = OBJECT_ID('MovimentacoesEstoque'))
    CREATE INDEX IX_MovimentacoesEstoque_InventarioId ON MovimentacoesEstoque(InventarioId) WHERE InventarioId IS NOT NULL;
GO

-- Inventarios
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventarios_DataInicio' AND object_id = OBJECT_ID('Inventarios'))
    CREATE INDEX IX_Inventarios_DataInicio ON Inventarios(DataInicio);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Inventarios_Status' AND object_id = OBJECT_ID('Inventarios'))
    CREATE INDEX IX_Inventarios_Status ON Inventarios(Status);
GO

-- InventarioItens
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventarioItens_InventarioId' AND object_id = OBJECT_ID('InventarioItens'))
    CREATE INDEX IX_InventarioItens_InventarioId ON InventarioItens(InventarioId);
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InventarioItens_ProdutoId' AND object_id = OBJECT_ID('InventarioItens'))
    CREATE INDEX IX_InventarioItens_ProdutoId ON InventarioItens(ProdutoId);
GO


PRINT 'Criação de índices concluída.';
GO
