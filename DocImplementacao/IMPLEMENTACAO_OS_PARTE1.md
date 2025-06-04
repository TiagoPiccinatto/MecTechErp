# Implementação do Módulo de Ordem de Serviço (OS) - Parte 1 - MecTecERP

## Visão Geral

O módulo de Ordem de Serviço é o coração do sistema MecTecERP, responsável por gerenciar todo o fluxo de trabalho da oficina mecânica, desde a entrada do veículo até a finalização do serviço.

## Estrutura de Dados

### Tabela: OrdemServico

```sql
CREATE TABLE OrdemServico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Numero NVARCHAR(20) NOT NULL UNIQUE,
    ClienteId INT NOT NULL,
    VeiculoId INT NOT NULL,
    DataEntrada DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataPrevista DATETIME2 NULL,
    DataSaida DATETIME2 NULL,
    KmEntrada INT NOT NULL,
    KmSaida INT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Aberta',
    Prioridade NVARCHAR(20) NOT NULL DEFAULT 'Normal',
    TipoServico NVARCHAR(50) NOT NULL,
    DefeitoRelatado NVARCHAR(MAX) NOT NULL,
    DefeitoConstatado NVARCHAR(MAX) NULL,
    ServicoExecutado NVARCHAR(MAX) NULL,
    ObservacoesInternas NVARCHAR(MAX) NULL,
    ObservacoesCliente NVARCHAR(MAX) NULL,
    ValorMaoObra DECIMAL(10,2) NOT NULL DEFAULT 0,
    ValorPecas DECIMAL(10,2) NOT NULL DEFAULT 0,
    ValorDesconto DECIMAL(10,2) NOT NULL DEFAULT 0,
    ValorTotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    FormaPagamento NVARCHAR(30) NULL,
    StatusPagamento NVARCHAR(20) NOT NULL DEFAULT 'Pendente',
    ResponsavelTecnico INT NULL,
    Garantia INT NULL, -- Dias de garantia
    DataGarantia DATETIME2 NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2 NULL,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT NULL,
    
    CONSTRAINT FK_OS_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_OS_Veiculo FOREIGN KEY (VeiculoId) REFERENCES Veiculos(Id),
    CONSTRAINT FK_OS_ResponsavelTecnico FOREIGN KEY (ResponsavelTecnico) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_OS_UsuarioCadastro FOREIGN KEY (UsuarioCadastro) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_OS_UsuarioAtualizacao FOREIGN KEY (UsuarioUltimaAtualizacao) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_OS_Status CHECK (Status IN ('Aberta', 'EmAndamento', 'Aguardando', 'Finalizada', 'Entregue', 'Cancelada')),
    CONSTRAINT CK_OS_Prioridade CHECK (Prioridade IN ('Baixa', 'Normal', 'Alta', 'Urgente')),
    CONSTRAINT CK_OS_StatusPagamento CHECK (StatusPagamento IN ('Pendente', 'Parcial', 'Pago', 'Cancelado')),
    CONSTRAINT CK_OS_KmEntrada CHECK (KmEntrada >= 0),
    CONSTRAINT CK_OS_KmSaida CHECK (KmSaida IS NULL OR KmSaida >= KmEntrada),
    CONSTRAINT CK_OS_Valores CHECK (ValorMaoObra >= 0 AND ValorPecas >= 0 AND ValorDesconto >= 0 AND ValorTotal >= 0),
    CONSTRAINT CK_OS_Garantia CHECK (Garantia IS NULL OR Garantia >= 0)
);

-- Índices para performance
CREATE INDEX IX_OS_Numero ON OrdemServico(Numero);
CREATE INDEX IX_OS_ClienteId ON OrdemServico(ClienteId);
CREATE INDEX IX_OS_VeiculoId ON OrdemServico(VeiculoId);
CREATE INDEX IX_OS_Status ON OrdemServico(Status);
CREATE INDEX IX_OS_DataEntrada ON OrdemServico(DataEntrada);
CREATE INDEX IX_OS_DataPrevista ON OrdemServico(DataPrevista);
CREATE INDEX IX_OS_ResponsavelTecnico ON OrdemServico(ResponsavelTecnico);
CREATE INDEX IX_OS_StatusPagamento ON OrdemServico(StatusPagamento);
CREATE INDEX IX_OS_Ativo ON OrdemServico(Ativo);
```

### Tabela: OSItens

```sql
CREATE TABLE OSItens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdemServicoId INT NOT NULL,
    TipoItem NVARCHAR(20) NOT NULL, -- 'Servico' ou 'Peca'
    ProdutoId INT NULL, -- FK para Produtos (quando for peça)
    ServicoId INT NULL, -- FK para Servicos (quando for serviço)
    Descricao NVARCHAR(200) NOT NULL,
    Quantidade DECIMAL(10,3) NOT NULL DEFAULT 1,
    ValorUnitario DECIMAL(10,2) NOT NULL,
    ValorDesconto DECIMAL(10,2) NOT NULL DEFAULT 0,
    ValorTotal DECIMAL(10,2) NOT NULL,
    Observacoes NVARCHAR(500) NULL,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioCadastro INT NOT NULL,
    
    CONSTRAINT FK_OSItens_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OSItens_Produto FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id),
    CONSTRAINT FK_OSItens_Servico FOREIGN KEY (ServicoId) REFERENCES Servicos(Id),
    CONSTRAINT FK_OSItens_UsuarioCadastro FOREIGN KEY (UsuarioCadastro) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_OSItens_TipoItem CHECK (TipoItem IN ('Servico', 'Peca')),
    CONSTRAINT CK_OSItens_Quantidade CHECK (Quantidade > 0),
    CONSTRAINT CK_OSItens_Valores CHECK (ValorUnitario >= 0 AND ValorDesconto >= 0 AND ValorTotal >= 0),
    CONSTRAINT CK_OSItens_ProdutoServico CHECK (
        (TipoItem = 'Peca' AND ProdutoId IS NOT NULL AND ServicoId IS NULL) OR
        (TipoItem = 'Servico' AND ServicoId IS NOT NULL AND ProdutoId IS NULL)
    )
);

CREATE INDEX IX_OSItens_OrdemServicoId ON OSItens(OrdemServicoId);
CREATE INDEX IX_OSItens_TipoItem ON OSItens(TipoItem);
CREATE INDEX IX_OSItens_ProdutoId ON OSItens(ProdutoId);
CREATE INDEX IX_OSItens_ServicoId ON OSItens(ServicoId);
```

### Tabela: OSHistorico

```sql
CREATE TABLE OSHistorico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdemServicoId INT NOT NULL,
    StatusAnterior NVARCHAR(20) NULL,
    StatusNovo NVARCHAR(20) NOT NULL,
    Observacao NVARCHAR(500) NULL,
    DataRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioRegistro INT NOT NULL,
    
    CONSTRAINT FK_OSHistorico_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OSHistorico_Usuario FOREIGN KEY (UsuarioRegistro) REFERENCES AspNetUsers(Id)
);

CREATE INDEX IX_OSHistorico_OrdemServicoId ON OSHistorico(OrdemServicoId);
CREATE INDEX IX_OSHistorico_DataRegistro ON OSHistorico(DataRegistro);
```

### Tabela: OSAnexos

```sql
CREATE TABLE OSAnexos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrdemServicoId INT NOT NULL,
    NomeArquivo NVARCHAR(255) NOT NULL,
    CaminhoArquivo NVARCHAR(500) NOT NULL,
    TipoArquivo NVARCHAR(50) NOT NULL,
    TamanhoArquivo BIGINT NOT NULL,
    Descricao NVARCHAR(200) NULL,
    DataUpload DATETIME2 NOT NULL DEFAULT GETDATE(),
    UsuarioUpload INT NOT NULL,
    
    CONSTRAINT FK_OSAnexos_OrdemServico FOREIGN KEY (OrdemServicoId) REFERENCES OrdemServico(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OSAnexos_Usuario FOREIGN KEY (UsuarioUpload) REFERENCES AspNetUsers(Id)
);

CREATE INDEX IX_OSAnexos_OrdemServicoId ON OSAnexos(OrdemServicoId);
```

## Entidades do Domínio

### OrdemServico.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class OrdemServico
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Número da OS é obrigatório")]
        [StringLength(20, ErrorMessage = "Número não pode exceder 20 caracteres")]
        public string Numero { get; set; }
        
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "Veículo é obrigatório")]
        public int VeiculoId { get; set; }
        
        public DateTime DataEntrada { get; set; } = DateTime.Now;
        
        public DateTime? DataPrevista { get; set; }
        
        public DateTime? DataSaida { get; set; }
        
        [Required(ErrorMessage = "Quilometragem de entrada é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser positiva")]
        public int KmEntrada { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser positiva")]
        public int? KmSaida { get; set; }
        
        [Required]
        public StatusOS Status { get; set; } = StatusOS.Aberta;
        
        [Required]
        public PrioridadeOS Prioridade { get; set; } = PrioridadeOS.Normal;
        
        [Required(ErrorMessage = "Tipo de serviço é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo de serviço não pode exceder 50 caracteres")]
        public string TipoServico { get; set; }
        
        [Required(ErrorMessage = "Defeito relatado é obrigatório")]
        public string DefeitoRelatado { get; set; }
        
        public string? DefeitoConstatado { get; set; }
        
        public string? ServicoExecutado { get; set; }
        
        public string? ObservacoesInternas { get; set; }
        
        public string? ObservacoesCliente { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorMaoObra { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorPecas { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorDesconto { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorTotal { get; set; }
        
        [StringLength(30, ErrorMessage = "Forma de pagamento não pode exceder 30 caracteres")]
        public string? FormaPagamento { get; set; }
        
        [Required]
        public StatusPagamentoOS StatusPagamento { get; set; } = StatusPagamentoOS.Pendente;
        
        public int? ResponsavelTecnico { get; set; }
        
        [Range(0, 3650, ErrorMessage = "Garantia deve ser entre 0 e 3650 dias")]
        public int? Garantia { get; set; }
        
        public DateTime? DataGarantia { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        public int UsuarioCadastro { get; set; }
        
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Navigation Properties
        public Cliente? Cliente { get; set; }
        public Veiculo? Veiculo { get; set; }
        public ICollection<OSItem>? Itens { get; set; }
        public ICollection<OSHistorico>? Historico { get; set; }
        public ICollection<OSAnexo>? Anexos { get; set; }
        
        // Propriedades calculadas
        public string StatusDescricao => Status.GetDescription();
        public string PrioridadeDescricao => Prioridade.GetDescription();
        public string StatusPagamentoDescricao => StatusPagamento.GetDescription();
        
        public decimal ValorSubtotal => ValorMaoObra + ValorPecas;
        public decimal ValorLiquido => ValorSubtotal - ValorDesconto;
        
        public bool PodeEditar => Status == StatusOS.Aberta || Status == StatusOS.EmAndamento;
        public bool PodeFinalizar => Status == StatusOS.EmAndamento;
        public bool PodeEntregar => Status == StatusOS.Finalizada;
        public bool PodeCancelar => Status != StatusOS.Entregue && Status != StatusOS.Cancelada;
        
        public int DiasEmAndamento => Status == StatusOS.Entregue && DataSaida.HasValue 
            ? (DataSaida.Value.Date - DataEntrada.Date).Days 
            : (DateTime.Now.Date - DataEntrada.Date).Days;
        
        public bool EmAtraso => DataPrevista.HasValue && DateTime.Now > DataPrevista.Value && Status != StatusOS.Entregue;
        
        public bool GarantiaVigente => DataGarantia.HasValue && DateTime.Now <= DataGarantia.Value;
        
        public int DiasGarantiaRestantes => DataGarantia.HasValue 
            ? Math.Max(0, (DataGarantia.Value.Date - DateTime.Now.Date).Days)
            : 0;
        
        // Métodos
        public void CalcularValorTotal()
        {
            ValorTotal = ValorSubtotal - ValorDesconto;
        }
        
        public void DefinirDataGarantia()
        {
            if (Garantia.HasValue && Garantia.Value > 0 && DataSaida.HasValue)
            {
                DataGarantia = DataSaida.Value.AddDays(Garantia.Value);
            }
            else
            {
                DataGarantia = null;
            }
        }
        
        public bool ValidarKmSaida()
        {
            return !KmSaida.HasValue || KmSaida.Value >= KmEntrada;
        }
    }
    
    public class OSItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrdemServicoId { get; set; }
        
        [Required]
        public TipoItemOS TipoItem { get; set; }
        
        public int? ProdutoId { get; set; }
        
        public int? ServicoId { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "Descrição não pode exceder 200 caracteres")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public decimal Quantidade { get; set; } = 1;
        
        [Required(ErrorMessage = "Valor unitário é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorUnitario { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
        public decimal ValorDesconto { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor total deve ser positivo")]
        public decimal ValorTotal { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações não podem exceder 500 caracteres")]
        public string? Observacoes { get; set; }
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public int UsuarioCadastro { get; set; }
        
        // Navigation Properties
        public OrdemServico? OrdemServico { get; set; }
        public Produto? Produto { get; set; }
        public Servico? Servico { get; set; }
        
        // Propriedades calculadas
        public string TipoItemDescricao => TipoItem.GetDescription();
        public decimal ValorSubtotal => Quantidade * ValorUnitario;
        public decimal ValorLiquido => ValorSubtotal - ValorDesconto;
        
        // Métodos
        public void CalcularValorTotal()
        {
            ValorTotal = ValorLiquido;
        }
    }
    
    public class OSHistorico
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public StatusOS? StatusAnterior { get; set; }
        public StatusOS StatusNovo { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public int UsuarioRegistro { get; set; }
        
        // Navigation Properties
        public OrdemServico? OrdemServico { get; set; }
        
        // Propriedades calculadas
        public string StatusAnteriorDescricao => StatusAnterior?.GetDescription() ?? "Inicial";
        public string StatusNovoDescricao => StatusNovo.GetDescription();
    }
    
    public class OSAnexo
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public string NomeArquivo { get; set; }
        public string CaminhoArquivo { get; set; }
        public string TipoArquivo { get; set; }
        public long TamanhoArquivo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataUpload { get; set; } = DateTime.Now;
        public int UsuarioUpload { get; set; }
        
        // Navigation Properties
        public OrdemServico? OrdemServico { get; set; }
        
        // Propriedades calculadas
        public string TamanhoFormatado => FormatarTamanho(TamanhoArquivo);
        public bool EhImagem => TipoArquivo.StartsWith("image/");
        public bool EhPDF => TipoArquivo == "application/pdf";
        
        private static string FormatarTamanho(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
```

## Enums

### StatusOS.cs

```csharp
using System.ComponentModel;

namespace MecTecERP.Domain.Enums
{
    public enum StatusOS
    {
        [Description("Aberta")]
        Aberta = 1,
        
        [Description("Em Andamento")]
        EmAndamento = 2,
        
        [Description("Aguardando Peças")]
        Aguardando = 3,
        
        [Description("Finalizada")]
        Finalizada = 4,
        
        [Description("Entregue")]
        Entregue = 5,
        
        [Description("Cancelada")]
        Cancelada = 6
    }
    
    public enum PrioridadeOS
    {
        [Description("Baixa")]
        Baixa = 1,
        
        [Description("Normal")]
        Normal = 2,
        
        [Description("Alta")]
        Alta = 3,
        
        [Description("Urgente")]
        Urgente = 4
    }
    
    public enum StatusPagamentoOS
    {
        [Description("Pendente")]
        Pendente = 1,
        
        [Description("Parcial")]
        Parcial = 2,
        
        [Description("Pago")]
        Pago = 3,
        
        [Description("Cancelado")]
        Cancelado = 4
    }
    
    public enum TipoItemOS
    {
        [Description("Serviço")]
        Servico = 1,
        
        [Description("Peça")]
        Peca = 2
    }
}
```

## Extensions

### EnumExtensions.cs

```csharp
using System.ComponentModel;
using System.Reflection;

namespace MecTecERP.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
        
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            
            throw new ArgumentException($"Não foi encontrado um valor para a descrição '{description}'", nameof(description));
        }
    }
}
```

## Helpers

### OSHelper.cs

```csharp
namespace MecTecERP.Domain.Helpers
{
    public static class OSHelper
    {
        public static string GerarNumeroOS(int proximoId, DateTime data)
        {
            // Formato: OS-AAAA-NNNNNN (OS-2024-000001)
            return $"OS-{data.Year}-{proximoId:D6}";
        }
        
        public static string GerarNumeroOSPersonalizado(int proximoId, DateTime data, string prefixo = "OS")
        {
            // Formato personalizado: PREFIXO-AAAA-NNNNNN
            return $"{prefixo}-{data.Year}-{proximoId:D6}";
        }
        
        public static bool ValidarTransicaoStatus(StatusOS statusAtual, StatusOS novoStatus)
        {
            return novoStatus switch
            {
                StatusOS.Aberta => false, // Não pode voltar para aberta
                StatusOS.EmAndamento => statusAtual == StatusOS.Aberta || statusAtual == StatusOS.Aguardando,
                StatusOS.Aguardando => statusAtual == StatusOS.EmAndamento,
                StatusOS.Finalizada => statusAtual == StatusOS.EmAndamento || statusAtual == StatusOS.Aguardando,
                StatusOS.Entregue => statusAtual == StatusOS.Finalizada,
                StatusOS.Cancelada => statusAtual != StatusOS.Entregue,
                _ => false
            };
        }
        
        public static string GetCorStatus(StatusOS status)
        {
            return status switch
            {
                StatusOS.Aberta => "primary",
                StatusOS.EmAndamento => "warning",
                StatusOS.Aguardando => "info",
                StatusOS.Finalizada => "success",
                StatusOS.Entregue => "secondary",
                StatusOS.Cancelada => "danger",
                _ => "light"
            };
        }
        
        public static string GetCorPrioridade(PrioridadeOS prioridade)
        {
            return prioridade switch
            {
                PrioridadeOS.Baixa => "success",
                PrioridadeOS.Normal => "primary",
                PrioridadeOS.Alta => "warning",
                PrioridadeOS.Urgente => "danger",
                _ => "light"
            };
        }
        
        public static string GetIconeStatus(StatusOS status)
        {
            return status switch
            {
                StatusOS.Aberta => "fas fa-folder-open",
                StatusOS.EmAndamento => "fas fa-cogs",
                StatusOS.Aguardando => "fas fa-clock",
                StatusOS.Finalizada => "fas fa-check-circle",
                StatusOS.Entregue => "fas fa-handshake",
                StatusOS.Cancelada => "fas fa-times-circle",
                _ => "fas fa-question-circle"
            };
        }
        
        public static List<string> GetTiposServico()
        {
            return new List<string>
            {
                "Manutenção Preventiva",
                "Manutenção Corretiva",
                "Revisão",
                "Diagnóstico",
                "Troca de Óleo",
                "Alinhamento",
                "Balanceamento",
                "Freios",
                "Suspensão",
                "Motor",
                "Transmissão",
                "Ar Condicionado",
                "Sistema Elétrico",
                "Funilaria",
                "Pintura",
                "Outros"
            };
        }
        
        public static List<string> GetFormasPagamento()
        {
            return new List<string>
            {
                "Dinheiro",
                "Cartão de Débito",
                "Cartão de Crédito",
                "PIX",
                "Transferência Bancária",
                "Boleto",
                "Cheque",
                "Crediário",
                "Cartão da Loja"
            };
        }
    }
}
```

## Continuação na Parte 2

Este documento continua na **IMPLEMENTACAO_OS_PARTE2.md** com:
- DTOs e ViewModels
- Interfaces de Repositório
- Implementação dos Repositórios
- Serviços de Domínio
- Validações de Negócio