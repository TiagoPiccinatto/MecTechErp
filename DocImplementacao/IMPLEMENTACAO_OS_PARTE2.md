# Implementação do Módulo de Ordem de Serviço (OS) - Parte 2 - MecTecERP

## DTOs e ViewModels

### OrdemServicoDto.cs

```csharp
using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Shared.DTOs
{
    public class OrdemServicoDto
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; }
        public string ClienteDocumento { get; set; }
        public string ClienteTelefone { get; set; }
        public int VeiculoId { get; set; }
        public string VeiculoDescricao { get; set; }
        public string VeiculoPlaca { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataPrevista { get; set; }
        public DateTime? DataSaida { get; set; }
        public int KmEntrada { get; set; }
        public int? KmSaida { get; set; }
        public StatusOS Status { get; set; }
        public string StatusDescricao { get; set; }
        public PrioridadeOS Prioridade { get; set; }
        public string PrioridadeDescricao { get; set; }
        public string TipoServico { get; set; }
        public string DefeitoRelatado { get; set; }
        public string? DefeitoConstatado { get; set; }
        public string? ServicoExecutado { get; set; }
        public string? ObservacoesInternas { get; set; }
        public string? ObservacoesCliente { get; set; }
        public decimal ValorMaoObra { get; set; }
        public decimal ValorPecas { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public string? FormaPagamento { get; set; }
        public StatusPagamentoOS StatusPagamento { get; set; }
        public string StatusPagamentoDescricao { get; set; }
        public int? ResponsavelTecnico { get; set; }
        public string? ResponsavelTecnicoNome { get; set; }
        public int? Garantia { get; set; }
        public DateTime? DataGarantia { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        // Propriedades calculadas
        public decimal ValorSubtotal { get; set; }
        public decimal ValorLiquido { get; set; }
        public bool PodeEditar { get; set; }
        public bool PodeFinalizar { get; set; }
        public bool PodeEntregar { get; set; }
        public bool PodeCancelar { get; set; }
        public int DiasEmAndamento { get; set; }
        public bool EmAtraso { get; set; }
        public bool GarantiaVigente { get; set; }
        public int DiasGarantiaRestantes { get; set; }
        public string CorStatus { get; set; }
        public string CorPrioridade { get; set; }
        public string IconeStatus { get; set; }
        
        // Listas relacionadas
        public List<OSItemDto> Itens { get; set; } = new();
        public List<OSHistoricoDto> Historico { get; set; } = new();
        public List<OSAnexoDto> Anexos { get; set; } = new();
    }
    
    public class OrdemServicoCreateDto
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "Veículo é obrigatório")]
        public int VeiculoId { get; set; }
        
        public DateTime? DataPrevista { get; set; }
        
        [Required(ErrorMessage = "Quilometragem de entrada é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser positiva")]
        public int KmEntrada { get; set; }
        
        [Required]
        public PrioridadeOS Prioridade { get; set; } = PrioridadeOS.Normal;
        
        [Required(ErrorMessage = "Tipo de serviço é obrigatório")]
        [StringLength(50)]
        public string TipoServico { get; set; }
        
        [Required(ErrorMessage = "Defeito relatado é obrigatório")]
        public string DefeitoRelatado { get; set; }
        
        public string? ObservacoesInternas { get; set; }
        
        public string? ObservacoesCliente { get; set; }
        
        public int? ResponsavelTecnico { get; set; }
        
        [Range(0, 3650, ErrorMessage = "Garantia deve ser entre 0 e 3650 dias")]
        public int? Garantia { get; set; }
        
        public List<OSItemCreateDto> Itens { get; set; } = new();
    }
    
    public class OrdemServicoUpdateDto : OrdemServicoCreateDto
    {
        public int Id { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser positiva")]
        public int? KmSaida { get; set; }
        
        public string? DefeitoConstatado { get; set; }
        
        public string? ServicoExecutado { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorMaoObra { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorPecas { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorDesconto { get; set; }
        
        public string? FormaPagamento { get; set; }
        
        public StatusPagamentoOS StatusPagamento { get; set; }
    }
    
    public class OrdemServicoListDto
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string ClienteNome { get; set; }
        public string VeiculoDescricao { get; set; }
        public string VeiculoPlaca { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime? DataPrevista { get; set; }
        public StatusOS Status { get; set; }
        public string StatusDescricao { get; set; }
        public PrioridadeOS Prioridade { get; set; }
        public string PrioridadeDescricao { get; set; }
        public string TipoServico { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusPagamentoOS StatusPagamento { get; set; }
        public string StatusPagamentoDescricao { get; set; }
        public string? ResponsavelTecnicoNome { get; set; }
        public int DiasEmAndamento { get; set; }
        public bool EmAtraso { get; set; }
        public string CorStatus { get; set; }
        public string CorPrioridade { get; set; }
        public string IconeStatus { get; set; }
    }
    
    public class OrdemServicoSearchDto
    {
        public int Id { get; set; }
        public string Numero { get; set; }
        public string ClienteNome { get; set; }
        public string VeiculoDescricao { get; set; }
        public StatusOS Status { get; set; }
        public string StatusDescricao { get; set; }
    }
    
    public class OSItemDto
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public TipoItemOS TipoItem { get; set; }
        public string TipoItemDescricao { get; set; }
        public int? ProdutoId { get; set; }
        public int? ServicoId { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataCadastro { get; set; }
        
        // Propriedades calculadas
        public decimal ValorSubtotal { get; set; }
        public decimal ValorLiquido { get; set; }
    }
    
    public class OSItemCreateDto
    {
        [Required]
        public TipoItemOS TipoItem { get; set; }
        
        public int? ProdutoId { get; set; }
        
        public int? ServicoId { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200)]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public decimal Quantidade { get; set; } = 1;
        
        [Required(ErrorMessage = "Valor unitário é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        public decimal ValorUnitario { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
        public decimal ValorDesconto { get; set; }
        
        [StringLength(500)]
        public string? Observacoes { get; set; }
    }
    
    public class OSItemUpdateDto : OSItemCreateDto
    {
        public int Id { get; set; }
    }
    
    public class OSHistoricoDto
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public StatusOS? StatusAnterior { get; set; }
        public string StatusAnteriorDescricao { get; set; }
        public StatusOS StatusNovo { get; set; }
        public string StatusNovoDescricao { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataRegistro { get; set; }
        public string UsuarioNome { get; set; }
    }
    
    public class OSAnexoDto
    {
        public int Id { get; set; }
        public int OrdemServicoId { get; set; }
        public string NomeArquivo { get; set; }
        public string CaminhoArquivo { get; set; }
        public string TipoArquivo { get; set; }
        public long TamanhoArquivo { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataUpload { get; set; }
        public string UsuarioNome { get; set; }
        
        // Propriedades calculadas
        public string TamanhoFormatado { get; set; }
        public bool EhImagem { get; set; }
        public bool EhPDF { get; set; }
    }
    
    public class OSStatusUpdateDto
    {
        [Required]
        public int OrdemServicoId { get; set; }
        
        [Required]
        public StatusOS NovoStatus { get; set; }
        
        [StringLength(500)]
        public string? Observacao { get; set; }
        
        public DateTime? DataSaida { get; set; }
        
        public int? KmSaida { get; set; }
    }
    
    public class OSRelatorioDto
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int TotalOS { get; set; }
        public int OSAbertas { get; set; }
        public int OSEmAndamento { get; set; }
        public int OSFinalizadas { get; set; }
        public int OSEntregues { get; set; }
        public int OSCanceladas { get; set; }
        public decimal ValorTotalServicos { get; set; }
        public decimal ValorTotalPecas { get; set; }
        public decimal ValorTotalGeral { get; set; }
        public decimal TicketMedio { get; set; }
        public int TempoMedioExecucao { get; set; }
        public List<OrdemServicoListDto> OSDetalhes { get; set; } = new();
    }
}
```

## Interfaces de Repositório

### IOrdemServicoRepository.cs

```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Shared.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IOrdemServicoRepository
    {
        // CRUD básico
        Task<IEnumerable<OrdemServicoListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, 
            StatusOS? status = null, int? clienteId = null, int? responsavelId = null, DateTime? dataInicio = null, 
            DateTime? dataFim = null, bool? emAtraso = null);
        Task<OrdemServicoDto?> GetByIdAsync(int id);
        Task<OrdemServico?> GetEntityByIdAsync(int id);
        Task<OrdemServicoDto?> GetByNumeroAsync(string numero);
        Task<int> CreateAsync(OrdemServicoCreateDto os, int usuarioId);
        Task<bool> UpdateAsync(OrdemServicoUpdateDto os, int usuarioId);
        Task<bool> DeleteAsync(int id);
        
        // Operações de status
        Task<bool> UpdateStatusAsync(OSStatusUpdateDto statusUpdate, int usuarioId);
        Task<bool> CanChangeStatusAsync(int osId, StatusOS novoStatus);
        
        // Numeração
        Task<string> GerarProximoNumeroAsync();
        Task<bool> ExistsByNumeroAsync(string numero, int? excludeId = null);
        
        // Itens
        Task<IEnumerable<OSItemDto>> GetItensAsync(int ordemServicoId);
        Task<int> AddItemAsync(int ordemServicoId, OSItemCreateDto item, int usuarioId);
        Task<bool> UpdateItemAsync(OSItemUpdateDto item, int usuarioId);
        Task<bool> RemoveItemAsync(int itemId);
        Task<bool> RecalcularValoresAsync(int ordemServicoId);
        
        // Histórico
        Task<IEnumerable<OSHistoricoDto>> GetHistoricoAsync(int ordemServicoId);
        Task<bool> AddHistoricoAsync(int ordemServicoId, StatusOS? statusAnterior, StatusOS statusNovo, 
            string? observacao, int usuarioId);
        
        // Anexos
        Task<IEnumerable<OSAnexoDto>> GetAnexosAsync(int ordemServicoId);
        Task<int> AddAnexoAsync(int ordemServicoId, string nomeArquivo, string caminhoArquivo, 
            string tipoArquivo, long tamanhoArquivo, string? descricao, int usuarioId);
        Task<bool> RemoveAnexoAsync(int anexoId);
        
        // Consultas específicas
        Task<int> GetTotalCountAsync(string? search = null, StatusOS? status = null, int? clienteId = null, 
            int? responsavelId = null, DateTime? dataInicio = null, DateTime? dataFim = null, bool? emAtraso = null);
        Task<IEnumerable<OrdemServicoSearchDto>> SearchAsync(string term);
        Task<IEnumerable<OrdemServicoListDto>> GetByClienteAsync(int clienteId);
        Task<IEnumerable<OrdemServicoListDto>> GetByVeiculoAsync(int veiculoId);
        Task<IEnumerable<OrdemServicoListDto>> GetByResponsavelAsync(int responsavelId);
        Task<IEnumerable<OrdemServicoListDto>> GetEmAtrasoAsync();
        Task<IEnumerable<OrdemServicoListDto>> GetComGarantiaVigenteAsync();
        
        // Relatórios
        Task<OSRelatorioDto> GetRelatorioAsync(DateTime dataInicio, DateTime dataFim, int? clienteId = null, 
            int? responsavelId = null, StatusOS? status = null);
        Task<IEnumerable<dynamic>> GetEstatisticasStatusAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<dynamic>> GetEstatisticasTecnicosAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<dynamic>> GetEstatisticasClientesAsync(DateTime dataInicio, DateTime dataFim);
    }
}
```

### IOrdemServicoService.cs

```csharp
using MecTecERP.Domain.Enums;
using MecTecERP.Shared.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IOrdemServicoService
    {
        // CRUD
        Task<IEnumerable<OrdemServicoListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, 
            StatusOS? status = null, int? clienteId = null, int? responsavelId = null, DateTime? dataInicio = null, 
            DateTime? dataFim = null, bool? emAtraso = null);
        Task<OrdemServicoDto?> GetByIdAsync(int id);
        Task<OrdemServicoDto?> GetByNumeroAsync(string numero);
        Task<int> CreateAsync(OrdemServicoCreateDto os, int usuarioId);
        Task<bool> UpdateAsync(OrdemServicoUpdateDto os, int usuarioId);
        Task<bool> DeleteAsync(int id, int usuarioId);
        
        // Operações de negócio
        Task<bool> IniciarAndamentoAsync(int osId, int usuarioId, string? observacao = null);
        Task<bool> ColocarEmEsperaAsync(int osId, int usuarioId, string? observacao = null);
        Task<bool> FinalizarAsync(int osId, int usuarioId, string? observacao = null, DateTime? dataSaida = null, int? kmSaida = null);
        Task<bool> EntregarAsync(int osId, int usuarioId, string? observacao = null);
        Task<bool> CancelarAsync(int osId, int usuarioId, string observacao);
        
        // Validações
        Task<bool> ValidarCriacaoAsync(OrdemServicoCreateDto os);
        Task<bool> ValidarAtualizacaoAsync(OrdemServicoUpdateDto os);
        Task<bool> ValidarFinalizacaoAsync(int osId);
        Task<bool> ValidarEntregaAsync(int osId);
        
        // Itens
        Task<IEnumerable<OSItemDto>> GetItensAsync(int ordemServicoId);
        Task<int> AddItemAsync(int ordemServicoId, OSItemCreateDto item, int usuarioId);
        Task<bool> UpdateItemAsync(OSItemUpdateDto item, int usuarioId);
        Task<bool> RemoveItemAsync(int itemId, int usuarioId);
        
        // Anexos
        Task<IEnumerable<OSAnexoDto>> GetAnexosAsync(int ordemServicoId);
        Task<int> AddAnexoAsync(int ordemServicoId, IFormFile arquivo, string? descricao, int usuarioId);
        Task<bool> RemoveAnexoAsync(int anexoId, int usuarioId);
        
        // Consultas
        Task<int> GetTotalCountAsync(string? search = null, StatusOS? status = null, int? clienteId = null, 
            int? responsavelId = null, DateTime? dataInicio = null, DateTime? dataFim = null, bool? emAtraso = null);
        Task<IEnumerable<OrdemServicoSearchDto>> SearchAsync(string term);
        Task<IEnumerable<OrdemServicoListDto>> GetByClienteAsync(int clienteId);
        Task<IEnumerable<OrdemServicoListDto>> GetByVeiculoAsync(int veiculoId);
        
        // Relatórios
        Task<OSRelatorioDto> GetRelatorioAsync(DateTime dataInicio, DateTime dataFim, int? clienteId = null, 
            int? responsavelId = null, StatusOS? status = null);
        Task<byte[]> GerarRelatorioOSAsync(int osId);
        Task<byte[]> GerarRelatorioPeriodicoAsync(DateTime dataInicio, DateTime dataFim, string formato = "PDF");
        
        // Notificações
        Task<IEnumerable<OrdemServicoListDto>> GetOSVencendoAsync(int dias = 3);
        Task<IEnumerable<OrdemServicoListDto>> GetOSEmAtrasoAsync();
        Task<IEnumerable<OrdemServicoListDto>> GetOSComGarantiaVencendoAsync(int dias = 30);
    }
}
```

## Implementação do Repositório

### OrdemServicoRepository.cs (Parte 1)

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Helpers;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Shared.DTOs;
using System.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class OrdemServicoRepository : IOrdemServicoRepository
    {
        private readonly string _connectionString;
        
        public OrdemServicoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetAllAsync(int page = 1, int pageSize = 50, 
            string? search = null, StatusOS? status = null, int? clienteId = null, int? responsavelId = null, 
            DateTime? dataInicio = null, DateTime? dataFim = null, bool? emAtraso = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    os.Id,
                    os.Numero,
                    c.NomeRazaoSocial as ClienteNome,
                    CONCAT(v.Marca, ' ', v.Modelo, ' ', v.AnoModelo) as VeiculoDescricao,
                    v.Placa as VeiculoPlaca,
                    os.DataEntrada,
                    os.DataPrevista,
                    os.Status,
                    os.Prioridade,
                    os.TipoServico,
                    os.ValorTotal,
                    os.StatusPagamento,
                    u.UserName as ResponsavelTecnicoNome,
                    DATEDIFF(DAY, os.DataEntrada, ISNULL(os.DataSaida, GETDATE())) as DiasEmAndamento,
                    CASE WHEN os.DataPrevista IS NOT NULL AND GETDATE() > os.DataPrevista AND os.Status NOT IN (5, 6) THEN 1 ELSE 0 END as EmAtraso
                FROM OrdemServico os
                INNER JOIN Clientes c ON os.ClienteId = c.Id
                INNER JOIN Veiculos v ON os.VeiculoId = v.Id
                LEFT JOIN AspNetUsers u ON os.ResponsavelTecnico = u.Id
                WHERE os.Ativo = 1
                  AND (@Search IS NULL OR os.Numero LIKE '%' + @Search + '%' 
                       OR c.NomeRazaoSocial LIKE '%' + @Search + '%'
                       OR v.Placa LIKE '%' + @Search + '%'
                       OR v.Marca LIKE '%' + @Search + '%'
                       OR v.Modelo LIKE '%' + @Search + '%')
                  AND (@Status IS NULL OR os.Status = @Status)
                  AND (@ClienteId IS NULL OR os.ClienteId = @ClienteId)
                  AND (@ResponsavelId IS NULL OR os.ResponsavelTecnico = @ResponsavelId)
                  AND (@DataInicio IS NULL OR os.DataEntrada >= @DataInicio)
                  AND (@DataFim IS NULL OR os.DataEntrada <= @DataFim)
                  AND (@EmAtraso IS NULL OR 
                       (@EmAtraso = 1 AND os.DataPrevista IS NOT NULL AND GETDATE() > os.DataPrevista AND os.Status NOT IN (5, 6)) OR
                       (@EmAtraso = 0 AND (os.DataPrevista IS NULL OR GETDATE() <= os.DataPrevista OR os.Status IN (5, 6))))
                ORDER BY 
                    CASE WHEN os.Status = 4 THEN 1 ELSE 0 END, -- Urgente primeiro
                    os.Prioridade DESC,
                    os.DataEntrada DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            var parameters = new
            {
                Search = search,
                Status = (int?)status,
                ClienteId = clienteId,
                ResponsavelId = responsavelId,
                DataInicio = dataInicio,
                DataFim = dataFim,
                EmAtraso = emAtraso,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            };
            
            var result = await connection.QueryAsync<OrdemServicoListDto>(sql, parameters);
            
            // Adicionar propriedades calculadas
            foreach (var os in result)
            {
                os.StatusDescricao = os.Status.GetDescription();
                os.PrioridadeDescricao = os.Prioridade.GetDescription();
                os.StatusPagamentoDescricao = os.StatusPagamento.GetDescription();
                os.CorStatus = OSHelper.GetCorStatus(os.Status);
                os.CorPrioridade = OSHelper.GetCorPrioridade(os.Prioridade);
                os.IconeStatus = OSHelper.GetIconeStatus(os.Status);
            }
            
            return result;
        }
        
        public async Task<OrdemServicoDto?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    os.*,
                    c.NomeRazaoSocial as ClienteNome,
                    c.CpfCnpj as ClienteDocumento,
                    c.Telefone as ClienteTelefone,
                    CONCAT(v.Marca, ' ', v.Modelo, ' ', v.AnoModelo) as VeiculoDescricao,
                    v.Placa as VeiculoPlaca,
                    u.UserName as ResponsavelTecnicoNome
                FROM OrdemServico os
                INNER JOIN Clientes c ON os.ClienteId = c.Id
                INNER JOIN Veiculos v ON os.VeiculoId = v.Id
                LEFT JOIN AspNetUsers u ON os.ResponsavelTecnico = u.Id
                WHERE os.Id = @Id";
            
            var os = await connection.QueryFirstOrDefaultAsync<OrdemServicoDto>(sql, new { Id = id });
            
            if (os != null)
            {
                // Adicionar propriedades calculadas
                os.StatusDescricao = os.Status.GetDescription();
                os.PrioridadeDescricao = os.Prioridade.GetDescription();
                os.StatusPagamentoDescricao = os.StatusPagamento.GetDescription();
                os.ValorSubtotal = os.ValorMaoObra + os.ValorPecas;
                os.ValorLiquido = os.ValorSubtotal - os.ValorDesconto;
                os.PodeEditar = os.Status == StatusOS.Aberta || os.Status == StatusOS.EmAndamento;
                os.PodeFinalizar = os.Status == StatusOS.EmAndamento;
                os.PodeEntregar = os.Status == StatusOS.Finalizada;
                os.PodeCancelar = os.Status != StatusOS.Entregue && os.Status != StatusOS.Cancelada;
                os.DiasEmAndamento = os.Status == StatusOS.Entregue && os.DataSaida.HasValue 
                    ? (os.DataSaida.Value.Date - os.DataEntrada.Date).Days 
                    : (DateTime.Now.Date - os.DataEntrada.Date).Days;
                os.EmAtraso = os.DataPrevista.HasValue && DateTime.Now > os.DataPrevista.Value && os.Status != StatusOS.Entregue;
                os.GarantiaVigente = os.DataGarantia.HasValue && DateTime.Now <= os.DataGarantia.Value;
                os.DiasGarantiaRestantes = os.DataGarantia.HasValue 
                    ? Math.Max(0, (os.DataGarantia.Value.Date - DateTime.Now.Date).Days)
                    : 0;
                os.CorStatus = OSHelper.GetCorStatus(os.Status);
                os.CorPrioridade = OSHelper.GetCorPrioridade(os.Prioridade);
                os.IconeStatus = OSHelper.GetIconeStatus(os.Status);
                
                // Carregar itens, histórico e anexos
                os.Itens = (await GetItensAsync(id)).ToList();
                os.Historico = (await GetHistoricoAsync(id)).ToList();
                os.Anexos = (await GetAnexosAsync(id)).ToList();
            }
            
            return os;
        }
        
        public async Task<OrdemServico?> GetEntityByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT * FROM OrdemServico WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<OrdemServico>(sql, new { Id = id });
        }
        
        public async Task<OrdemServicoDto?> GetByNumeroAsync(string numero)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    os.*,
                    c.NomeRazaoSocial as ClienteNome,
                    c.CpfCnpj as ClienteDocumento,
                    c.Telefone as ClienteTelefone,
                    CONCAT(v.Marca, ' ', v.Modelo, ' ', v.AnoModelo) as VeiculoDescricao,
                    v.Placa as VeiculoPlaca,
                    u.UserName as ResponsavelTecnicoNome
                FROM OrdemServico os
                INNER JOIN Clientes c ON os.ClienteId = c.Id
                INNER JOIN Veiculos v ON os.VeiculoId = v.Id
                LEFT JOIN AspNetUsers u ON os.ResponsavelTecnico = u.Id
                WHERE os.Numero = @Numero";
            
            var os = await connection.QueryFirstOrDefaultAsync<OrdemServicoDto>(sql, new { Numero = numero });
            
            if (os != null)
            {
                // Adicionar propriedades calculadas (mesmo código do GetByIdAsync)
                // ... (código omitido para brevidade)
            }
            
            return os;
        }
        
        public async Task<string> GerarProximoNumeroAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT ISNULL(MAX(CAST(RIGHT(Numero, 6) AS INT)), 0) + 1
                FROM OrdemServico 
                WHERE Numero LIKE 'OS-' + CAST(YEAR(GETDATE()) AS VARCHAR) + '-%'";
            
            var proximoId = await connection.QueryFirstAsync<int>(sql);
            return OSHelper.GerarNumeroOS(proximoId, DateTime.Now);
        }
        
        public async Task<bool> ExistsByNumeroAsync(string numero, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT COUNT(1) FROM OrdemServico WHERE Numero = @Numero AND (@ExcludeId IS NULL OR Id != @ExcludeId)";
            var count = await connection.QueryFirstAsync<int>(sql, new { Numero = numero, ExcludeId = excludeId });
            
            return count > 0;
        }
        
        public async Task<int> CreateAsync(OrdemServicoCreateDto os, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Gerar número da OS
                var numero = await GerarProximoNumeroAsync();
                
                var sql = @"
                    INSERT INTO OrdemServico (
                        Numero, ClienteId, VeiculoId, DataEntrada, DataPrevista, KmEntrada,
                        Prioridade, TipoServico, DefeitoRelatado, ObservacoesInternas, 
                        ObservacoesCliente, ResponsavelTecnico, Garantia, UsuarioCadastro
                    ) VALUES (
                        @Numero, @ClienteId, @VeiculoId, @DataEntrada, @DataPrevista, @KmEntrada,
                        @Prioridade, @TipoServico, @DefeitoRelatado, @ObservacoesInternas,
                        @ObservacoesCliente, @ResponsavelTecnico, @Garantia, @UsuarioCadastro
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                
                var parameters = new
                {
                    Numero = numero,
                    os.ClienteId,
                    os.VeiculoId,
                    DataEntrada = DateTime.Now,
                    os.DataPrevista,
                    os.KmEntrada,
                    Prioridade = (int)os.Prioridade,
                    os.TipoServico,
                    os.DefeitoRelatado,
                    os.ObservacoesInternas,
                    os.ObservacoesCliente,
                    os.ResponsavelTecnico,
                    os.Garantia,
                    UsuarioCadastro = usuarioId
                };
                
                var osId = await connection.QueryFirstAsync<int>(sql, parameters, transaction);
                
                // Adicionar itens se houver
                foreach (var item in os.Itens)
                {
                    await AddItemInternalAsync(connection, transaction, osId, item, usuarioId);
                }
                
                // Recalcular valores
                await RecalcularValoresInternalAsync(connection, transaction, osId);
                
                // Adicionar histórico inicial
                await AddHistoricoInternalAsync(connection, transaction, osId, null, StatusOS.Aberta, 
                    "OS criada", usuarioId);
                
                // Atualizar KM do veículo
                var updateKmSql = @"
                    UPDATE Veiculos SET 
                        KmAtual = @KmEntrada,
                        DataUltimaAtualizacao = GETDATE(),
                        UsuarioUltimaAtualizacao = @UsuarioId
                    WHERE Id = @VeiculoId AND KmAtual < @KmEntrada";
                
                await connection.ExecuteAsync(updateKmSql, new
                {
                    os.KmEntrada,
                    os.VeiculoId,
                    UsuarioId = usuarioId
                }, transaction);
                
                transaction.Commit();
                return osId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        // Métodos auxiliares internos
        private async Task<int> AddItemInternalAsync(IDbConnection connection, IDbTransaction transaction, 
            int ordemServicoId, OSItemCreateDto item, int usuarioId)
        {
            var sql = @"
                INSERT INTO OSItens (
                    OrdemServicoId, TipoItem, ProdutoId, ServicoId, Descricao,
                    Quantidade, ValorUnitario, ValorDesconto, ValorTotal, Observacoes, UsuarioCadastro
                ) VALUES (
                    @OrdemServicoId, @TipoItem, @ProdutoId, @ServicoId, @Descricao,
                    @Quantidade, @ValorUnitario, @ValorDesconto, @ValorTotal, @Observacoes, @UsuarioCadastro
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
            
            var valorTotal = (item.Quantidade * item.ValorUnitario) - item.ValorDesconto;
            
            var parameters = new
            {
                OrdemServicoId = ordemServicoId,
                TipoItem = (int)item.TipoItem,
                item.ProdutoId,
                item.ServicoId,
                item.Descricao,
                item.Quantidade,
                item.ValorUnitario,
                item.ValorDesconto,
                ValorTotal = valorTotal,
                item.Observacoes,
                UsuarioCadastro = usuarioId
            };
            
            return await connection.QueryFirstAsync<int>(sql, parameters, transaction);
        }
        
        private async Task<bool> RecalcularValoresInternalAsync(IDbConnection connection, IDbTransaction transaction, int ordemServicoId)
        {
            var sql = @"
                UPDATE OrdemServico SET
                    ValorMaoObra = ISNULL((SELECT SUM(ValorTotal) FROM OSItens WHERE OrdemServicoId = @Id AND TipoItem = 1), 0),
                    ValorPecas = ISNULL((SELECT SUM(ValorTotal) FROM OSItens WHERE OrdemServicoId = @Id AND TipoItem = 2), 0)
                WHERE Id = @Id;
                
                UPDATE OrdemServico SET
                    ValorTotal = ValorMaoObra + ValorPecas - ValorDesconto
                WHERE Id = @Id;";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = ordemServicoId }, transaction);
            return rowsAffected > 0;
        }
        
        private async Task<bool> AddHistoricoInternalAsync(IDbConnection connection, IDbTransaction transaction, 
            int ordemServicoId, StatusOS? statusAnterior, StatusOS statusNovo, string? observacao, int usuarioId)
        {
            var sql = @"
                INSERT INTO OSHistorico (OrdemServicoId, StatusAnterior, StatusNovo, Observacao, UsuarioRegistro)
                VALUES (@OrdemServicoId, @StatusAnterior, @StatusNovo, @Observacao, @UsuarioRegistro)";
            
            var parameters = new
            {
                OrdemServicoId = ordemServicoId,
                StatusAnterior = (int?)statusAnterior,
                StatusNovo = (int)statusNovo,
                Observacao = observacao,
                UsuarioRegistro = usuarioId
            };
            
            var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
            return rowsAffected > 0;
        }
        
        // Continuação dos métodos na próxima parte...
    }
}
```

## Continuação na Parte 3

Este documento continua na **IMPLEMENTACAO_OS_PARTE3.md** com:
- Continuação da implementação do repositório
- Implementação dos serviços
- Validações de negócio
- Páginas Blazor
- Componentes de UI