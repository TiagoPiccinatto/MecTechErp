USE MecTecERP;
GO

-- Tabelas base da BaseEntity (campos comuns)
-- Id INT IDENTITY(1,1) PRIMARY KEY
-- DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE()
-- DataAtualizacao DATETIME2 NULL
-- Ativo BIT NOT NULL DEFAULT 1
-- UsuarioCriacao NVARCHAR(100) NULL
-- UsuarioAtualizacao NVARCHAR(100) NULL

PRINT 'Criando tabela Categorias...';
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(500) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL
);
GO

PRINT 'Criando tabela Fornecedores...';
CREATE TABLE Fornecedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RazaoSocial NVARCHAR(200) NOT NULL,
    NomeFantasia NVARCHAR(200) NULL,
    Cnpj NVARCHAR(18) NULL, -- Formato XX.XXX.XXX/XXXX-XX
    InscricaoEstadual NVARCHAR(20) NULL,
    Telefone1 NVARCHAR(20) NULL,
    Telefone2 NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Cep NVARCHAR(10) NULL, -- Formato XXXXX-XXX
    Logradouro NVARCHAR(200) NULL,
    Numero NVARCHAR(20) NULL,
    Complemento NVARCHAR(100) NULL,
    Bairro NVARCHAR(100) NULL,
    Cidade NVARCHAR(100) NULL,
    Uf NCHAR(2) NULL, -- Sigla do estado
    NomeContato NVARCHAR(100) NULL,
    TelefoneContato NVARCHAR(20) NULL,
    EmailContato NVARCHAR(100) NULL,
    Observacoes NVARCHAR(1000) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL
);
GO

PRINT 'Criando tabela Clientes...';
CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TipoPessoa INT NOT NULL, -- Enum: 1 para PF, 2 para PJ
    NomeRazaoSocial NVARCHAR(200) NOT NULL,
    CpfCnpj NVARCHAR(18) NOT NULL,
    RgIe NVARCHAR(20) NULL,
    Telefone1 NVARCHAR(20) NULL,
    Telefone2 NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Cep NVARCHAR(10) NULL,
    Logradouro NVARCHAR(200) NULL,
    Numero NVARCHAR(20) NULL,
    Complemento NVARCHAR(100) NULL,
    Bairro NVARCHAR(100) NULL,
    Cidade NVARCHAR(100) NULL,
    Uf NCHAR(2) NULL,
    Observacoes NVARCHAR(1000) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL
);
GO

PRINT 'Criando tabela Produtos...';
CREATE TABLE Produtos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(1000) NULL,
    CodigoBarras NVARCHAR(50) NULL,
    CategoriaId INT NOT NULL,
    FornecedorId INT NULL,
    Unidade INT NOT NULL, -- Enum UnidadeMedida
    PrecoCusto DECIMAL(18,2) NOT NULL DEFAULT 0,
    PrecoVenda DECIMAL(18,2) NOT NULL DEFAULT 0,
    EstoqueAtual DECIMAL(18,3) NOT NULL DEFAULT 0, -- Nome da entidade é EstoqueAtual
    EstoqueMinimo DECIMAL(18,3) NULL,
    EstoqueMaximo DECIMAL(18,3) NULL,
    Localizacao NVARCHAR(100) NULL,
    Observacoes NVARCHAR(1000) NULL,
    FotoUrl NVARCHAR(500) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_Produtos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    CONSTRAINT FK_Produtos_Fornecedores FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id)
);
GO

PRINT 'Criando tabela Veiculos...';
CREATE TABLE Veiculos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Placa NVARCHAR(8) NOT NULL, -- Formato antigo e Mercosul
    Marca NVARCHAR(50) NOT NULL,
    Modelo NVARCHAR(100) NOT NULL,
    AnoFabricacao INT NOT NULL,
    AnoModelo INT NULL,
    Cor NVARCHAR(30) NOT NULL,
    Chassi NVARCHAR(17) NULL,
    KmAtual INT NULL,
    TipoCombustivel NVARCHAR(30) NULL,
    Renavam NVARCHAR(11) NULL,
    Foto NVARCHAR(500) NULL, -- URL ou caminho
    Observacoes NVARCHAR(1000) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_Veiculos_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);
GO

PRINT 'Criando tabela OrdensServico...';
CREATE TABLE OrdensServico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Numero NVARCHAR(20) NOT NULL,
    ClienteId INT NOT NULL,
    VeiculoId INT NOT NULL,
    DataEntrada DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataPrevisaoEntrega DATETIME2 NULL,
    DataConclusao DATETIME2 NULL,
    ProblemaRelatado NVARCHAR(MAX) NULL,
    DiagnosticoTecnico NVARCHAR(MAX) NULL,
    ValorServicos DECIMAL(18,2) NOT NULL DEFAULT 0,
    ValorPecas DECIMAL(18,2) NOT NULL DEFAULT 0,
    ValorDesconto DECIMAL(18,2) NOT NULL DEFAULT 0,
    ValorTotal DECIMAL(18,2) NOT NULL DEFAULT 0, -- (Servicos + Pecas - Desconto)
    Status INT NOT NULL, -- Enum StatusOrdemServico
    ObservacoesInternas NVARCHAR(MAX) NULL,
    ObservacoesCliente NVARCHAR(MAX) NULL,
    MecanicoResponsavelId INT NULL, -- FK para uma futura tabela de Funcionarios/Usuarios
    OrcamentoAprovado BIT NOT NULL DEFAULT 0,
    DataAprovacaoOrcamento DATETIME2 NULL,
    QuilometragemEntrada INT NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_OrdensServico_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_OrdensServico_Veiculos FOREIGN KEY (VeiculoId) REFERENCES Veiculos(Id)
    -- CONSTRAINT FK_OrdensServico_Funcionarios FOREIGN KEY (MecanicoResponsavelId) REFERENCES Funcionarios(Id) -- Se existir
);
GO

PRINT 'Criando tabela OrdensServicoItens...';
CREATE TABLE OrdensServicoItens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdemServicoId INT NOT NULL,
    Tipo INT NOT NULL, -- Enum TipoItemOrdemServico (Servico = 1, Peca = 2)
    ProdutoId INT NULL, -- FK para Produtos se Tipo for Peca
    Descricao NVARCHAR(200) NOT NULL, -- Descrição do serviço ou observação da peça
    Quantidade DECIMAL(18,3) NOT NULL,
    ValorUnitario DECIMAL(18,2) NOT NULL,
    ValorTotal DECIMAL(18,2) NOT NULL, -- Calculado: Quantidade * ValorUnitario
    DescontoItem DECIMAL(18,2) NULL DEFAULT 0,
    ObservacoesItem NVARCHAR(500) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_OrdensServicoItens_OrdensServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdensServico(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrdensServicoItens_Produtos FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);
GO

PRINT 'Criando tabela OrdensServicoFotos...';
CREATE TABLE OrdensServicoFotos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdemServicoId INT NOT NULL,
    UrlFoto NVARCHAR(500) NOT NULL,
    Descricao NVARCHAR(200) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_OrdensServicoFotos_OrdensServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdensServico(Id) ON DELETE CASCADE
);
GO

PRINT 'Criando tabela Inventarios...';
CREATE TABLE Inventarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descricao NVARCHAR(200) NOT NULL,
    DataInicio DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataFinalizacao DATETIME2 NULL,
    Status INT NOT NULL, -- Enum StatusInventario (Planejado, EmAndamento, Finalizado, Cancelado)
    Observacoes NVARCHAR(1000) NULL,

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL, -- Responsável pela criação
    UsuarioAtualizacao NVARCHAR(100) NULL
);
GO

PRINT 'Criando tabela InventarioItens...';
CREATE TABLE InventarioItens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventarioId INT NOT NULL,
    ProdutoId INT NOT NULL,
    EstoqueSistema DECIMAL(18,3) NOT NULL, -- Saldo do sistema no momento da abertura do item no inventário
    EstoqueContado DECIMAL(18,3) NOT NULL,
    -- Diferenca é EstoqueContado - EstoqueSistema (calculado)
    Observacoes NVARCHAR(1000) NULL, -- Justificativa ou observação da contagem
    DataContagem DATETIME2 NULL, -- Momento da última contagem registrada para este item
    UsuarioContagem NVARCHAR(100) NULL, -- Quem realizou a contagem

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_InventarioItens_Inventarios FOREIGN KEY (InventarioId) REFERENCES Inventarios(Id) ON DELETE CASCADE,
    CONSTRAINT FK_InventarioItens_Produtos FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);
GO

PRINT 'Criando tabela MovimentacoesEstoque...';
CREATE TABLE MovimentacoesEstoque (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProdutoId INT NOT NULL,
    DataMovimentacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Tipo INT NOT NULL, -- Enum TipoMovimentacaoEstoque (Entrada, Saida, Ajuste, Inventario)
    Quantidade DECIMAL(18,3) NOT NULL,
    ValorUnitario DECIMAL(18,2) NULL, -- Pode ser nulo para alguns tipos de movimentação, como ajuste simples sem custo.
    EstoqueAnterior DECIMAL(18,3) NOT NULL,
    EstoquePosterior DECIMAL(18,3) NOT NULL,
    Observacoes NVARCHAR(1000) NULL,
    Documento NVARCHAR(100) NULL, -- Nota fiscal, requisição, etc.
    OrdemServicoItemId INT NULL,
    InventarioId INT NULL, -- Se a movimentação foi originada por um inventário (geralmente para o ajuste final)

    DataCriacao DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    DataAtualizacao DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1, -- Movimentações geralmente não são inativadas, mas estornadas.
    UsuarioCriacao NVARCHAR(100) NULL,
    UsuarioAtualizacao NVARCHAR(100) NULL,

    CONSTRAINT FK_MovimentacoesEstoque_Produtos FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id),
    CONSTRAINT FK_MovimentacoesEstoque_OrdensServicoItens FOREIGN KEY (OrdemServicoItemId) REFERENCES OrdensServicoItens(Id),
    CONSTRAINT FK_MovimentacoesEstoque_Inventarios FOREIGN KEY (InventarioId) REFERENCES Inventarios(Id)
);
GO

PRINT 'Criação de tabelas concluída.';
GO
