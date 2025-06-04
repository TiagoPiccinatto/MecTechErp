# Implementação do Módulo de Estoque - Parte 3
# Continuação dos Repositórios e Serviços - MecTecERP

## 6.3 ProdutoRepository.cs (Continuação)

```csharp
        // Continuação da classe ProdutoRepository
        
        public async Task<IEnumerable<Produto>> GetProdutosBaixoEstoqueAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Ativo = 1 AND p.ControlaEstoque = 1 
                  AND p.EstoqueAtual <= p.EstoqueMinimo
                ORDER BY p.EstoqueAtual ASC";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<IEnumerable<Produto>> GetProdutosCriticosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Ativo = 1 AND p.ControlaEstoque = 1 
                  AND p.EstoqueAtual <= (p.EstoqueMinimo * 0.5)
                ORDER BY p.EstoqueAtual ASC";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<IEnumerable<Produto>> GetProdutosSemEstoqueAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Ativo = 1 AND p.ControlaEstoque = 1 
                  AND p.EstoqueAtual <= 0
                ORDER BY p.Nome";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.CategoriaId = @CategoriaId AND p.Ativo = 1
                ORDER BY p.Nome";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { CategoriaId = categoriaId },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<IEnumerable<Produto>> GetProdutosPorFornecedorAsync(int fornecedorId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.FornecedorId = @FornecedorId AND p.Ativo = 1
                ORDER BY p.Nome";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { FornecedorId = fornecedorId },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<IEnumerable<Produto>> SearchAsync(string termo)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Ativo = 1 AND (
                    p.Codigo LIKE @Termo OR
                    p.Nome LIKE @Termo OR
                    p.CodigoBarras LIKE @Termo OR
                    p.Descricao LIKE @Termo
                )
                ORDER BY p.Nome";
                
            return await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { Termo = $"%{termo}%" },
                splitOn: "CategoriaNome,FornecedorNome");
        }
        
        public async Task<decimal> GetValorTotalEstoqueAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT ISNULL(SUM(EstoqueAtual * CustoUnitario), 0)
                FROM Produtos
                WHERE Ativo = 1 AND ControlaEstoque = 1";
                
            return await connection.QuerySingleAsync<decimal>(sql);
        }
        
        public async Task<int> GetTotalProdutosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Produtos WHERE Ativo = 1";
            
            return await connection.QuerySingleAsync<int>(sql);
        }
        
        public async Task<int> GetTotalProdutosBaixoEstoqueAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT COUNT(*)
                FROM Produtos
                WHERE Ativo = 1 AND ControlaEstoque = 1 
                  AND EstoqueAtual <= EstoqueMinimo";
                
            return await connection.QuerySingleAsync<int>(sql);
        }
        
        public async Task<int> GetTotalProdutosSemEstoqueAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT COUNT(*)
                FROM Produtos
                WHERE Ativo = 1 AND ControlaEstoque = 1 
                  AND EstoqueAtual <= 0";
                
            return await connection.QuerySingleAsync<int>(sql);
        }
    }
}
```

## 6.4 MovimentacaoEstoqueRepository.cs

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Application.DTOs;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class MovimentacaoEstoqueRepository : IMovimentacaoEstoqueRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        
        public MovimentacaoEstoqueRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                splitOn: "ProdutoNome");
        }
        
        public async Task<MovimentacaoEstoque> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                WHERE m.Id = @Id";
                
            var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                new { Id = id },
                splitOn: "ProdutoNome");
                
            return result.FirstOrDefault();
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                WHERE m.ProdutoId = @ProdutoId
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                new { ProdutoId = produtoId },
                splitOn: "ProdutoNome");
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetByDocumentoAsync(string documentoTipo, int documentoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                WHERE m.DocumentoTipo = @DocumentoTipo AND m.DocumentoId = @DocumentoId
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                new { DocumentoTipo = documentoTipo, DocumentoId = documentoId },
                splitOn: "ProdutoNome");
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                WHERE m.DataMovimentacao >= @DataInicio AND m.DataMovimentacao <= @DataFim
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                new { DataInicio = dataInicio, DataFim = dataFim },
                splitOn: "ProdutoNome");
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetPagedAsync(int page, int pageSize, int? produtoId = null, 
            TipoMovimentacao? tipo = null, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (produtoId.HasValue)
            {
                whereClause += " AND m.ProdutoId = @ProdutoId";
                parameters.Add("ProdutoId", produtoId.Value);
            }
            
            if (tipo.HasValue)
            {
                whereClause += " AND m.TipoMovimentacao = @Tipo";
                parameters.Add("Tipo", tipo.Value);
            }
            
            if (dataInicio.HasValue)
            {
                whereClause += " AND m.DataMovimentacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                whereClause += " AND m.DataMovimentacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            var offset = (page - 1) * pageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);
            
            var sql = $@"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                {whereClause}
                ORDER BY m.DataMovimentacao DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                parameters,
                splitOn: "ProdutoNome");
        }
        
        public async Task<int> GetTotalCountAsync(int? produtoId = null, TipoMovimentacao? tipo = null, 
            DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (produtoId.HasValue)
            {
                whereClause += " AND m.ProdutoId = @ProdutoId";
                parameters.Add("ProdutoId", produtoId.Value);
            }
            
            if (tipo.HasValue)
            {
                whereClause += " AND m.TipoMovimentacao = @Tipo";
                parameters.Add("Tipo", tipo.Value);
            }
            
            if (dataInicio.HasValue)
            {
                whereClause += " AND m.DataMovimentacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                whereClause += " AND m.DataMovimentacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            var sql = $@"
                SELECT COUNT(*)
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                {whereClause}";
                
            return await connection.QuerySingleAsync<int>(sql, parameters);
        }
        
        public async Task<int> CreateAsync(MovimentacaoEstoque movimentacao)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                INSERT INTO MovimentacaoEstoque (
                    ProdutoId, TipoMovimentacao, Quantidade, ValorUnitario, ValorTotal,
                    EstoqueAnterior, EstoquePosterior, DocumentoTipo, DocumentoId,
                    NumeroDocumento, Observacoes, DataMovimentacao, UsuarioId
                )
                VALUES (
                    @ProdutoId, @TipoMovimentacao, @Quantidade, @ValorUnitario, @ValorTotal,
                    @EstoqueAnterior, @EstoquePosterior, @DocumentoTipo, @DocumentoId,
                    @NumeroDocumento, @Observacoes, @DataMovimentacao, @UsuarioId
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
                
            return await connection.QuerySingleAsync<int>(sql, movimentacao);
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "DELETE FROM MovimentacaoEstoque WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        
        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM MovimentacaoEstoque WHERE Id = @Id";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        
        public async Task<decimal> GetEstoqueAtualAsync(int produtoId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT ISNULL(
                    (SELECT SUM(CASE 
                        WHEN TipoMovimentacao IN (0, 2, 4, 6) THEN Quantidade  -- Entrada, Ajuste Positivo, Devolução Venda, Transferência Entrada
                        ELSE -Quantidade  -- Saída, Ajuste Negativo, Devolução Compra, Transferência Saída
                    END)
                    FROM MovimentacaoEstoque 
                    WHERE ProdutoId = @ProdutoId), 0
                )";
                
            return await connection.QuerySingleAsync<decimal>(sql, new { ProdutoId = produtoId });
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetUltimasMovimentacoesAsync(int limite = 10)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = $@"
                SELECT TOP (@Limite) m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                new { Limite = limite },
                splitOn: "ProdutoNome");
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesPorTipoAsync(TipoMovimentacao tipo, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE m.TipoMovimentacao = @Tipo";
            var parameters = new DynamicParameters();
            parameters.Add("Tipo", tipo);
            
            if (dataInicio.HasValue)
            {
                whereClause += " AND m.DataMovimentacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                whereClause += " AND m.DataMovimentacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            var sql = $@"
                SELECT m.*, p.Nome as ProdutoNome, p.Codigo as ProdutoCodigo
                FROM MovimentacaoEstoque m
                INNER JOIN Produtos p ON m.ProdutoId = p.Id
                {whereClause}
                ORDER BY m.DataMovimentacao DESC";
                
            return await connection.QueryAsync<MovimentacaoEstoque, Produto, MovimentacaoEstoque>(sql,
                (movimentacao, produto) =>
                {
                    movimentacao.Produto = produto;
                    return movimentacao;
                },
                parameters,
                splitOn: "ProdutoNome");
        }
        
        public async Task<decimal> GetTotalEntradasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE TipoMovimentacao IN (0, 2, 4, 6)";
            var parameters = new DynamicParameters();
            
            if (dataInicio.HasValue)
            {
                whereClause += " AND DataMovimentacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                whereClause += " AND DataMovimentacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            var sql = $@"
                SELECT ISNULL(SUM(ValorTotal), 0)
                FROM MovimentacaoEstoque
                {whereClause}";
                
            return await connection.QuerySingleAsync<decimal>(sql, parameters);
        }
        
        public async Task<decimal> GetTotalSaidasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE TipoMovimentacao IN (1, 3, 5, 7)";
            var parameters = new DynamicParameters();
            
            if (dataInicio.HasValue)
            {
                whereClause += " AND DataMovimentacao >= @DataInicio";
                parameters.Add("DataInicio", dataInicio.Value);
            }
            
            if (dataFim.HasValue)
            {
                whereClause += " AND DataMovimentacao <= @DataFim";
                parameters.Add("DataFim", dataFim.Value);
            }
            
            var sql = $@"
                SELECT ISNULL(SUM(ValorTotal), 0)
                FROM MovimentacaoEstoque
                {whereClause}";
                
            return await connection.QuerySingleAsync<decimal>(sql, parameters);
        }
    }
}
```

## 7. Interfaces dos Serviços

### 7.1 ICategoriaService.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetAllAsync();
        Task<CategoriaDto> GetByIdAsync(int id);
        Task<IEnumerable<CategoriaListDto>> GetPagedAsync(int page, int pageSize, string search = null, bool? ativo = null);
        Task<int> GetTotalCountAsync(string search = null, bool? ativo = null);
        Task<int> CreateAsync(CategoriaCreateDto dto, int usuarioId);
        Task<bool> UpdateAsync(int id, CategoriaUpdateDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CategoriaDto>> GetAtivasAsync();
        Task<bool> ValidateCreateAsync(CategoriaCreateDto dto);
        Task<bool> ValidateUpdateAsync(int id, CategoriaUpdateDto dto);
        Task<bool> ValidateDeleteAsync(int id);
    }
}
```

### 7.2 IProdutoService.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDto>> GetAllAsync();
        Task<ProdutoDto> GetByIdAsync(int id);
        Task<ProdutoDto> GetByCodigoAsync(string codigo);
        Task<ProdutoDto> GetByCodigoBarrasAsync(string codigoBarras);
        Task<IEnumerable<ProdutoListDto>> GetPagedAsync(ProdutoSearchDto search);
        Task<int> GetTotalCountAsync(ProdutoSearchDto search);
        Task<int> CreateAsync(ProdutoCreateDto dto, int usuarioId);
        Task<bool> UpdateAsync(int id, ProdutoUpdateDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ProdutoDto>> GetProdutosBaixoEstoqueAsync();
        Task<IEnumerable<ProdutoDto>> GetProdutosCriticosAsync();
        Task<IEnumerable<ProdutoDto>> GetProdutosSemEstoqueAsync();
        Task<IEnumerable<ProdutoDto>> GetProdutosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<ProdutoDto>> GetProdutosPorFornecedorAsync(int fornecedorId);
        Task<IEnumerable<ProdutoDto>> SearchAsync(string termo);
        Task<decimal> GetValorTotalEstoqueAsync();
        Task<int> GetTotalProdutosAsync();
        Task<int> GetTotalProdutosBaixoEstoqueAsync();
        Task<int> GetTotalProdutosSemEstoqueAsync();
        Task<bool> ValidateCreateAsync(ProdutoCreateDto dto);
        Task<bool> ValidateUpdateAsync(int id, ProdutoUpdateDto dto);
        Task<bool> ValidateDeleteAsync(int id);
        Task<string> GerarCodigoAsync();
    }
}
```

### 7.3 IEstoqueService.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces
{
    public interface IEstoqueService
    {
        // Movimentações
        Task<bool> EntradaEstoqueAsync(int produtoId, decimal quantidade, decimal valorUnitario, 
            string documentoTipo, int? documentoId, string numeroDocumento, string observacoes, int usuarioId);
        Task<bool> SaidaEstoqueAsync(int produtoId, decimal quantidade, decimal valorUnitario, 
            string documentoTipo, int? documentoId, string numeroDocumento, string observacoes, int usuarioId);
        Task<bool> AjusteEstoqueAsync(int produtoId, decimal novaQuantidade, string motivo, int usuarioId);
        Task<bool> TransferenciaEstoqueAsync(int produtoOrigemId, int produtoDestinoId, decimal quantidade, 
            string observacoes, int usuarioId);
        
        // Consultas
        Task<decimal> GetEstoqueAtualAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetHistoricoMovimentacoesAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<MovimentacaoEstoque>> GetUltimasMovimentacoesAsync(int limite = 10);
        
        // Relatórios
        Task<decimal> GetTotalEntradasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<decimal> GetTotalSaidasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<ProdutoDto>> GetProdutosBaixoEstoqueAsync();
        Task<IEnumerable<ProdutoDto>> GetProdutosSemEstoqueAsync();
        
        // Validações
        Task<bool> ValidarEstoqueSuficienteAsync(int produtoId, decimal quantidade);
        Task<bool> ValidarProdutoControlaEstoqueAsync(int produtoId);
    }
}
```

## 8. Implementação dos Serviços

### 8.1 CategoriaService.cs
```csharp
using AutoMapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Application.Interfaces;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMapper _mapper;
        
        public CategoriaService(ICategoriaRepository categoriaRepository, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        }
        
        public async Task<CategoriaDto> GetByIdAsync(int id)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            return _mapper.Map<CategoriaDto>(categoria);
        }
        
        public async Task<IEnumerable<CategoriaListDto>> GetPagedAsync(int page, int pageSize, string search = null, bool? ativo = null)
        {
            return await _categoriaRepository.GetPagedAsync(page, pageSize, search, ativo);
        }
        
        public async Task<int> GetTotalCountAsync(string search = null, bool? ativo = null)
        {
            return await _categoriaRepository.GetTotalCountAsync(search, ativo);
        }
        
        public async Task<int> CreateAsync(CategoriaCreateDto dto, int usuarioId)
        {
            if (!await ValidateCreateAsync(dto))
                throw new ArgumentException("Dados inválidos para criação da categoria");
                
            var categoria = _mapper.Map<Categoria>(dto);
            categoria.DataCadastro = DateTime.Now;
            categoria.UsuarioCadastro = usuarioId;
            categoria.Ativo = true;
            
            return await _categoriaRepository.CreateAsync(categoria);
        }
        
        public async Task<bool> UpdateAsync(int id, CategoriaUpdateDto dto, int usuarioId)
        {
            if (!await ValidateUpdateAsync(id, dto))
                throw new ArgumentException("Dados inválidos para atualização da categoria");
                
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria == null)
                throw new ArgumentException("Categoria não encontrada");
                
            _mapper.Map(dto, categoria);
            categoria.DataUltimaAtualizacao = DateTime.Now;
            categoria.UsuarioUltimaAtualizacao = usuarioId;
            
            return await _categoriaRepository.UpdateAsync(categoria);
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            if (!await ValidateDeleteAsync(id))
                throw new ArgumentException("Não é possível excluir esta categoria");
                
            return await _categoriaRepository.DeleteAsync(id);
        }
        
        public async Task<IEnumerable<CategoriaDto>> GetAtivasAsync()
        {
            var categorias = await _categoriaRepository.GetAtivasAsync();
            return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
        }
        
        public async Task<bool> ValidateCreateAsync(CategoriaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return false;
                
            if (await _categoriaRepository.ExistsNomeAsync(dto.Nome))
                return false;
                
            return true;
        }
        
        public async Task<bool> ValidateUpdateAsync(int id, CategoriaUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return false;
                
            if (!await _categoriaRepository.ExistsAsync(id))
                return false;
                
            if (await _categoriaRepository.ExistsNomeAsync(dto.Nome, id))
                return false;
                
            return true;
        }
        
        public async Task<bool> ValidateDeleteAsync(int id)
        {
            if (!await _categoriaRepository.ExistsAsync(id))
                return false;
                
            if (await _categoriaRepository.TemProdutosVinculadosAsync(id))
                return false;
                
            return true;
        }
    }
}
```

### 8.2 EstoqueService.cs
```csharp
using AutoMapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Application.Interfaces;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IMapper _mapper;
        
        public EstoqueService(
            IMovimentacaoEstoqueRepository movimentacaoRepository,
            IProdutoRepository produtoRepository,
            IMapper mapper)
        {
            _movimentacaoRepository = movimentacaoRepository;
            _produtoRepository = produtoRepository;
            _mapper = mapper;
        }
        
        public async Task<bool> EntradaEstoqueAsync(int produtoId, decimal quantidade, decimal valorUnitario, 
            string documentoTipo, int? documentoId, string numeroDocumento, string observacoes, int usuarioId)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");
                
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");
                
            if (!produto.ControlaEstoque)
                throw new ArgumentException("Produto não controla estoque");
                
            var estoqueAnterior = produto.EstoqueAtual;
            var estoquePosterior = estoqueAnterior + quantidade;
            
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                TipoMovimentacao = TipoMovimentacao.Entrada,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                ValorTotal = quantidade * valorUnitario,
                EstoqueAnterior = estoqueAnterior,
                EstoquePosterior = estoquePosterior,
                DocumentoTipo = documentoTipo,
                DocumentoId = documentoId,
                NumeroDocumento = numeroDocumento,
                Observacoes = observacoes,
                DataMovimentacao = DateTime.Now,
                UsuarioId = usuarioId
            };
            
            await _movimentacaoRepository.CreateAsync(movimentacao);
            await _produtoRepository.UpdateEstoqueAsync(produtoId, estoquePosterior);
            
            return true;
        }
        
        public async Task<bool> SaidaEstoqueAsync(int produtoId, decimal quantidade, decimal valorUnitario, 
            string documentoTipo, int? documentoId, string numeroDocumento, string observacoes, int usuarioId)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");
                
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");
                
            if (!produto.ControlaEstoque)
                throw new ArgumentException("Produto não controla estoque");
                
            var estoqueAnterior = produto.EstoqueAtual;
            var estoquePosterior = estoqueAnterior - quantidade;
            
            if (estoquePosterior < 0 && !produto.PermiteVendaEstoqueNegativo)
                throw new ArgumentException("Estoque insuficiente");
                
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                TipoMovimentacao = TipoMovimentacao.Saida,
                Quantidade = quantidade,
                ValorUnitario = valorUnitario,
                ValorTotal = quantidade * valorUnitario,
                EstoqueAnterior = estoqueAnterior,
                EstoquePosterior = estoquePosterior,
                DocumentoTipo = documentoTipo,
                DocumentoId = documentoId,
                NumeroDocumento = numeroDocumento,
                Observacoes = observacoes,
                DataMovimentacao = DateTime.Now,
                UsuarioId = usuarioId
            };
            
            await _movimentacaoRepository.CreateAsync(movimentacao);
            await _produtoRepository.UpdateEstoqueAsync(produtoId, estoquePosterior);
            
            return true;
        }
        
        public async Task<bool> AjusteEstoqueAsync(int produtoId, decimal novaQuantidade, string motivo, int usuarioId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null)
                throw new ArgumentException("Produto não encontrado");
                
            if (!produto.ControlaEstoque)
                throw new ArgumentException("Produto não controla estoque");
                
            var estoqueAnterior = produto.EstoqueAtual;
            var diferenca = novaQuantidade - estoqueAnterior;
            
            if (diferenca == 0)
                return true; // Não há necessidade de ajuste
                
            var tipoMovimentacao = diferenca > 0 ? TipoMovimentacao.AjustePositivo : TipoMovimentacao.AjusteNegativo;
            var quantidade = Math.Abs(diferenca);
            
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = produtoId,
                TipoMovimentacao = tipoMovimentacao,
                Quantidade = quantidade,
                ValorUnitario = produto.CustoUnitario,
                ValorTotal = quantidade * produto.CustoUnitario,
                EstoqueAnterior = estoqueAnterior,
                EstoquePosterior = novaQuantidade,
                DocumentoTipo = "AJUSTE",
                DocumentoId = null,
                NumeroDocumento = $"AJ-{DateTime.Now:yyyyMMddHHmmss}",
                Observacoes = motivo,
                DataMovimentacao = DateTime.Now,
                UsuarioId = usuarioId
            };
            
            await _movimentacaoRepository.CreateAsync(movimentacao);
            await _produtoRepository.UpdateEstoqueAsync(produtoId, novaQuantidade);
            
            return true;
        }
        
        public async Task<bool> TransferenciaEstoqueAsync(int produtoOrigemId, int produtoDestinoId, decimal quantidade, 
            string observacoes, int usuarioId)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero");
                
            if (produtoOrigemId == produtoDestinoId)
                throw new ArgumentException("Produto de origem e destino devem ser diferentes");
                
            var produtoOrigem = await _produtoRepository.GetByIdAsync(produtoOrigemId);
            var produtoDestino = await _produtoRepository.GetByIdAsync(produtoDestinoId);
            
            if (produtoOrigem == null || produtoDestino == null)
                throw new ArgumentException("Produto não encontrado");
                
            if (!produtoOrigem.ControlaEstoque || !produtoDestino.ControlaEstoque)
                throw new ArgumentException("Ambos os produtos devem controlar estoque");
                
            if (produtoOrigem.EstoqueAtual < quantidade && !produtoOrigem.PermiteVendaEstoqueNegativo)
                throw new ArgumentException("Estoque insuficiente no produto de origem");
                
            var numeroTransferencia = $"TF-{DateTime.Now:yyyyMMddHHmmss}";
            
            // Saída do produto origem
            await SaidaEstoqueAsync(produtoOrigemId, quantidade, produtoOrigem.CustoUnitario,
                "TRANSFERENCIA", null, numeroTransferencia, observacoes, usuarioId);
                
            // Entrada no produto destino
            await EntradaEstoqueAsync(produtoDestinoId, quantidade, produtoDestino.CustoUnitario,
                "TRANSFERENCIA", null, numeroTransferencia, observacoes, usuarioId);
                
            return true;
        }
        
        public async Task<decimal> GetEstoqueAtualAsync(int produtoId)
        {
            return await _movimentacaoRepository.GetEstoqueAtualAsync(produtoId);
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetHistoricoMovimentacoesAsync(int produtoId)
        {
            return await _movimentacaoRepository.GetByProdutoIdAsync(produtoId);
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _movimentacaoRepository.GetByPeriodoAsync(dataInicio, dataFim);
        }
        
        public async Task<IEnumerable<MovimentacaoEstoque>> GetUltimasMovimentacoesAsync(int limite = 10)
        {
            return await _movimentacaoRepository.GetUltimasMovimentacoesAsync(limite);
        }
        
        public async Task<decimal> GetTotalEntradasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _movimentacaoRepository.GetTotalEntradasAsync(dataInicio, dataFim);
        }
        
        public async Task<decimal> GetTotalSaidasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _movimentacaoRepository.GetTotalSaidasAsync(dataInicio, dataFim);
        }
        
        public async Task<IEnumerable<ProdutoDto>> GetProdutosBaixoEstoqueAsync()
        {
            var produtos = await _produtoRepository.GetProdutosBaixoEstoqueAsync();
            return _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
        }
        
        public async Task<IEnumerable<ProdutoDto>> GetProdutosSemEstoqueAsync()
        {
            var produtos = await _produtoRepository.GetProdutosSemEstoqueAsync();
            return _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
        }
        
        public async Task<bool> ValidarEstoqueSuficienteAsync(int produtoId, decimal quantidade)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            if (produto == null) return false;
            
            if (!produto.ControlaEstoque) return true;
            
            if (produto.PermiteVendaEstoqueNegativo) return true;
            
            return produto.EstoqueAtual >= quantidade;
        }
        
        public async Task<bool> ValidarProdutoControlaEstoqueAsync(int produtoId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            return produto?.ControlaEstoque ?? false;
        }
    }
}
```

## Próximos Passos

Este documento completa a implementação dos repositórios e serviços principais do módulo de Estoque. Os próximos passos incluem:

1. **Implementação das páginas Blazor** (listagem, cadastro, edição)
2. **Criação dos componentes** (formulários, tabelas, modais)
3. **Implementação dos relatórios** de estoque
4. **Criação do sistema de alertas** para estoque baixo
5. **Implementação do inventário** de estoque
6. **Testes unitários** e de integração

O módulo está sendo desenvolvido seguindo as melhores práticas de Clean Architecture, SOLID e padrões de repositório e serviço.