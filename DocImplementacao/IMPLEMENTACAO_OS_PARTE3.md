# Implementação do Módulo de Ordem de Serviço (OS) - Parte 3 - MecTecERP

## Continuação do OrdemServicoRepository.cs

```csharp
        public async Task<bool> UpdateAsync(OrdemServicoUpdateDto os, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var sql = @"
                    UPDATE OrdemServico SET
                        ClienteId = @ClienteId,
                        VeiculoId = @VeiculoId,
                        DataPrevista = @DataPrevista,
                        KmEntrada = @KmEntrada,
                        KmSaida = @KmSaida,
                        Prioridade = @Prioridade,
                        TipoServico = @TipoServico,
                        DefeitoRelatado = @DefeitoRelatado,
                        DefeitoConstatado = @DefeitoConstatado,
                        ServicoExecutado = @ServicoExecutado,
                        ObservacoesInternas = @ObservacoesInternas,
                        ObservacoesCliente = @ObservacoesCliente,
                        ValorMaoObra = @ValorMaoObra,
                        ValorPecas = @ValorPecas,
                        ValorDesconto = @ValorDesconto,
                        ValorTotal = @ValorMaoObra + @ValorPecas - @ValorDesconto,
                        FormaPagamento = @FormaPagamento,
                        StatusPagamento = @StatusPagamento,
                        ResponsavelTecnico = @ResponsavelTecnico,
                        Garantia = @Garantia,
                        DataGarantia = CASE WHEN @Garantia IS NOT NULL THEN DATEADD(DAY, @Garantia, GETDATE()) ELSE NULL END,
                        DataUltimaAtualizacao = GETDATE(),
                        UsuarioUltimaAtualizacao = @UsuarioId
                    WHERE Id = @Id";
                
                var parameters = new
                {
                    os.Id,
                    os.ClienteId,
                    os.VeiculoId,
                    os.DataPrevista,
                    os.KmEntrada,
                    os.KmSaida,
                    Prioridade = (int)os.Prioridade,
                    os.TipoServico,
                    os.DefeitoRelatado,
                    os.DefeitoConstatado,
                    os.ServicoExecutado,
                    os.ObservacoesInternas,
                    os.ObservacoesCliente,
                    os.ValorMaoObra,
                    os.ValorPecas,
                    os.ValorDesconto,
                    os.FormaPagamento,
                    StatusPagamento = (int)os.StatusPagamento,
                    os.ResponsavelTecnico,
                    os.Garantia,
                    UsuarioId = usuarioId
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                
                // Atualizar itens
                await connection.ExecuteAsync("DELETE FROM OSItens WHERE OrdemServicoId = @Id", new { os.Id }, transaction);
                
                foreach (var item in os.Itens)
                {
                    await AddItemInternalAsync(connection, transaction, os.Id, item, usuarioId);
                }
                
                transaction.Commit();
                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public async Task<bool> UpdateStatusAsync(OSStatusUpdateDto statusUpdate, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Obter status atual
                var osAtual = await connection.QueryFirstOrDefaultAsync<OrdemServico>(
                    "SELECT * FROM OrdemServico WHERE Id = @Id", 
                    new { Id = statusUpdate.OrdemServicoId }, transaction);
                
                if (osAtual == null) return false;
                
                // Validar transição de status
                if (!OSHelper.PodeAlterarStatus(osAtual.Status, statusUpdate.NovoStatus))
                    return false;
                
                var sql = @"
                    UPDATE OrdemServico SET
                        Status = @NovoStatus,
                        DataSaida = @DataSaida,
                        KmSaida = @KmSaida,
                        DataUltimaAtualizacao = GETDATE(),
                        UsuarioUltimaAtualizacao = @UsuarioId
                    WHERE Id = @Id";
                
                var parameters = new
                {
                    Id = statusUpdate.OrdemServicoId,
                    NovoStatus = (int)statusUpdate.NovoStatus,
                    statusUpdate.DataSaida,
                    statusUpdate.KmSaida,
                    UsuarioId = usuarioId
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                
                // Adicionar histórico
                await AddHistoricoInternalAsync(connection, transaction, statusUpdate.OrdemServicoId, 
                    osAtual.Status, statusUpdate.NovoStatus, statusUpdate.Observacao, usuarioId);
                
                // Atualizar KM do veículo se necessário
                if (statusUpdate.KmSaida.HasValue)
                {
                    var updateKmSql = @"
                        UPDATE Veiculos SET 
                            KmAtual = @KmSaida,
                            DataUltimaAtualizacao = GETDATE(),
                            UsuarioUltimaAtualizacao = @UsuarioId
                        WHERE Id = (SELECT VeiculoId FROM OrdemServico WHERE Id = @OSId) 
                          AND KmAtual < @KmSaida";
                    
                    await connection.ExecuteAsync(updateKmSql, new
                    {
                        statusUpdate.KmSaida,
                        OSId = statusUpdate.OrdemServicoId,
                        UsuarioId = usuarioId
                    }, transaction);
                }
                
                transaction.Commit();
                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public async Task<bool> CanChangeStatusAsync(int osId, StatusOS novoStatus)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var os = await connection.QueryFirstOrDefaultAsync<OrdemServico>(
                "SELECT * FROM OrdemServico WHERE Id = @Id", new { Id = osId });
            
            return os != null && OSHelper.PodeAlterarStatus(os.Status, novoStatus);
        }
        
        public async Task<IEnumerable<OSItemDto>> GetItensAsync(int ordemServicoId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    i.*,
                    p.Nome as ProdutoNome,
                    s.Nome as ServicoNome
                FROM OSItens i
                LEFT JOIN Produtos p ON i.ProdutoId = p.Id
                LEFT JOIN Servicos s ON i.ServicoId = s.Id
                WHERE i.OrdemServicoId = @OrdemServicoId
                ORDER BY i.Id";
            
            var result = await connection.QueryAsync<OSItemDto>(sql, new { OrdemServicoId = ordemServicoId });
            
            foreach (var item in result)
            {
                item.TipoItemDescricao = item.TipoItem.GetDescription();
                item.ValorSubtotal = item.Quantidade * item.ValorUnitario;
                item.ValorLiquido = item.ValorSubtotal - item.ValorDesconto;
            }
            
            return result;
        }
        
        public async Task<IEnumerable<OSHistoricoDto>> GetHistoricoAsync(int ordemServicoId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    h.*,
                    u.UserName as UsuarioNome
                FROM OSHistorico h
                INNER JOIN AspNetUsers u ON h.UsuarioRegistro = u.Id
                WHERE h.OrdemServicoId = @OrdemServicoId
                ORDER BY h.DataRegistro DESC";
            
            var result = await connection.QueryAsync<OSHistoricoDto>(sql, new { OrdemServicoId = ordemServicoId });
            
            foreach (var hist in result)
            {
                if (hist.StatusAnterior.HasValue)
                    hist.StatusAnteriorDescricao = ((StatusOS)hist.StatusAnterior.Value).GetDescription();
                hist.StatusNovoDescricao = ((StatusOS)hist.StatusNovo).GetDescription();
            }
            
            return result;
        }
        
        public async Task<IEnumerable<OSAnexoDto>> GetAnexosAsync(int ordemServicoId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    a.*,
                    u.UserName as UsuarioNome
                FROM OSAnexos a
                INNER JOIN AspNetUsers u ON a.UsuarioUpload = u.Id
                WHERE a.OrdemServicoId = @OrdemServicoId
                ORDER BY a.DataUpload DESC";
            
            var result = await connection.QueryAsync<OSAnexoDto>(sql, new { OrdemServicoId = ordemServicoId });
            
            foreach (var anexo in result)
            {
                anexo.TamanhoFormatado = FormatarTamanhoArquivo(anexo.TamanhoArquivo);
                anexo.EhImagem = IsImageFile(anexo.TipoArquivo);
                anexo.EhPDF = anexo.TipoArquivo.ToLower() == "application/pdf";
            }
            
            return result;
        }
        
        public async Task<int> GetTotalCountAsync(string? search = null, StatusOS? status = null, 
            int? clienteId = null, int? responsavelId = null, DateTime? dataInicio = null, 
            DateTime? dataFim = null, bool? emAtraso = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT COUNT(1)
                FROM OrdemServico os
                INNER JOIN Clientes c ON os.ClienteId = c.Id
                INNER JOIN Veiculos v ON os.VeiculoId = v.Id
                WHERE os.Ativo = 1
                  AND (@Search IS NULL OR os.Numero LIKE '%' + @Search + '%' 
                       OR c.NomeRazaoSocial LIKE '%' + @Search + '%'
                       OR v.Placa LIKE '%' + @Search + '%')
                  AND (@Status IS NULL OR os.Status = @Status)
                  AND (@ClienteId IS NULL OR os.ClienteId = @ClienteId)
                  AND (@ResponsavelId IS NULL OR os.ResponsavelTecnico = @ResponsavelId)
                  AND (@DataInicio IS NULL OR os.DataEntrada >= @DataInicio)
                  AND (@DataFim IS NULL OR os.DataEntrada <= @DataFim)
                  AND (@EmAtraso IS NULL OR 
                       (@EmAtraso = 1 AND os.DataPrevista IS NOT NULL AND GETDATE() > os.DataPrevista AND os.Status NOT IN (5, 6)) OR
                       (@EmAtraso = 0 AND (os.DataPrevista IS NULL OR GETDATE() <= os.DataPrevista OR os.Status IN (5, 6))))";
            
            var parameters = new
            {
                Search = search,
                Status = (int?)status,
                ClienteId = clienteId,
                ResponsavelId = responsavelId,
                DataInicio = dataInicio,
                DataFim = dataFim,
                EmAtraso = emAtraso
            };
            
            return await connection.QueryFirstAsync<int>(sql, parameters);
        }
        
        // Métodos auxiliares
        private string FormatarTamanhoArquivo(long bytes)
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
        
        private bool IsImageFile(string mimeType)
        {
            return mimeType.StartsWith("image/");
        }
        
        // Implementar outros métodos da interface...
        // (GetByClienteAsync, GetByVeiculoAsync, etc.)
    }
}
```

## Implementação do Serviço

### OrdemServicoService.cs

```csharp
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Shared.DTOs;
using Microsoft.AspNetCore.Http;

namespace MecTecERP.Application.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _repository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly ILogger<OrdemServicoService> _logger;
        
        public OrdemServicoService(
            IOrdemServicoRepository repository,
            IClienteRepository clienteRepository,
            IVeiculoRepository veiculoRepository,
            ILogger<OrdemServicoService> logger)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
            _veiculoRepository = veiculoRepository;
            _logger = logger;
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetAllAsync(int page = 1, int pageSize = 50, 
            string? search = null, StatusOS? status = null, int? clienteId = null, int? responsavelId = null, 
            DateTime? dataInicio = null, DateTime? dataFim = null, bool? emAtraso = null)
        {
            return await _repository.GetAllAsync(page, pageSize, search, status, clienteId, 
                responsavelId, dataInicio, dataFim, emAtraso);
        }
        
        public async Task<OrdemServicoDto?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        
        public async Task<OrdemServicoDto?> GetByNumeroAsync(string numero)
        {
            return await _repository.GetByNumeroAsync(numero);
        }
        
        public async Task<int> CreateAsync(OrdemServicoCreateDto os, int usuarioId)
        {
            // Validações
            if (!await ValidarCriacaoAsync(os))
                throw new InvalidOperationException("Dados inválidos para criação da OS");
            
            try
            {
                var osId = await _repository.CreateAsync(os, usuarioId);
                _logger.LogInformation($"OS criada com sucesso. ID: {osId}, Usuário: {usuarioId}");
                return osId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar OS. Usuário: {usuarioId}");
                throw;
            }
        }
        
        public async Task<bool> UpdateAsync(OrdemServicoUpdateDto os, int usuarioId)
        {
            // Validações
            if (!await ValidarAtualizacaoAsync(os))
                throw new InvalidOperationException("Dados inválidos para atualização da OS");
            
            try
            {
                var result = await _repository.UpdateAsync(os, usuarioId);
                _logger.LogInformation($"OS atualizada. ID: {os.Id}, Usuário: {usuarioId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar OS {os.Id}. Usuário: {usuarioId}");
                throw;
            }
        }
        
        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            var os = await _repository.GetEntityByIdAsync(id);
            if (os == null) return false;
            
            // Só permite exclusão se estiver em status Aberta
            if (os.Status != StatusOS.Aberta)
                throw new InvalidOperationException("Só é possível excluir OS em status 'Aberta'");
            
            try
            {
                var result = await _repository.DeleteAsync(id);
                _logger.LogInformation($"OS excluída. ID: {id}, Usuário: {usuarioId}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao excluir OS {id}. Usuário: {usuarioId}");
                throw;
            }
        }
        
        public async Task<bool> IniciarAndamentoAsync(int osId, int usuarioId, string? observacao = null)
        {
            var statusUpdate = new OSStatusUpdateDto
            {
                OrdemServicoId = osId,
                NovoStatus = StatusOS.EmAndamento,
                Observacao = observacao ?? "OS iniciada"
            };
            
            return await _repository.UpdateStatusAsync(statusUpdate, usuarioId);
        }
        
        public async Task<bool> ColocarEmEsperaAsync(int osId, int usuarioId, string? observacao = null)
        {
            var statusUpdate = new OSStatusUpdateDto
            {
                OrdemServicoId = osId,
                NovoStatus = StatusOS.EmEspera,
                Observacao = observacao ?? "OS colocada em espera"
            };
            
            return await _repository.UpdateStatusAsync(statusUpdate, usuarioId);
        }
        
        public async Task<bool> FinalizarAsync(int osId, int usuarioId, string? observacao = null, 
            DateTime? dataSaida = null, int? kmSaida = null)
        {
            if (!await ValidarFinalizacaoAsync(osId))
                throw new InvalidOperationException("OS não pode ser finalizada no estado atual");
            
            var statusUpdate = new OSStatusUpdateDto
            {
                OrdemServicoId = osId,
                NovoStatus = StatusOS.Finalizada,
                Observacao = observacao ?? "OS finalizada",
                DataSaida = dataSaida,
                KmSaida = kmSaida
            };
            
            return await _repository.UpdateStatusAsync(statusUpdate, usuarioId);
        }
        
        public async Task<bool> EntregarAsync(int osId, int usuarioId, string? observacao = null)
        {
            if (!await ValidarEntregaAsync(osId))
                throw new InvalidOperationException("OS não pode ser entregue no estado atual");
            
            var statusUpdate = new OSStatusUpdateDto
            {
                OrdemServicoId = osId,
                NovoStatus = StatusOS.Entregue,
                Observacao = observacao ?? "OS entregue ao cliente"
            };
            
            return await _repository.UpdateStatusAsync(statusUpdate, usuarioId);
        }
        
        public async Task<bool> CancelarAsync(int osId, int usuarioId, string observacao)
        {
            if (string.IsNullOrWhiteSpace(observacao))
                throw new ArgumentException("Observação é obrigatória para cancelamento");
            
            var statusUpdate = new OSStatusUpdateDto
            {
                OrdemServicoId = osId,
                NovoStatus = StatusOS.Cancelada,
                Observacao = observacao
            };
            
            return await _repository.UpdateStatusAsync(statusUpdate, usuarioId);
        }
        
        public async Task<bool> ValidarCriacaoAsync(OrdemServicoCreateDto os)
        {
            // Verificar se cliente existe e está ativo
            var cliente = await _clienteRepository.GetByIdAsync(os.ClienteId);
            if (cliente == null || !cliente.Ativo)
                return false;
            
            // Verificar se veículo existe, está ativo e pertence ao cliente
            var veiculo = await _veiculoRepository.GetByIdAsync(os.VeiculoId);
            if (veiculo == null || !veiculo.Ativo || veiculo.ClienteId != os.ClienteId)
                return false;
            
            // Validar KM (deve ser maior ou igual ao KM atual do veículo)
            if (os.KmEntrada < veiculo.KmAtual)
                return false;
            
            // Validar data prevista (não pode ser no passado)
            if (os.DataPrevista.HasValue && os.DataPrevista.Value.Date < DateTime.Now.Date)
                return false;
            
            return true;
        }
        
        public async Task<bool> ValidarAtualizacaoAsync(OrdemServicoUpdateDto os)
        {
            // Mesmas validações da criação
            var createDto = new OrdemServicoCreateDto
            {
                ClienteId = os.ClienteId,
                VeiculoId = os.VeiculoId,
                KmEntrada = os.KmEntrada,
                DataPrevista = os.DataPrevista,
                Prioridade = os.Prioridade,
                TipoServico = os.TipoServico,
                DefeitoRelatado = os.DefeitoRelatado,
                ObservacoesInternas = os.ObservacoesInternas,
                ObservacoesCliente = os.ObservacoesCliente,
                ResponsavelTecnico = os.ResponsavelTecnico,
                Garantia = os.Garantia,
                Itens = os.Itens
            };
            
            if (!await ValidarCriacaoAsync(createDto))
                return false;
            
            // Validar KM de saída (se informado, deve ser maior que KM de entrada)
            if (os.KmSaida.HasValue && os.KmSaida.Value < os.KmEntrada)
                return false;
            
            return true;
        }
        
        public async Task<bool> ValidarFinalizacaoAsync(int osId)
        {
            var os = await _repository.GetEntityByIdAsync(osId);
            if (os == null) return false;
            
            // Só pode finalizar se estiver em andamento ou em espera
            return os.Status == StatusOS.EmAndamento || os.Status == StatusOS.EmEspera;
        }
        
        public async Task<bool> ValidarEntregaAsync(int osId)
        {
            var os = await _repository.GetEntityByIdAsync(osId);
            if (os == null) return false;
            
            // Só pode entregar se estiver finalizada
            return os.Status == StatusOS.Finalizada;
        }
        
        public async Task<IEnumerable<OSItemDto>> GetItensAsync(int ordemServicoId)
        {
            return await _repository.GetItensAsync(ordemServicoId);
        }
        
        public async Task<int> AddItemAsync(int ordemServicoId, OSItemCreateDto item, int usuarioId)
        {
            return await _repository.AddItemAsync(ordemServicoId, item, usuarioId);
        }
        
        public async Task<bool> UpdateItemAsync(OSItemUpdateDto item, int usuarioId)
        {
            return await _repository.UpdateItemAsync(item, usuarioId);
        }
        
        public async Task<bool> RemoveItemAsync(int itemId, int usuarioId)
        {
            return await _repository.RemoveItemAsync(itemId);
        }
        
        public async Task<IEnumerable<OSAnexoDto>> GetAnexosAsync(int ordemServicoId)
        {
            return await _repository.GetAnexosAsync(ordemServicoId);
        }
        
        public async Task<int> AddAnexoAsync(int ordemServicoId, IFormFile arquivo, string? descricao, int usuarioId)
        {
            // Validações do arquivo
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido");
            
            // Validar tamanho (máximo 10MB)
            if (arquivo.Length > 10 * 1024 * 1024)
                throw new ArgumentException("Arquivo muito grande (máximo 10MB)");
            
            // Validar tipo
            var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf", "text/plain" };
            if (!tiposPermitidos.Contains(arquivo.ContentType))
                throw new ArgumentException("Tipo de arquivo não permitido");
            
            // Salvar arquivo
            var nomeArquivo = $"{Guid.NewGuid()}_{arquivo.FileName}";
            var caminhoArquivo = Path.Combine("uploads", "os", nomeArquivo);
            
            Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo));
            
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            
            return await _repository.AddAnexoAsync(ordemServicoId, arquivo.FileName, caminhoArquivo, 
                arquivo.ContentType, arquivo.Length, descricao, usuarioId);
        }
        
        public async Task<bool> RemoveAnexoAsync(int anexoId, int usuarioId)
        {
            return await _repository.RemoveAnexoAsync(anexoId);
        }
        
        public async Task<int> GetTotalCountAsync(string? search = null, StatusOS? status = null, 
            int? clienteId = null, int? responsavelId = null, DateTime? dataInicio = null, 
            DateTime? dataFim = null, bool? emAtraso = null)
        {
            return await _repository.GetTotalCountAsync(search, status, clienteId, responsavelId, 
                dataInicio, dataFim, emAtraso);
        }
        
        public async Task<IEnumerable<OrdemServicoSearchDto>> SearchAsync(string term)
        {
            return await _repository.SearchAsync(term);
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetByClienteAsync(int clienteId)
        {
            return await _repository.GetByClienteAsync(clienteId);
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetByVeiculoAsync(int veiculoId)
        {
            return await _repository.GetByVeiculoAsync(veiculoId);
        }
        
        public async Task<OSRelatorioDto> GetRelatorioAsync(DateTime dataInicio, DateTime dataFim, 
            int? clienteId = null, int? responsavelId = null, StatusOS? status = null)
        {
            return await _repository.GetRelatorioAsync(dataInicio, dataFim, clienteId, responsavelId, status);
        }
        
        public async Task<byte[]> GerarRelatorioOSAsync(int osId)
        {
            // Implementar geração de relatório PDF da OS
            throw new NotImplementedException();
        }
        
        public async Task<byte[]> GerarRelatorioPeriodicoAsync(DateTime dataInicio, DateTime dataFim, string formato = "PDF")
        {
            // Implementar geração de relatório periódico
            throw new NotImplementedException();
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetOSVencendoAsync(int dias = 3)
        {
            return await _repository.GetAllAsync(1, 100, null, null, null, null, null, 
                DateTime.Now.AddDays(dias), true);
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetOSEmAtrasoAsync()
        {
            return await _repository.GetEmAtrasoAsync();
        }
        
        public async Task<IEnumerable<OrdemServicoListDto>> GetOSComGarantiaVencendoAsync(int dias = 30)
        {
            return await _repository.GetComGarantiaVigenteAsync();
        }
    }
}
```

## Páginas Blazor

### OrdemServico/Index.razor

```razor
@page "/ordem-servico"
@using MecTecERP.Shared.DTOs
@using MecTecERP.Domain.Enums
@inject IOrdemServicoService OSService
@inject IJSRuntime JSRuntime
@inject NavigationManager Navigation

<PageTitle>Ordens de Serviço - MecTecERP</PageTitle>

<div class="d-flex justify-content-between align-items-center mb-4">
    <div>
        <h3 class="mb-0">
            <i class="fas fa-clipboard-list me-2"></i>
            Ordens de Serviço
        </h3>
        <small class="text-muted">Gerenciamento de ordens de serviço</small>
    </div>
    <button class="btn btn-primary" @onclick="NovaOS">
        <i class="fas fa-plus me-2"></i>
        Nova OS
    </button>
</div>

<!-- Filtros -->
<div class="card mb-4">
    <div class="card-body">
        <div class="row g-3">
            <div class="col-md-3">
                <label class="form-label">Buscar</label>
                <input type="text" class="form-control" @bind="filtros.Search" @onkeypress="OnKeyPress" 
                       placeholder="Número, cliente, placa..." />
            </div>
            <div class="col-md-2">
                <label class="form-label">Status</label>
                <select class="form-select" @bind="filtros.Status">
                    <option value="">Todos</option>
                    @foreach (StatusOS status in Enum.GetValues<StatusOS>())
                    {
                        <option value="@((int)status)">@status.GetDescription()</option>
                    }
                </select>
            </div>
            <div class="col-md-2">
                <label class="form-label">Data Início</label>
                <input type="date" class="form-control" @bind="filtros.DataInicio" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Data Fim</label>
                <input type="date" class="form-control" @bind="filtros.DataFim" />
            </div>
            <div class="col-md-2">
                <label class="form-label">Em Atraso</label>
                <select class="form-select" @bind="filtros.EmAtraso">
                    <option value="">Todos</option>
                    <option value="true">Sim</option>
                    <option value="false">Não</option>
                </select>
            </div>
            <div class="col-md-1 d-flex align-items-end">
                <button class="btn btn-outline-primary" @onclick="Pesquisar">
                    <i class="fas fa-search"></i>
                </button>
            </div>
        </div>
    </div>
</div>

<!-- Lista de OS -->
@if (loading)
{
    <div class="text-center py-5">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Carregando...</span>
        </div>
    </div>
}
else if (ordemServicos?.Any() == true)
{
    <div class="card">
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-hover mb-0">
                    <thead class="table-light">
                        <tr>
                            <th>Número</th>
                            <th>Cliente</th>
                            <th>Veículo</th>
                            <th>Entrada</th>
                            <th>Previsão</th>
                            <th>Status</th>
                            <th>Prioridade</th>
                            <th>Valor</th>
                            <th>Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var os in ordemServicos)
                        {
                            <tr class="@(os.EmAtraso ? "table-warning" : "")">
                                <td>
                                    <strong>@os.Numero</strong>
                                    @if (os.EmAtraso)
                                    {
                                        <i class="fas fa-exclamation-triangle text-warning ms-1" title="Em atraso"></i>
                                    }
                                </td>
                                <td>@os.ClienteNome</td>
                                <td>
                                    <div>@os.VeiculoDescricao</div>
                                    <small class="text-muted">@os.VeiculoPlaca</small>
                                </td>
                                <td>@os.DataEntrada.ToString("dd/MM/yyyy")</td>
                                <td>
                                    @if (os.DataPrevista.HasValue)
                                    {
                                        @os.DataPrevista.Value.ToString("dd/MM/yyyy")
                                    }
                                    else
                                    {
                                        <span class="text-muted">-</span>
                                    }
                                </td>
                                <td>
                                    <span class="badge" style="background-color: @os.CorStatus">
                                        <i class="@os.IconeStatus me-1"></i>
                                        @os.StatusDescricao
                                    </span>
                                </td>
                                <td>
                                    <span class="badge" style="background-color: @os.CorPrioridade">
                                        @os.PrioridadeDescricao
                                    </span>
                                </td>
                                <td>@os.ValorTotal.ToString("C")</td>
                                <td>
                                    <div class="btn-group btn-group-sm">
                                        <button class="btn btn-outline-primary" @onclick="() => VisualizarOS(os.Id)" title="Visualizar">
                                            <i class="fas fa-eye"></i>
                                        </button>
                                        @if (os.Status == StatusOS.Aberta || os.Status == StatusOS.EmAndamento)
                                        {
                                            <button class="btn btn-outline-secondary" @onclick="() => EditarOS(os.Id)" title="Editar">
                                                <i class="fas fa-edit"></i>
                                            </button>
                                        }
                                        <div class="btn-group btn-group-sm">
                                            <button class="btn btn-outline-info dropdown-toggle" data-bs-toggle="dropdown" title="Ações">
                                                <i class="fas fa-cog"></i>
                                            </button>
                                            <ul class="dropdown-menu">
                                                @if (os.Status == StatusOS.Aberta)
                                                {
                                                    <li><a class="dropdown-item" @onclick="() => IniciarAndamento(os.Id)">
                                                        <i class="fas fa-play me-2"></i>Iniciar
                                                    </a></li>
                                                }
                                                @if (os.Status == StatusOS.EmAndamento)
                                                {
                                                    <li><a class="dropdown-item" @onclick="() => ColocarEmEspera(os.Id)">
                                                        <i class="fas fa-pause me-2"></i>Em Espera
                                                    </a></li>
                                                    <li><a class="dropdown-item" @onclick="() => FinalizarOS(os.Id)">
                                                        <i class="fas fa-check me-2"></i>Finalizar
                                                    </a></li>
                                                }
                                                @if (os.Status == StatusOS.Finalizada)
                                                {
                                                    <li><a class="dropdown-item" @onclick="() => EntregarOS(os.Id)">
                                                        <i class="fas fa-handshake me-2"></i>Entregar
                                                    </a></li>
                                                }
                                                @if (os.Status != StatusOS.Entregue && os.Status != StatusOS.Cancelada)
                                                {
                                                    <li><hr class="dropdown-divider"></li>
                                                    <li><a class="dropdown-item text-danger" @onclick="() => CancelarOS(os.Id)">
                                                        <i class="fas fa-times me-2"></i>Cancelar
                                                    </a></li>
                                                }
                                            </ul>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    
    <!-- Paginação -->
    @if (totalPaginas > 1)
    {
        <nav class="mt-4">
            <ul class="pagination justify-content-center">
                <li class="page-item @(paginaAtual == 1 ? "disabled" : "")">
                    <button class="page-link" @onclick="() => IrParaPagina(paginaAtual - 1)">
                        <i class="fas fa-chevron-left"></i>
                    </button>
                </li>
                
                @for (int i = Math.Max(1, paginaAtual - 2); i <= Math.Min(totalPaginas, paginaAtual + 2); i++)
                {
                    <li class="page-item @(i == paginaAtual ? "active" : "")">
                        <button class="page-link" @onclick="() => IrParaPagina(i)">@i</button>
                    </li>
                }
                
                <li class="page-item @(paginaAtual == totalPaginas ? "disabled" : "")">
                    <button class="page-link" @onclick="() => IrParaPagina(paginaAtual + 1)">
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </li>
            </ul>
        </nav>
    }
}
else
{
    <div class="text-center py-5">
        <i class="fas fa-clipboard-list fa-3x text-muted mb-3"></i>
        <h5 class="text-muted">Nenhuma ordem de serviço encontrada</h5>
        <p class="text-muted">Clique em "Nova OS" para criar a primeira ordem de serviço.</p>
    </div>
}

@code {
    private List<OrdemServicoListDto>? ordemServicos;
    private bool loading = true;
    private int paginaAtual = 1;
    private int totalPaginas = 1;
    private int totalRegistros = 0;
    private const int itensPorPagina = 20;
    
    private FiltrosOS filtros = new();
    
    protected override async Task OnInitializedAsync()
    {
        await CarregarOrdemServicos();
    }
    
    private async Task CarregarOrdemServicos()
    {
        loading = true;
        
        try
        {
            var statusFiltro = !string.IsNullOrEmpty(filtros.Status) ? (StatusOS?)int.Parse(filtros.Status) : null;
            var emAtrasoFiltro = !string.IsNullOrEmpty(filtros.EmAtraso) ? bool.Parse(filtros.EmAtraso) : (bool?)null;
            
            ordemServicos = (await OSService.GetAllAsync(
                paginaAtual, itensPorPagina, filtros.Search, statusFiltro, 
                null, null, filtros.DataInicio, filtros.DataFim, emAtrasoFiltro)).ToList();
            
            totalRegistros = await OSService.GetTotalCountAsync(
                filtros.Search, statusFiltro, null, null, 
                filtros.DataInicio, filtros.DataFim, emAtrasoFiltro);
            
            totalPaginas = (int)Math.Ceiling((double)totalRegistros / itensPorPagina);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar ordens de serviço: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }
    
    private async Task Pesquisar()
    {
        paginaAtual = 1;
        await CarregarOrdemServicos();
    }
    
    private async Task OnKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Pesquisar();
        }
    }
    
    private async Task IrParaPagina(int pagina)
    {
        if (pagina >= 1 && pagina <= totalPaginas)
        {
            paginaAtual = pagina;
            await CarregarOrdemServicos();
        }
    }
    
    private void NovaOS()
    {
        Navigation.NavigateTo("/ordem-servico/nova");
    }
    
    private void VisualizarOS(int id)
    {
        Navigation.NavigateTo($"/ordem-servico/{id}");
    }
    
    private void EditarOS(int id)
    {
        Navigation.NavigateTo($"/ordem-servico/{id}/editar");
    }
    
    private async Task IniciarAndamento(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja iniciar esta OS?"))
        {
            try
            {
                await OSService.IniciarAndamentoAsync(id, 1); // TODO: Obter usuário logado
                await CarregarOrdemServicos();
                await JSRuntime.InvokeVoidAsync("alert", "OS iniciada com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro: {ex.Message}");
            }
        }
    }
    
    private async Task ColocarEmEspera(int id)
    {
        var observacao = await JSRuntime.InvokeAsync<string>("prompt", "Motivo para colocar em espera:");
        if (!string.IsNullOrEmpty(observacao))
        {
            try
            {
                await OSService.ColocarEmEsperaAsync(id, 1, observacao); // TODO: Obter usuário logado
                await CarregarOrdemServicos();
                await JSRuntime.InvokeVoidAsync("alert", "OS colocada em espera!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro: {ex.Message}");
            }
        }
    }
    
    private async Task FinalizarOS(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja finalizar esta OS?"))
        {
            try
            {
                await OSService.FinalizarAsync(id, 1); // TODO: Obter usuário logado
                await CarregarOrdemServicos();
                await JSRuntime.InvokeVoidAsync("alert", "OS finalizada com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro: {ex.Message}");
            }
        }
    }
    
    private async Task EntregarOS(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Confirma a entrega desta OS ao cliente?"))
        {
            try
            {
                await OSService.EntregarAsync(id, 1); // TODO: Obter usuário logado
                await CarregarOrdemServicos();
                await JSRuntime.InvokeVoidAsync("alert", "OS entregue com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro: {ex.Message}");
            }
        }
    }
    
    private async Task CancelarOS(int id)
    {
        var motivo = await JSRuntime.InvokeAsync<string>("prompt", "Motivo do cancelamento:");
        if (!string.IsNullOrEmpty(motivo))
        {
            try
            {
                await OSService.CancelarAsync(id, 1, motivo); // TODO: Obter usuário logado
                await CarregarOrdemServicos();
                await JSRuntime.InvokeVoidAsync("alert", "OS cancelada!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro: {ex.Message}");
            }
        }
    }
    
    public class FiltrosOS
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? EmAtraso { get; set; }
    }
}
```

## Próximos Passos

1. **Implementar páginas de criação e edição de OS**
2. **Criar componentes para itens e anexos**
3. **Implementar relatórios em PDF**
4. **Adicionar notificações em tempo real**
5. **Implementar dashboard com métricas**
6. **Criar testes unitários**
7. **Implementar auditoria completa**
8. **Adicionar integração com WhatsApp/Email**

## Estrutura de Arquivos Sugerida

```
MecTecERP/
├── Domain/
│   ├── Entities/
│   │   ├── OrdemServico.cs
│   │   ├── OSItem.cs
│   │   ├── OSHistorico.cs
│   │   └── OSAnexo.cs
│   ├── Enums/
│   │   ├── StatusOS.cs
│   │   ├── PrioridadeOS.cs
│   │   ├── StatusPagamentoOS.cs
│   │   └── TipoItemOS.cs
│   ├── Helpers/
│   │   └── OSHelper.cs
│   └── Interfaces/
│       ├── IOrdemServicoRepository.cs
│       └── IOrdemServicoService.cs
├── Infrastructure/
│   └── Repositories/
│       └── OrdemServicoRepository.cs
├── Application/
│   └── Services/
│       └── OrdemServicoService.cs
├── Shared/
│   └── DTOs/
│       └── OrdemServicoDto.cs
└── Web/
    └── Pages/
        └── OrdemServico/
            ├── Index.razor
            ├── Create.razor
            ├── Edit.razor
            └── Details.razor
```

Esta implementação fornece uma base sólida para o módulo de Ordem de Serviço, seguindo as melhores práticas de Clean Architecture, SOLID e padrões modernos de desenvolvimento .NET.