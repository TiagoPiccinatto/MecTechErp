# MecTecERP - Implementação do Módulo de Estoque - Parte 4

## Implementação do Inventário de Estoque

### 1. Implementação do InventarioRepository.cs

```csharp
using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Infrastructure.Interfaces;
using System.Data;
using System.Text;

namespace MecTecERP.Infrastructure.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly IDbConnection _connection;

        public InventarioRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<InventarioEstoque> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT i.*, ii.* 
                FROM InventarioEstoque i
                LEFT JOIN InventarioItens ii ON i.Id = ii.InventarioId
                WHERE i.Id = @Id";

            var inventarioDictionary = new Dictionary<int, InventarioEstoque>();

            var result = await _connection.QueryAsync<InventarioEstoque, InventarioItem, InventarioEstoque>(
                sql,
                (inventario, item) =>
                {
                    if (!inventarioDictionary.TryGetValue(inventario.Id, out var inventarioEntry))
                    {
                        inventarioEntry = inventario;
                        inventarioEntry.Itens = new List<InventarioItem>();
                        inventarioDictionary.Add(inventario.Id, inventarioEntry);
                    }

                    if (item != null)
                    {
                        inventarioEntry.Itens.Add(item);
                    }

                    return inventarioEntry;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return inventarioDictionary.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<InventarioEstoque>> GetAllAsync()
        {
            const string sql = @"
                SELECT * FROM InventarioEstoque 
                ORDER BY DataCriacao DESC";

            return await _connection.QueryAsync<InventarioEstoque>(sql);
        }

        public async Task<(IEnumerable<InventarioEstoque> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string searchTerm = null, StatusInventario? status = null)
        {
            var whereClause = new StringBuilder("WHERE 1=1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                whereClause.Append(" AND (Descricao LIKE @SearchTerm OR Observacoes LIKE @SearchTerm)");
                parameters.Add("SearchTerm", $"%{searchTerm}%");
            }

            if (status.HasValue)
            {
                whereClause.Append(" AND Status = @Status");
                parameters.Add("Status", status.Value);
            }

            // Count total
            var countSql = $"SELECT COUNT(*) FROM InventarioEstoque {whereClause}";
            var totalCount = await _connection.QuerySingleAsync<int>(countSql, parameters);

            // Get paged data
            var offset = (pageNumber - 1) * pageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var sql = $@"
                SELECT * FROM InventarioEstoque 
                {whereClause}
                ORDER BY DataCriacao DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var items = await _connection.QueryAsync<InventarioEstoque>(sql, parameters);

            return (items, totalCount);
        }

        public async Task<int> CreateAsync(InventarioEstoque inventario)
        {
            const string sql = @"
                INSERT INTO InventarioEstoque 
                (Descricao, DataCriacao, DataInicio, DataFim, Status, UsuarioId, Observacoes)
                VALUES 
                (@Descricao, @DataCriacao, @DataInicio, @DataFim, @Status, @UsuarioId, @Observacoes);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.QuerySingleAsync<int>(sql, inventario);
        }

        public async Task<bool> UpdateAsync(InventarioEstoque inventario)
        {
            const string sql = @"
                UPDATE InventarioEstoque SET
                    Descricao = @Descricao,
                    DataInicio = @DataInicio,
                    DataFim = @DataFim,
                    Status = @Status,
                    Observacoes = @Observacoes
                WHERE Id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, inventario);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                // Delete items first
                await _connection.ExecuteAsync(
                    "DELETE FROM InventarioItens WHERE InventarioId = @Id",
                    new { Id = id },
                    transaction);

                // Delete inventory
                var rowsAffected = await _connection.ExecuteAsync(
                    "DELETE FROM InventarioEstoque WHERE Id = @Id",
                    new { Id = id },
                    transaction);

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            const string sql = "SELECT COUNT(1) FROM InventarioEstoque WHERE Id = @Id";
            var count = await _connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, StatusInventario status)
        {
            const string sql = @"
                UPDATE InventarioEstoque SET
                    Status = @Status,
                    DataFim = CASE WHEN @Status = @StatusFinalizado THEN GETDATE() ELSE DataFim END
                WHERE Id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, new 
            { 
                Id = id, 
                Status = status,
                StatusFinalizado = StatusInventario.Finalizado
            });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<InventarioEstoque>> GetByStatusAsync(StatusInventario status)
        {
            const string sql = @"
                SELECT * FROM InventarioEstoque 
                WHERE Status = @Status
                ORDER BY DataCriacao DESC";

            return await _connection.QueryAsync<InventarioEstoque>(sql, new { Status = status });
        }

        public async Task<IEnumerable<InventarioEstoque>> GetByPeriodAsync(DateTime dataInicio, DateTime dataFim)
        {
            const string sql = @"
                SELECT * FROM InventarioEstoque 
                WHERE DataCriacao BETWEEN @DataInicio AND @DataFim
                ORDER BY DataCriacao DESC";

            return await _connection.QueryAsync<InventarioEstoque>(sql, new 
            { 
                DataInicio = dataInicio, 
                DataFim = dataFim 
            });
        }

        // Métodos para InventarioItens
        public async Task<IEnumerable<InventarioItem>> GetItensAsync(int inventarioId)
        {
            const string sql = @"
                SELECT ii.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM InventarioItens ii
                INNER JOIN Produtos p ON ii.ProdutoId = p.Id
                WHERE ii.InventarioId = @InventarioId
                ORDER BY p.Nome";

            return await _connection.QueryAsync<InventarioItem>(sql, new { InventarioId = inventarioId });
        }

        public async Task<int> AddItemAsync(InventarioItem item)
        {
            const string sql = @"
                INSERT INTO InventarioItens 
                (InventarioId, ProdutoId, EstoqueSistema, EstoqueContado, Diferenca, Observacoes)
                VALUES 
                (@InventarioId, @ProdutoId, @EstoqueSistema, @EstoqueContado, @Diferenca, @Observacoes);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _connection.QuerySingleAsync<int>(sql, item);
        }

        public async Task<bool> UpdateItemAsync(InventarioItem item)
        {
            const string sql = @"
                UPDATE InventarioItens SET
                    EstoqueSistema = @EstoqueSistema,
                    EstoqueContado = @EstoqueContado,
                    Diferenca = @Diferenca,
                    Observacoes = @Observacoes
                WHERE Id = @Id";

            var rowsAffected = await _connection.ExecuteAsync(sql, item);
            return rowsAffected > 0;
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            const string sql = "DELETE FROM InventarioItens WHERE Id = @Id";
            var rowsAffected = await _connection.ExecuteAsync(sql, new { Id = itemId });
            return rowsAffected > 0;
        }

        public async Task<bool> ItemExistsAsync(int inventarioId, int produtoId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM InventarioItens 
                WHERE InventarioId = @InventarioId AND ProdutoId = @ProdutoId";

            var count = await _connection.QuerySingleAsync<int>(sql, new 
            { 
                InventarioId = inventarioId, 
                ProdutoId = produtoId 
            });
            return count > 0;
        }

        public async Task<int> GetTotalItensAsync(int inventarioId)
        {
            const string sql = "SELECT COUNT(*) FROM InventarioItens WHERE InventarioId = @InventarioId";
            return await _connection.QuerySingleAsync<int>(sql, new { InventarioId = inventarioId });
        }

        public async Task<decimal> GetTotalDiferencaAsync(int inventarioId)
        {
            const string sql = @"
                SELECT ISNULL(SUM(ABS(Diferenca)), 0) 
                FROM InventarioItens 
                WHERE InventarioId = @InventarioId";

            return await _connection.QuerySingleAsync<decimal>(sql, new { InventarioId = inventarioId });
        }

        public async Task<bool> FinalizarInventarioAsync(int inventarioId)
        {
            using var transaction = _connection.BeginTransaction();
            try
            {
                // Atualizar status do inventário
                await _connection.ExecuteAsync(@"
                    UPDATE InventarioEstoque SET
                        Status = @Status,
                        DataFim = GETDATE()
                    WHERE Id = @Id",
                    new { Id = inventarioId, Status = StatusInventario.Finalizado },
                    transaction);

                // Aplicar ajustes no estoque baseado nas diferenças
                await _connection.ExecuteAsync(@"
                    INSERT INTO MovimentacaoEstoque 
                    (ProdutoId, TipoMovimentacao, Quantidade, DataMovimentacao, Observacoes, DocumentoReferencia)
                    SELECT 
                        ii.ProdutoId,
                        CASE WHEN ii.Diferenca > 0 THEN @TipoEntrada ELSE @TipoSaida END,
                        ABS(ii.Diferenca),
                        GETDATE(),
                        'Ajuste por inventário: ' + ISNULL(ii.Observacoes, ''),
                        'INV-' + CAST(@InventarioId as varchar(10))
                    FROM InventarioItens ii
                    WHERE ii.InventarioId = @InventarioId AND ii.Diferenca != 0",
                    new 
                    { 
                        InventarioId = inventarioId,
                        TipoEntrada = TipoMovimentacao.Entrada,
                        TipoSaida = TipoMovimentacao.Saida
                    },
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
```

### 2. Implementação do InventarioService.cs

```csharp
using MecTecERP.Domain.DTOs;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Interfaces;

namespace MecTecERP.Application.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly IInventarioRepository _inventarioRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstoqueService _estoqueService;

        public InventarioService(
            IInventarioRepository inventarioRepository,
            IProdutoRepository produtoRepository,
            IEstoqueService estoqueService)
        {
            _inventarioRepository = inventarioRepository;
            _produtoRepository = produtoRepository;
            _estoqueService = estoqueService;
        }

        public async Task<InventarioDto> GetByIdAsync(int id)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return null;

            return MapToDto(inventario);
        }

        public async Task<IEnumerable<InventarioListDto>> GetAllAsync()
        {
            var inventarios = await _inventarioRepository.GetAllAsync();
            return inventarios.Select(MapToListDto);
        }

        public async Task<(IEnumerable<InventarioListDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string searchTerm = null, StatusInventario? status = null)
        {
            var (items, totalCount) = await _inventarioRepository.GetPagedAsync(
                pageNumber, pageSize, searchTerm, status);

            var dtos = items.Select(MapToListDto);
            return (dtos, totalCount);
        }

        public async Task<int> CreateAsync(InventarioCreateDto dto)
        {
            ValidateCreate(dto);

            var inventario = new InventarioEstoque
            {
                Descricao = dto.Descricao,
                DataCriacao = DateTime.Now,
                DataInicio = dto.DataInicio,
                Status = StatusInventario.Planejado,
                UsuarioId = dto.UsuarioId,
                Observacoes = dto.Observacoes
            };

            return await _inventarioRepository.CreateAsync(inventario);
        }

        public async Task<bool> UpdateAsync(int id, InventarioUpdateDto dto)
        {
            ValidateUpdate(dto);

            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return false;

            // Não permite alterar inventário finalizado
            if (inventario.Status == StatusInventario.Finalizado)
                throw new InvalidOperationException("Não é possível alterar um inventário finalizado.");

            inventario.Descricao = dto.Descricao;
            inventario.DataInicio = dto.DataInicio;
            inventario.Observacoes = dto.Observacoes;

            return await _inventarioRepository.UpdateAsync(inventario);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return false;

            // Não permite excluir inventário em andamento ou finalizado
            if (inventario.Status != StatusInventario.Planejado)
                throw new InvalidOperationException("Só é possível excluir inventários com status 'Planejado'.");

            return await _inventarioRepository.DeleteAsync(id);
        }

        public async Task<bool> IniciarInventarioAsync(int id)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return false;

            if (inventario.Status != StatusInventario.Planejado)
                throw new InvalidOperationException("Só é possível iniciar inventários com status 'Planejado'.");

            return await _inventarioRepository.UpdateStatusAsync(id, StatusInventario.EmAndamento);
        }

        public async Task<bool> FinalizarInventarioAsync(int id)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return false;

            if (inventario.Status != StatusInventario.EmAndamento)
                throw new InvalidOperationException("Só é possível finalizar inventários com status 'Em Andamento'.");

            return await _inventarioRepository.FinalizarInventarioAsync(id);
        }

        public async Task<bool> CancelarInventarioAsync(int id)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(id);
            if (inventario == null) return false;

            if (inventario.Status == StatusInventario.Finalizado)
                throw new InvalidOperationException("Não é possível cancelar um inventário finalizado.");

            return await _inventarioRepository.UpdateStatusAsync(id, StatusInventario.Cancelado);
        }

        // Métodos para itens do inventário
        public async Task<IEnumerable<InventarioItemDto>> GetItensAsync(int inventarioId)
        {
            var itens = await _inventarioRepository.GetItensAsync(inventarioId);
            return itens.Select(MapItemToDto);
        }

        public async Task<int> AddItemAsync(InventarioItemCreateDto dto)
        {
            ValidateItemCreate(dto);

            // Verificar se o produto já existe no inventário
            var exists = await _inventarioRepository.ItemExistsAsync(dto.InventarioId, dto.ProdutoId);
            if (exists)
                throw new InvalidOperationException("Este produto já foi adicionado ao inventário.");

            // Obter estoque atual do sistema
            var estoqueAtual = await _estoqueService.GetEstoqueAtualAsync(dto.ProdutoId);

            var item = new InventarioItem
            {
                InventarioId = dto.InventarioId,
                ProdutoId = dto.ProdutoId,
                EstoqueSistema = estoqueAtual,
                EstoqueContado = dto.EstoqueContado,
                Diferenca = dto.EstoqueContado - estoqueAtual,
                Observacoes = dto.Observacoes
            };

            return await _inventarioRepository.AddItemAsync(item);
        }

        public async Task<bool> UpdateItemAsync(int itemId, InventarioItemUpdateDto dto)
        {
            ValidateItemUpdate(dto);

            var item = new InventarioItem
            {
                Id = itemId,
                EstoqueContado = dto.EstoqueContado,
                Diferenca = dto.EstoqueContado - dto.EstoqueSistema,
                Observacoes = dto.Observacoes
            };

            return await _inventarioRepository.UpdateItemAsync(item);
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            return await _inventarioRepository.RemoveItemAsync(itemId);
        }

        public async Task<InventarioResumoDto> GetResumoAsync(int inventarioId)
        {
            var inventario = await _inventarioRepository.GetByIdAsync(inventarioId);
            if (inventario == null) return null;

            var totalItens = await _inventarioRepository.GetTotalItensAsync(inventarioId);
            var totalDiferenca = await _inventarioRepository.GetTotalDiferencaAsync(inventarioId);

            return new InventarioResumoDto
            {
                InventarioId = inventarioId,
                Descricao = inventario.Descricao,
                Status = inventario.Status,
                DataCriacao = inventario.DataCriacao,
                DataInicio = inventario.DataInicio,
                DataFim = inventario.DataFim,
                TotalItens = totalItens,
                TotalDiferenca = totalDiferenca
            };
        }

        // Métodos de validação
        private void ValidateCreate(InventarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição é obrigatória.");

            if (dto.DataInicio < DateTime.Today)
                throw new ArgumentException("Data de início não pode ser anterior à data atual.");
        }

        private void ValidateUpdate(InventarioUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição é obrigatória.");
        }

        private void ValidateItemCreate(InventarioItemCreateDto dto)
        {
            if (dto.InventarioId <= 0)
                throw new ArgumentException("ID do inventário é obrigatório.");

            if (dto.ProdutoId <= 0)
                throw new ArgumentException("ID do produto é obrigatório.");

            if (dto.EstoqueContado < 0)
                throw new ArgumentException("Estoque contado não pode ser negativo.");
        }

        private void ValidateItemUpdate(InventarioItemUpdateDto dto)
        {
            if (dto.EstoqueContado < 0)
                throw new ArgumentException("Estoque contado não pode ser negativo.");
        }

        // Métodos de mapeamento
        private InventarioDto MapToDto(InventarioEstoque inventario)
        {
            return new InventarioDto
            {
                Id = inventario.Id,
                Descricao = inventario.Descricao,
                DataCriacao = inventario.DataCriacao,
                DataInicio = inventario.DataInicio,
                DataFim = inventario.DataFim,
                Status = inventario.Status,
                UsuarioId = inventario.UsuarioId,
                Observacoes = inventario.Observacoes,
                Itens = inventario.Itens?.Select(MapItemToDto).ToList() ?? new List<InventarioItemDto>()
            };
        }

        private InventarioListDto MapToListDto(InventarioEstoque inventario)
        {
            return new InventarioListDto
            {
                Id = inventario.Id,
                Descricao = inventario.Descricao,
                DataCriacao = inventario.DataCriacao,
                DataInicio = inventario.DataInicio,
                DataFim = inventario.DataFim,
                Status = inventario.Status,
                StatusDescricao = inventario.Status.ToString()
            };
        }

        private InventarioItemDto MapItemToDto(InventarioItem item)
        {
            return new InventarioItemDto
            {
                Id = item.Id,
                InventarioId = item.InventarioId,
                ProdutoId = item.ProdutoId,
                EstoqueSistema = item.EstoqueSistema,
                EstoqueContado = item.EstoqueContado,
                Diferenca = item.Diferenca,
                Observacoes = item.Observacoes
            };
        }
    }
}
```

### 3. DTOs para Inventário

```csharp
// InventarioDto.cs
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.DTOs
{
    public class InventarioDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public StatusInventario Status { get; set; }
        public int UsuarioId { get; set; }
        public string Observacoes { get; set; }
        public List<InventarioItemDto> Itens { get; set; } = new();
    }

    public class InventarioCreateDto
    {
        public string Descricao { get; set; }
        public DateTime? DataInicio { get; set; }
        public int UsuarioId { get; set; }
        public string Observacoes { get; set; }
    }

    public class InventarioUpdateDto
    {
        public string Descricao { get; set; }
        public DateTime? DataInicio { get; set; }
        public string Observacoes { get; set; }
    }

    public class InventarioListDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public StatusInventario Status { get; set; }
        public string StatusDescricao { get; set; }
    }

    public class InventarioItemDto
    {
        public int Id { get; set; }
        public int InventarioId { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public string ProdutoCodigo { get; set; }
        public decimal EstoqueSistema { get; set; }
        public decimal EstoqueContado { get; set; }
        public decimal Diferenca { get; set; }
        public string Observacoes { get; set; }
    }

    public class InventarioItemCreateDto
    {
        public int InventarioId { get; set; }
        public int ProdutoId { get; set; }
        public decimal EstoqueContado { get; set; }
        public string Observacoes { get; set; }
    }

    public class InventarioItemUpdateDto
    {
        public decimal EstoqueSistema { get; set; }
        public decimal EstoqueContado { get; set; }
        public string Observacoes { get; set; }
    }

    public class InventarioResumoDto
    {
        public int InventarioId { get; set; }
        public string Descricao { get; set; }
        public StatusInventario Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int TotalItens { get; set; }
        public decimal TotalDiferenca { get; set; }
    }
}
```

### 4. Interface do Serviço de Inventário

```csharp
// IInventarioService.cs
using MecTecERP.Domain.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Interfaces
{
    public interface IInventarioService
    {
        Task<InventarioDto> GetByIdAsync(int id);
        Task<IEnumerable<InventarioListDto>> GetAllAsync();
        Task<(IEnumerable<InventarioListDto> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string searchTerm = null, StatusInventario? status = null);
        Task<int> CreateAsync(InventarioCreateDto dto);
        Task<bool> UpdateAsync(int id, InventarioUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> IniciarInventarioAsync(int id);
        Task<bool> FinalizarInventarioAsync(int id);
        Task<bool> CancelarInventarioAsync(int id);
        
        // Métodos para itens
        Task<IEnumerable<InventarioItemDto>> GetItensAsync(int inventarioId);
        Task<int> AddItemAsync(InventarioItemCreateDto dto);
        Task<bool> UpdateItemAsync(int itemId, InventarioItemUpdateDto dto);
        Task<bool> RemoveItemAsync(int itemId);
        Task<InventarioResumoDto> GetResumoAsync(int inventarioId);
    }
}
```

### Próximos Passos

1. **Implementação das páginas Blazor** para inventário
2. **Componentes de interface** para gestão de inventário
3. **Relatórios de inventário** e divergências
4. **Implementação de alertas** e notificações
5. **Testes unitários** e de integração
6. **Documentação** de uso do módulo