# Implementação do Módulo de Estoque - MecTecERP

## Visão Geral

O módulo de Estoque é responsável pelo controle completo de peças, produtos e materiais utilizados na oficina. Inclui funcionalidades de cadastro, movimentação, controle de níveis mínimos, alertas de reposição e relatórios de estoque.

## 1. Estrutura do Banco de Dados

### 1.1 Tabela: Categorias
```sql
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Descricao NVARCHAR(500),
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT,
    
    CONSTRAINT UK_Categorias_Nome UNIQUE (Nome)
);
```

### 1.2 Tabela: Fornecedores
```sql
CREATE TABLE Fornecedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RazaoSocial NVARCHAR(200) NOT NULL,
    NomeFantasia NVARCHAR(200),
    CnpjCpf NVARCHAR(18) NOT NULL,
    InscricaoEstadual NVARCHAR(20),
    Email NVARCHAR(100),
    Telefone NVARCHAR(20),
    Celular NVARCHAR(20),
    Site NVARCHAR(100),
    
    -- Endereço
    Cep NVARCHAR(10),
    Logradouro NVARCHAR(200),
    Numero NVARCHAR(20),
    Complemento NVARCHAR(100),
    Bairro NVARCHAR(100),
    Cidade NVARCHAR(100),
    Uf NVARCHAR(2),
    
    -- Dados Bancários
    Banco NVARCHAR(100),
    Agencia NVARCHAR(20),
    Conta NVARCHAR(30),
    Pix NVARCHAR(100),
    
    Observacoes NVARCHAR(1000),
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT,
    
    CONSTRAINT UK_Fornecedores_CnpjCpf UNIQUE (CnpjCpf)
);
```

### 1.3 Tabela: Produtos
```sql
CREATE TABLE Produtos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL,
    CodigoBarras NVARCHAR(50),
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(1000),
    CategoriaId INT NOT NULL,
    FornecedorId INT,
    
    -- Preços e Custos
    CustoUnitario DECIMAL(18,4) NOT NULL DEFAULT 0,
    PrecoVenda DECIMAL(18,4) NOT NULL DEFAULT 0,
    MargemLucro DECIMAL(5,2) NOT NULL DEFAULT 0,
    
    -- Estoque
    EstoqueAtual DECIMAL(18,3) NOT NULL DEFAULT 0,
    EstoqueMinimo DECIMAL(18,3) NOT NULL DEFAULT 0,
    EstoqueMaximo DECIMAL(18,3) NOT NULL DEFAULT 0,
    PontoReposicao DECIMAL(18,3) NOT NULL DEFAULT 0,
    
    -- Unidades
    UnidadeMedida NVARCHAR(10) NOT NULL DEFAULT 'UN', -- UN, KG, LT, MT, etc
    
    -- Localização
    Localizacao NVARCHAR(100), -- Prateleira, gaveta, etc
    
    -- Características
    Peso DECIMAL(18,3),
    Dimensoes NVARCHAR(100),
    
    -- Controle
    ControlaEstoque BIT NOT NULL DEFAULT 1,
    PermiteVendaEstoqueNegativo BIT NOT NULL DEFAULT 0,
    
    -- NCM e Tributação
    Ncm NVARCHAR(10),
    Cest NVARCHAR(10),
    
    -- Imagem
    Foto NVARCHAR(500),
    
    Observacoes NVARCHAR(1000),
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT,
    
    CONSTRAINT FK_Produtos_Categoria FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    CONSTRAINT FK_Produtos_Fornecedor FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id),
    CONSTRAINT UK_Produtos_Codigo UNIQUE (Codigo),
    CONSTRAINT UK_Produtos_CodigoBarras UNIQUE (CodigoBarras)
);
```

### 1.4 Tabela: MovimentacaoEstoque
```sql
CREATE TABLE MovimentacaoEstoque (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProdutoId INT NOT NULL,
    TipoMovimentacao TINYINT NOT NULL, -- 1=Entrada, 2=Saida, 3=Ajuste, 4=Transferencia
    Quantidade DECIMAL(18,3) NOT NULL,
    QuantidadeAnterior DECIMAL(18,3) NOT NULL,
    QuantidadeAtual DECIMAL(18,3) NOT NULL,
    CustoUnitario DECIMAL(18,4),
    ValorTotal DECIMAL(18,4),
    
    -- Referências
    DocumentoTipo NVARCHAR(20), -- OS, NF, AJUSTE, TRANSFERENCIA
    DocumentoNumero NVARCHAR(50),
    DocumentoId INT,
    
    -- Fornecedor (para entradas)
    FornecedorId INT,
    
    Observacoes NVARCHAR(500),
    DataMovimentacao DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioMovimentacao INT NOT NULL,
    
    CONSTRAINT FK_MovimentacaoEstoque_Produto FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id),
    CONSTRAINT FK_MovimentacaoEstoque_Fornecedor FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id)
);

CREATE INDEX IX_MovimentacaoEstoque_Produto_Data ON MovimentacaoEstoque(ProdutoId, DataMovimentacao);
CREATE INDEX IX_MovimentacaoEstoque_Data ON MovimentacaoEstoque(DataMovimentacao);
CREATE INDEX IX_MovimentacaoEstoque_Documento ON MovimentacaoEstoque(DocumentoTipo, DocumentoId);
```

### 1.5 Tabela: InventarioEstoque
```sql
CREATE TABLE InventarioEstoque (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Numero NVARCHAR(20) NOT NULL,
    Descricao NVARCHAR(200) NOT NULL,
    DataInicio DATETIME2 NOT NULL,
    DataFim DATETIME2,
    Status TINYINT NOT NULL DEFAULT 1, -- 1=Aberto, 2=Finalizado, 3=Cancelado
    TotalItens INT NOT NULL DEFAULT 0,
    TotalDivergencias INT NOT NULL DEFAULT 0,
    ValorDivergencia DECIMAL(18,4) NOT NULL DEFAULT 0,
    Observacoes NVARCHAR(1000),
    UsuarioAbertura INT NOT NULL,
    UsuarioFechamento INT,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT UK_InventarioEstoque_Numero UNIQUE (Numero)
);
```

### 1.6 Tabela: InventarioItens
```sql
CREATE TABLE InventarioItens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventarioId INT NOT NULL,
    ProdutoId INT NOT NULL,
    EstoqueSistema DECIMAL(18,3) NOT NULL,
    EstoqueContado DECIMAL(18,3),
    Divergencia DECIMAL(18,3) NOT NULL DEFAULT 0,
    CustoUnitario DECIMAL(18,4) NOT NULL,
    ValorDivergencia DECIMAL(18,4) NOT NULL DEFAULT 0,
    Observacoes NVARCHAR(500),
    DataContagem DATETIME2,
    UsuarioContagem INT,
    
    CONSTRAINT FK_InventarioItens_Inventario FOREIGN KEY (InventarioId) REFERENCES InventarioEstoque(Id),
    CONSTRAINT FK_InventarioItens_Produto FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id),
    CONSTRAINT UK_InventarioItens_Inventario_Produto UNIQUE (InventarioId, ProdutoId)
);
```

## 2. Entidades do Domínio

### 2.1 Categoria.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; }
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int UsuarioCadastro { get; set; }
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Navigation Properties
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
```

### 2.2 Fornecedor.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class Fornecedor
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Razão Social é obrigatória")]
        [StringLength(200, ErrorMessage = "Razão Social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; }
        
        [StringLength(200, ErrorMessage = "Nome Fantasia deve ter no máximo 200 caracteres")]
        public string NomeFantasia { get; set; }
        
        [Required(ErrorMessage = "CNPJ/CPF é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ/CPF deve ter no máximo 18 caracteres")]
        public string CnpjCpf { get; set; }
        
        [StringLength(20, ErrorMessage = "Inscrição Estadual deve ter no máximo 20 caracteres")]
        public string InscricaoEstadual { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; }
        
        [StringLength(20, ErrorMessage = "Celular deve ter no máximo 20 caracteres")]
        public string Celular { get; set; }
        
        [StringLength(100, ErrorMessage = "Site deve ter no máximo 100 caracteres")]
        public string Site { get; set; }
        
        // Endereço
        [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")]
        public string Cep { get; set; }
        
        [StringLength(200, ErrorMessage = "Logradouro deve ter no máximo 200 caracteres")]
        public string Logradouro { get; set; }
        
        [StringLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres")]
        public string Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; }
        
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }
        
        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string Uf { get; set; }
        
        // Dados Bancários
        [StringLength(100, ErrorMessage = "Banco deve ter no máximo 100 caracteres")]
        public string Banco { get; set; }
        
        [StringLength(20, ErrorMessage = "Agência deve ter no máximo 20 caracteres")]
        public string Agencia { get; set; }
        
        [StringLength(30, ErrorMessage = "Conta deve ter no máximo 30 caracteres")]
        public string Conta { get; set; }
        
        [StringLength(100, ErrorMessage = "PIX deve ter no máximo 100 caracteres")]
        public string Pix { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
        public string Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int UsuarioCadastro { get; set; }
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Navigation Properties
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
        public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
        
        // Computed Properties
        public string NomeExibicao => !string.IsNullOrEmpty(NomeFantasia) ? NomeFantasia : RazaoSocial;
        public string EnderecoCompleto => $"{Logradouro}, {Numero} - {Bairro} - {Cidade}/{Uf}";
    }
}
```

### 2.3 Produto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
        public string Codigo { get; set; }
        
        [StringLength(50, ErrorMessage = "Código de Barras deve ter no máximo 50 caracteres")]
        public string CodigoBarras { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }
        
        public int? FornecedorId { get; set; }
        
        // Preços e Custos
        [Range(0, double.MaxValue, ErrorMessage = "Custo Unitário deve ser maior ou igual a zero")]
        public decimal CustoUnitario { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Preço de Venda deve ser maior ou igual a zero")]
        public decimal PrecoVenda { get; set; }
        
        [Range(0, 100, ErrorMessage = "Margem de Lucro deve estar entre 0 e 100")]
        public decimal MargemLucro { get; set; }
        
        // Estoque
        public decimal EstoqueAtual { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque Mínimo deve ser maior ou igual a zero")]
        public decimal EstoqueMinimo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque Máximo deve ser maior ou igual a zero")]
        public decimal EstoqueMaximo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Ponto de Reposição deve ser maior ou igual a zero")]
        public decimal PontoReposicao { get; set; }
        
        // Unidades
        [Required(ErrorMessage = "Unidade de Medida é obrigatória")]
        [StringLength(10, ErrorMessage = "Unidade de Medida deve ter no máximo 10 caracteres")]
        public string UnidadeMedida { get; set; } = "UN";
        
        // Localização
        [StringLength(100, ErrorMessage = "Localização deve ter no máximo 100 caracteres")]
        public string Localizacao { get; set; }
        
        // Características
        public decimal? Peso { get; set; }
        
        [StringLength(100, ErrorMessage = "Dimensões deve ter no máximo 100 caracteres")]
        public string Dimensoes { get; set; }
        
        // Controle
        public bool ControlaEstoque { get; set; } = true;
        public bool PermiteVendaEstoqueNegativo { get; set; } = false;
        
        // NCM e Tributação
        [StringLength(10, ErrorMessage = "NCM deve ter no máximo 10 caracteres")]
        public string Ncm { get; set; }
        
        [StringLength(10, ErrorMessage = "CEST deve ter no máximo 10 caracteres")]
        public string Cest { get; set; }
        
        // Imagem
        [StringLength(500, ErrorMessage = "Foto deve ter no máximo 500 caracteres")]
        public string Foto { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
        public string Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int UsuarioCadastro { get; set; }
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Navigation Properties
        public virtual Categoria Categoria { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; } = new List<MovimentacaoEstoque>();
        public virtual ICollection<InventarioItem> InventarioItens { get; set; } = new List<InventarioItem>();
        
        // Computed Properties
        public bool EstoqueBaixo => EstoqueAtual <= EstoqueMinimo;
        public bool EstoqueCritico => EstoqueAtual <= PontoReposicao;
        public decimal ValorEstoque => EstoqueAtual * CustoUnitario;
        public string StatusEstoque
        {
            get
            {
                if (EstoqueAtual <= 0) return "Sem Estoque";
                if (EstoqueCritico) return "Crítico";
                if (EstoqueBaixo) return "Baixo";
                return "Normal";
            }
        }
    }
}
```

### 2.4 MovimentacaoEstoque.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Produto é obrigatório")]
        public int ProdutoId { get; set; }
        
        [Required(ErrorMessage = "Tipo de Movimentação é obrigatório")]
        public TipoMovimentacao TipoMovimentacao { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        public decimal Quantidade { get; set; }
        
        public decimal QuantidadeAnterior { get; set; }
        public decimal QuantidadeAtual { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Custo Unitário deve ser maior ou igual a zero")]
        public decimal? CustoUnitario { get; set; }
        
        public decimal? ValorTotal { get; set; }
        
        // Referências
        [StringLength(20, ErrorMessage = "Tipo do Documento deve ter no máximo 20 caracteres")]
        public string DocumentoTipo { get; set; }
        
        [StringLength(50, ErrorMessage = "Número do Documento deve ter no máximo 50 caracteres")]
        public string DocumentoNumero { get; set; }
        
        public int? DocumentoId { get; set; }
        
        // Fornecedor (para entradas)
        public int? FornecedorId { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string Observacoes { get; set; }
        
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Usuário da Movimentação é obrigatório")]
        public int UsuarioMovimentacao { get; set; }
        
        // Navigation Properties
        public virtual Produto Produto { get; set; }
        public virtual Fornecedor Fornecedor { get; set; }
        
        // Computed Properties
        public string TipoMovimentacaoDescricao => TipoMovimentacao.GetDescription();
        public string QuantidadeFormatada => $"{Quantidade:N3} {Produto?.UnidadeMedida}";
        public decimal? ValorTotalCalculado => CustoUnitario.HasValue ? Quantidade * CustoUnitario.Value : null;
    }
    
    public enum TipoMovimentacao
    {
        [Display(Name = "Entrada")]
        Entrada = 1,
        
        [Display(Name = "Saída")]
        Saida = 2,
        
        [Display(Name = "Ajuste")]
        Ajuste = 3,
        
        [Display(Name = "Transferência")]
        Transferencia = 4
    }
}
```

### 2.5 InventarioEstoque.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class InventarioEstoque
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres")]
        public string Numero { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "Descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "Data de Início é obrigatória")]
        public DateTime DataInicio { get; set; }
        
        public DateTime? DataFim { get; set; }
        
        [Required(ErrorMessage = "Status é obrigatório")]
        public StatusInventario Status { get; set; } = StatusInventario.Aberto;
        
        public int TotalItens { get; set; }
        public int TotalDivergencias { get; set; }
        public decimal ValorDivergencia { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
        public string Observacoes { get; set; }
        
        [Required(ErrorMessage = "Usuário de Abertura é obrigatório")]
        public int UsuarioAbertura { get; set; }
        
        public int? UsuarioFechamento { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        // Navigation Properties
        public virtual ICollection<InventarioItem> Itens { get; set; } = new List<InventarioItem>();
        
        // Computed Properties
        public string StatusDescricao => Status.GetDescription();
        public bool PodeEditar => Status == StatusInventario.Aberto;
        public bool PodeFinalizar => Status == StatusInventario.Aberto && TotalItens > 0;
        public TimeSpan? TempoInventario => DataFim.HasValue ? DataFim.Value - DataInicio : DateTime.Now - DataInicio;
    }
    
    public enum StatusInventario
    {
        [Display(Name = "Aberto")]
        Aberto = 1,
        
        [Display(Name = "Finalizado")]
        Finalizado = 2,
        
        [Display(Name = "Cancelado")]
        Cancelado = 3
    }
}
```

### 2.6 InventarioItem.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class InventarioItem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Inventário é obrigatório")]
        public int InventarioId { get; set; }
        
        [Required(ErrorMessage = "Produto é obrigatório")]
        public int ProdutoId { get; set; }
        
        public decimal EstoqueSistema { get; set; }
        public decimal? EstoqueContado { get; set; }
        public decimal Divergencia { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Custo Unitário deve ser maior ou igual a zero")]
        public decimal CustoUnitario { get; set; }
        
        public decimal ValorDivergencia { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
        public string Observacoes { get; set; }
        
        public DateTime? DataContagem { get; set; }
        public int? UsuarioContagem { get; set; }
        
        // Navigation Properties
        public virtual InventarioEstoque Inventario { get; set; }
        public virtual Produto Produto { get; set; }
        
        // Computed Properties
        public bool TemDivergencia => Divergencia != 0;
        public bool Contado => EstoqueContado.HasValue;
        public string StatusContagem => Contado ? "Contado" : "Pendente";
        public decimal PercentualDivergencia
        {
            get
            {
                if (EstoqueSistema == 0) return 0;
                return Math.Abs(Divergencia / EstoqueSistema) * 100;
            }
        }
    }
}
```

## 3. Enums e Helpers

### 3.1 UnidadeMedida.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Enums
{
    public enum UnidadeMedida
    {
        [Display(Name = "Unidade")]
        UN,
        
        [Display(Name = "Quilograma")]
        KG,
        
        [Display(Name = "Grama")]
        GR,
        
        [Display(Name = "Litro")]
        LT,
        
        [Display(Name = "Mililitro")]
        ML,
        
        [Display(Name = "Metro")]
        MT,
        
        [Display(Name = "Centímetro")]
        CM,
        
        [Display(Name = "Milímetro")]
        MM,
        
        [Display(Name = "Metro Quadrado")]
        M2,
        
        [Display(Name = "Metro Cúbico")]
        M3,
        
        [Display(Name = "Caixa")]
        CX,
        
        [Display(Name = "Pacote")]
        PC,
        
        [Display(Name = "Peça")]
        PCA,
        
        [Display(Name = "Par")]
        PAR,
        
        [Display(Name = "Jogo")]
        JG,
        
        [Display(Name = "Kit")]
        KIT
    }
}
```

### 3.2 EstoqueHelper.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Helpers
{
    public static class EstoqueHelper
    {
        public static List<string> GetUnidadesMedida()
        {
            return Enum.GetValues<UnidadeMedida>()
                      .Select(u => u.ToString())
                      .ToList();
        }
        
        public static string GetStatusEstoqueClass(decimal estoqueAtual, decimal estoqueMinimo, decimal pontoReposicao)
        {
            if (estoqueAtual <= 0) return "danger";
            if (estoqueAtual <= pontoReposicao) return "warning";
            if (estoqueAtual <= estoqueMinimo) return "info";
            return "success";
        }
        
        public static string GetStatusEstoqueIcon(decimal estoqueAtual, decimal estoqueMinimo, decimal pontoReposicao)
        {
            if (estoqueAtual <= 0) return "fas fa-times-circle";
            if (estoqueAtual <= pontoReposicao) return "fas fa-exclamation-triangle";
            if (estoqueAtual <= estoqueMinimo) return "fas fa-info-circle";
            return "fas fa-check-circle";
        }
        
        public static string GetTipoMovimentacaoClass(TipoMovimentacao tipo)
        {
            return tipo switch
            {
                TipoMovimentacao.Entrada => "success",
                TipoMovimentacao.Saida => "danger",
                TipoMovimentacao.Ajuste => "warning",
                TipoMovimentacao.Transferencia => "info",
                _ => "secondary"
            };
        }
        
        public static string GetTipoMovimentacaoIcon(TipoMovimentacao tipo)
        {
            return tipo switch
            {
                TipoMovimentacao.Entrada => "fas fa-arrow-up",
                TipoMovimentacao.Saida => "fas fa-arrow-down",
                TipoMovimentacao.Ajuste => "fas fa-edit",
                TipoMovimentacao.Transferencia => "fas fa-exchange-alt",
                _ => "fas fa-question"
            };
        }
        
        public static string GerarCodigoProduto(string prefixo = "PROD")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100, 999);
            return $"{prefixo}{timestamp}{random}";
        }
        
        public static string GerarNumeroInventario()
        {
            var ano = DateTime.Now.Year;
            var mes = DateTime.Now.Month.ToString("D2");
            var timestamp = DateTime.Now.ToString("ddHHmmss");
            return $"INV{ano}{mes}{timestamp}";
        }
        
        public static decimal CalcularMargemLucro(decimal custoUnitario, decimal precoVenda)
        {
            if (custoUnitario <= 0) return 0;
            return ((precoVenda - custoUnitario) / custoUnitario) * 100;
        }
        
        public static decimal CalcularPrecoVenda(decimal custoUnitario, decimal margemLucro)
        {
            return custoUnitario * (1 + (margemLucro / 100));
        }
        
        public static bool ValidarCodigoBarras(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) return true; // Opcional
            
            // Remove espaços e caracteres especiais
            codigoBarras = codigoBarras.Replace(" ", "").Replace("-", "");
            
            // Verifica se contém apenas números
            if (!codigoBarras.All(char.IsDigit)) return false;
            
            // Verifica tamanhos válidos (EAN-8, EAN-13, UPC-A, etc.)
            var tamanhosValidos = new[] { 8, 12, 13, 14 };
            return tamanhosValidos.Contains(codigoBarras.Length);
        }
        
        public static List<Produto> GetProdutosBaixoEstoque(IEnumerable<Produto> produtos)
        {
            return produtos.Where(p => p.ControlaEstoque && p.EstoqueBaixo && p.Ativo)
                          .OrderBy(p => p.EstoqueAtual)
                          .ToList();
        }
        
        public static List<Produto> GetProdutosCriticos(IEnumerable<Produto> produtos)
        {
            return produtos.Where(p => p.ControlaEstoque && p.EstoqueCritico && p.Ativo)
                          .OrderBy(p => p.EstoqueAtual)
                          .ToList();
        }
        
        public static List<Produto> GetProdutosSemEstoque(IEnumerable<Produto> produtos)
        {
            return produtos.Where(p => p.ControlaEstoque && p.EstoqueAtual <= 0 && p.Ativo)
                          .OrderBy(p => p.Nome)
                          .ToList();
        }
    }
}
```

## 4. DTOs e ViewModels

### 4.1 CategoriaDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int TotalProdutos { get; set; }
    }
    
    public class CategoriaCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
    
    public class CategoriaUpdateDto : CategoriaCreateDto
    {
        public int Id { get; set; }
    }
    
    public class CategoriaListDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public int TotalProdutos { get; set; }
        public string StatusClass => Ativo ? "success" : "secondary";
        public string StatusText => Ativo ? "Ativo" : "Inativo";
    }
}
```

### 4.2 FornecedorDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs
{
    public class FornecedorDto
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CnpjCpf { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string Site { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string Pix { get; set; }
        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        public int TotalProdutos { get; set; }
        
        public string NomeExibicao => !string.IsNullOrEmpty(NomeFantasia) ? NomeFantasia : RazaoSocial;
        public string EnderecoCompleto => $"{Logradouro}, {Numero} - {Bairro} - {Cidade}/{Uf}";
    }
    
    public class FornecedorCreateDto
    {
        [Required(ErrorMessage = "Razão Social é obrigatória")]
        [StringLength(200, ErrorMessage = "Razão Social deve ter no máximo 200 caracteres")]
        public string RazaoSocial { get; set; }
        
        [StringLength(200, ErrorMessage = "Nome Fantasia deve ter no máximo 200 caracteres")]
        public string NomeFantasia { get; set; }
        
        [Required(ErrorMessage = "CNPJ/CPF é obrigatório")]
        [StringLength(18, ErrorMessage = "CNPJ/CPF deve ter no máximo 18 caracteres")]
        public string CnpjCpf { get; set; }
        
        [StringLength(20, ErrorMessage = "Inscrição Estadual deve ter no máximo 20 caracteres")]
        public string InscricaoEstadual { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; }
        
        [StringLength(20, ErrorMessage = "Celular deve ter no máximo 20 caracteres")]
        public string Celular { get; set; }
        
        [StringLength(100, ErrorMessage = "Site deve ter no máximo 100 caracteres")]
        public string Site { get; set; }
        
        [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")]
        public string Cep { get; set; }
        
        [StringLength(200, ErrorMessage = "Logradouro deve ter no máximo 200 caracteres")]
        public string Logradouro { get; set; }
        
        [StringLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres")]
        public string Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string Complemento { get; set; }
        
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Bairro { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }
        
        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string Uf { get; set; }
        
        [StringLength(100, ErrorMessage = "Banco deve ter no máximo 100 caracteres")]
        public string Banco { get; set; }
        
        [StringLength(20, ErrorMessage = "Agência deve ter no máximo 20 caracteres")]
        public string Agencia { get; set; }
        
        [StringLength(30, ErrorMessage = "Conta deve ter no máximo 30 caracteres")]
        public string Conta { get; set; }
        
        [StringLength(100, ErrorMessage = "PIX deve ter no máximo 100 caracteres")]
        public string Pix { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
        public string Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
    
    public class FornecedorUpdateDto : FornecedorCreateDto
    {
        public int Id { get; set; }
    }
    
    public class FornecedorListDto
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string CnpjCpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public bool Ativo { get; set; }
        public int TotalProdutos { get; set; }
        
        public string NomeExibicao => !string.IsNullOrEmpty(NomeFantasia) ? NomeFantasia : RazaoSocial;
        public string StatusClass => Ativo ? "success" : "secondary";
        public string StatusText => Ativo ? "Ativo" : "Inativo";
    }
    
    public class FornecedorSearchDto
    {
        public string Nome { get; set; }
        public string CnpjCpf { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public bool? Ativo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "RazaoSocial";
        public bool OrderDesc { get; set; } = false;
    }
}
```

### 4.3 ProdutoDto.cs
```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string CodigoBarras { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; }
        public int? FornecedorId { get; set; }
        public string FornecedorNome { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal MargemLucro { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }
        public decimal PontoReposicao { get; set; }
        public string UnidadeMedida { get; set; }
        public string Localizacao { get; set; }
        public decimal? Peso { get; set; }
        public string Dimensoes { get; set; }
        public bool ControlaEstoque { get; set; }
        public bool PermiteVendaEstoqueNegativo { get; set; }
        public string Ncm { get; set; }
        public string Cest { get; set; }
        public string Foto { get; set; }
        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        public bool EstoqueBaixo => EstoqueAtual <= EstoqueMinimo;
        public bool EstoqueCritico => EstoqueAtual <= PontoReposicao;
        public decimal ValorEstoque => EstoqueAtual * CustoUnitario;
        public string StatusEstoque
        {
            get
            {
                if (EstoqueAtual <= 0) return "Sem Estoque";
                if (EstoqueCritico) return "Crítico";
                if (EstoqueBaixo) return "Baixo";
                return "Normal";
            }
        }
    }
    
    public class ProdutoCreateDto
    {
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
        public string Codigo { get; set; }
        
        [StringLength(50, ErrorMessage = "Código de Barras deve ter no máximo 50 caracteres")]
        public string CodigoBarras { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; }
        
        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }
        
        public int? FornecedorId { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Custo Unitário deve ser maior ou igual a zero")]
        public decimal CustoUnitario { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Preço de Venda deve ser maior ou igual a zero")]
        public decimal PrecoVenda { get; set; }
        
        [Range(0, 100, ErrorMessage = "Margem de Lucro deve estar entre 0 e 100")]
        public decimal MargemLucro { get; set; }
        
        public decimal EstoqueAtual { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque Mínimo deve ser maior ou igual a zero")]
        public decimal EstoqueMinimo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Estoque Máximo deve ser maior ou igual a zero")]
        public decimal EstoqueMaximo { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Ponto de Reposição deve ser maior ou igual a zero")]
        public decimal PontoReposicao { get; set; }
        
        [Required(ErrorMessage = "Unidade de Medida é obrigatória")]
        [StringLength(10, ErrorMessage = "Unidade de Medida deve ter no máximo 10 caracteres")]
        public string UnidadeMedida { get; set; } = "UN";
        
        [StringLength(100, ErrorMessage = "Localização deve ter no máximo 100 caracteres")]
        public string Localizacao { get; set; }
        
        public decimal? Peso { get; set; }
        
        [StringLength(100, ErrorMessage = "Dimensões deve ter no máximo 100 caracteres")]
        public string Dimensoes { get; set; }
        
        public bool ControlaEstoque { get; set; } = true;
        public bool PermiteVendaEstoqueNegativo { get; set; } = false;
        
        [StringLength(10, ErrorMessage = "NCM deve ter no máximo 10 caracteres")]
        public string Ncm { get; set; }
        
        [StringLength(10, ErrorMessage = "CEST deve ter no máximo 10 caracteres")]
        public string Cest { get; set; }
        
        [StringLength(500, ErrorMessage = "Foto deve ter no máximo 500 caracteres")]
        public string Foto { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
        public string Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
    }
    
    public class ProdutoUpdateDto : ProdutoCreateDto
    {
        public int Id { get; set; }
    }
    
    public class ProdutoListDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string CategoriaNome { get; set; }
        public string FornecedorNome { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal PrecoVenda { get; set; }
        public decimal EstoqueAtual { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public string UnidadeMedida { get; set; }
        public bool Ativo { get; set; }
        
        public bool EstoqueBaixo => EstoqueAtual <= EstoqueMinimo;
        public string StatusEstoque
        {
            get
            {
                if (EstoqueAtual <= 0) return "Sem Estoque";
                if (EstoqueBaixo) return "Baixo";
                return "Normal";
            }
        }
        public string StatusClass
        {
            get
            {
                if (EstoqueAtual <= 0) return "danger";
                if (EstoqueBaixo) return "warning";
                return "success";
            }
        }
        public string StatusText => Ativo ? "Ativo" : "Inativo";
    }
    
    public class ProdutoSearchDto
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string CodigoBarras { get; set; }
        public int? CategoriaId { get; set; }
        public int? FornecedorId { get; set; }
        public bool? Ativo { get; set; }
        public bool? EstoqueBaixo { get; set; }
        public bool? SemEstoque { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "Nome";
        public bool OrderDesc { get; set; } = false;
    }
}
```

## Próximos Passos

Este documento apresenta a estrutura base do módulo de Estoque. Os próximos passos incluem:

1. **Implementação dos Repositórios** (IEstoqueRepository, IProdutoRepository, etc.)
2. **Implementação dos Serviços** (IEstoqueService, IProdutoService, etc.)
3. **Criação das páginas Blazor** (listagem, cadastro, edição)
4. **Implementação dos componentes** (formulários, tabelas, modais)
5. **Criação dos relatórios** de estoque
6. **Implementação dos alertas** de reposição
7. **Testes unitários** e de integração

O módulo de Estoque é fundamental para o controle eficiente da oficina e deve ser implementado com foco na usabilidade e performance.