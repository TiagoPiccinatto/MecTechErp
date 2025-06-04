# Implementação do Módulo de Estoque - Parte 2
# Repositórios e Serviços - MecTecERP

## 5. Interfaces dos Repositórios

### 5.1 ICategoriaRepository.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetAllAsync();
        Task<Categoria> GetByIdAsync(int id);
        Task<Categoria> GetByNomeAsync(string nome);
        Task<IEnumerable<CategoriaListDto>> GetPagedAsync(int page, int pageSize, string search = null, bool? ativo = null);
        Task<int> GetTotalCountAsync(string search = null, bool? ativo = null);
        Task<int> CreateAsync(Categoria categoria);
        Task<bool> UpdateAsync(Categoria categoria);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsNomeAsync(string nome, int? excludeId = null);
        Task<bool> TemProdutosVinculadosAsync(int categoriaId);
        Task<IEnumerable<Categoria>> GetAtivasAsync();
    }
}
```

### 5.2 IFornecedorRepository.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IFornecedorRepository
    {
        Task<IEnumerable<Fornecedor>> GetAllAsync();
        Task<Fornecedor> GetByIdAsync(int id);
        Task<Fornecedor> GetByCnpjCpfAsync(string cnpjCpf);
        Task<IEnumerable<FornecedorListDto>> GetPagedAsync(FornecedorSearchDto search);
        Task<int> GetTotalCountAsync(FornecedorSearchDto search);
        Task<int> CreateAsync(Fornecedor fornecedor);
        Task<bool> UpdateAsync(Fornecedor fornecedor);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsCnpjCpfAsync(string cnpjCpf, int? excludeId = null);
        Task<bool> TemProdutosVinculadosAsync(int fornecedorId);
        Task<IEnumerable<Fornecedor>> GetAtivosAsync();
        Task<IEnumerable<Fornecedor>> SearchAsync(string termo);
    }
}
```

### 5.3 IProdutoRepository.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<IEnumerable<Produto>> GetAllAsync();
        Task<Produto> GetByIdAsync(int id);
        Task<Produto> GetByCodigoAsync(string codigo);
        Task<Produto> GetByCodigoBarrasAsync(string codigoBarras);
        Task<IEnumerable<ProdutoListDto>> GetPagedAsync(ProdutoSearchDto search);
        Task<int> GetTotalCountAsync(ProdutoSearchDto search);
        Task<int> CreateAsync(Produto produto);
        Task<bool> UpdateAsync(Produto produto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null);
        Task<bool> ExistsCodigoBarrasAsync(string codigoBarras, int? excludeId = null);
        Task<bool> UpdateEstoqueAsync(int produtoId, decimal novoEstoque);
        Task<IEnumerable<Produto>> GetProdutosBaixoEstoqueAsync();
        Task<IEnumerable<Produto>> GetProdutosCriticosAsync();
        Task<IEnumerable<Produto>> GetProdutosSemEstoqueAsync();
        Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Produto>> GetProdutosPorFornecedorAsync(int fornecedorId);
        Task<IEnumerable<Produto>> SearchAsync(string termo);
        Task<decimal> GetValorTotalEstoqueAsync();
        Task<int> GetTotalProdutosAsync();
        Task<int> GetTotalProdutosBaixoEstoqueAsync();
        Task<int> GetTotalProdutosSemEstoqueAsync();
    }
}
```

### 5.4 IMovimentacaoEstoqueRepository.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IMovimentacaoEstoqueRepository
    {
        Task<IEnumerable<MovimentacaoEstoque>> GetAllAsync();
        Task<MovimentacaoEstoque> GetByIdAsync(int id);
        Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetByDocumentoAsync(string documentoTipo, int documentoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<MovimentacaoEstoque>> GetPagedAsync(int page, int pageSize, int? produtoId = null, 
            TipoMovimentacao? tipo = null, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<int> GetTotalCountAsync(int? produtoId = null, TipoMovimentacao? tipo = null, 
            DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<int> CreateAsync(MovimentacaoEstoque movimentacao);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<decimal> GetEstoqueAtualAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetUltimasMovimentacoesAsync(int limite = 10);
        Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesPorTipoAsync(TipoMovimentacao tipo, DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<decimal> GetTotalEntradasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
        Task<decimal> GetTotalSaidasAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    }
}
```

### 5.5 IInventarioRepository.cs
```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IInventarioRepository
    {
        Task<IEnumerable<InventarioEstoque>> GetAllAsync();
        Task<InventarioEstoque> GetByIdAsync(int id);
        Task<InventarioEstoque> GetByNumeroAsync(string numero);
        Task<IEnumerable<InventarioEstoque>> GetPagedAsync(int page, int pageSize, StatusInventario? status = null);
        Task<int> GetTotalCountAsync(StatusInventario? status = null);
        Task<int> CreateAsync(InventarioEstoque inventario);
        Task<bool> UpdateAsync(InventarioEstoque inventario);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsNumeroAsync(string numero, int? excludeId = null);
        Task<bool> TemInventarioAbertoAsync();
        Task<InventarioEstoque> GetInventarioAbertoAsync();
        
        // Itens do Inventário
        Task<IEnumerable<InventarioItem>> GetItensAsync(int inventarioId);
        Task<InventarioItem> GetItemAsync(int inventarioId, int produtoId);
        Task<int> CreateItemAsync(InventarioItem item);
        Task<bool> UpdateItemAsync(InventarioItem item);
        Task<bool> DeleteItemAsync(int itemId);
        Task<bool> AdicionarProdutosAsync(int inventarioId, List<int> produtoIds);
        Task<bool> FinalizarInventarioAsync(int inventarioId, int usuarioId);
        Task<bool> CancelarInventarioAsync(int inventarioId);
    }
}
```

## 6. Implementação dos Repositórios

### 6.1 CategoriaRepository.cs
```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Application.DTOs;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        
        public CategoriaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        
        public async Task<IEnumerable<Categoria>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT Id, Nome, Descricao, Ativo, DataCadastro, DataUltimaAtualizacao,
                       UsuarioCadastro, UsuarioUltimaAtualizacao
                FROM Categorias
                ORDER BY Nome";
                
            return await connection.QueryAsync<Categoria>(sql);
        }
        
        public async Task<Categoria> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT Id, Nome, Descricao, Ativo, DataCadastro, DataUltimaAtualizacao,
                       UsuarioCadastro, UsuarioUltimaAtualizacao
                FROM Categorias
                WHERE Id = @Id";
                
            return await connection.QueryFirstOrDefaultAsync<Categoria>(sql, new { Id = id });
        }
        
        public async Task<Categoria> GetByNomeAsync(string nome)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT Id, Nome, Descricao, Ativo, DataCadastro, DataUltimaAtualizacao,
                       UsuarioCadastro, UsuarioUltimaAtualizacao
                FROM Categorias
                WHERE Nome = @Nome";
                
            return await connection.QueryFirstOrDefaultAsync<Categoria>(sql, new { Nome = nome });
        }
        
        public async Task<IEnumerable<CategoriaListDto>> GetPagedAsync(int page, int pageSize, string search = null, bool? ativo = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrEmpty(search))
            {
                whereClause += " AND (Nome LIKE @Search OR Descricao LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }
            
            if (ativo.HasValue)
            {
                whereClause += " AND Ativo = @Ativo";
                parameters.Add("Ativo", ativo.Value);
            }
            
            var offset = (page - 1) * pageSize;
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);
            
            var sql = $@"
                SELECT c.Id, c.Nome, c.Descricao, c.Ativo,
                       COUNT(p.Id) as TotalProdutos
                FROM Categorias c
                LEFT JOIN Produtos p ON c.Id = p.CategoriaId AND p.Ativo = 1
                {whereClause}
                GROUP BY c.Id, c.Nome, c.Descricao, c.Ativo
                ORDER BY c.Nome
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                
            return await connection.QueryAsync<CategoriaListDto>(sql, parameters);
        }
        
        public async Task<int> GetTotalCountAsync(string search = null, bool? ativo = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrEmpty(search))
            {
                whereClause += " AND (Nome LIKE @Search OR Descricao LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }
            
            if (ativo.HasValue)
            {
                whereClause += " AND Ativo = @Ativo";
                parameters.Add("Ativo", ativo.Value);
            }
            
            var sql = $"SELECT COUNT(*) FROM Categorias {whereClause}";
            
            return await connection.QuerySingleAsync<int>(sql, parameters);
        }
        
        public async Task<int> CreateAsync(Categoria categoria)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                INSERT INTO Categorias (Nome, Descricao, Ativo, DataCadastro, UsuarioCadastro)
                VALUES (@Nome, @Descricao, @Ativo, @DataCadastro, @UsuarioCadastro);
                SELECT CAST(SCOPE_IDENTITY() as int);";
                
            return await connection.QuerySingleAsync<int>(sql, categoria);
        }
        
        public async Task<bool> UpdateAsync(Categoria categoria)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                UPDATE Categorias SET
                    Nome = @Nome,
                    Descricao = @Descricao,
                    Ativo = @Ativo,
                    DataUltimaAtualizacao = @DataUltimaAtualizacao,
                    UsuarioUltimaAtualizacao = @UsuarioUltimaAtualizacao
                WHERE Id = @Id";
                
            var rowsAffected = await connection.ExecuteAsync(sql, categoria);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "DELETE FROM Categorias WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        
        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Categorias WHERE Id = @Id";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        
        public async Task<bool> ExistsNomeAsync(string nome, int? excludeId = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Categorias WHERE Nome = @Nome";
            var parameters = new { Nome = nome };
            
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
                parameters = new { Nome = nome, ExcludeId = excludeId.Value };
            }
            
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        
        public async Task<bool> TemProdutosVinculadosAsync(int categoriaId)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Produtos WHERE CategoriaId = @CategoriaId";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { CategoriaId = categoriaId });
            return count > 0;
        }
        
        public async Task<IEnumerable<Categoria>> GetAtivasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT Id, Nome, Descricao, Ativo, DataCadastro, DataUltimaAtualizacao,
                       UsuarioCadastro, UsuarioUltimaAtualizacao
                FROM Categorias
                WHERE Ativo = 1
                ORDER BY Nome";
                
            return await connection.QueryAsync<Categoria>(sql);
        }
    }
}
```

### 6.2 ProdutoRepository.cs (Parte 1)
```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Application.DTOs;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        
        public ProdutoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        
        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
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
        
        public async Task<Produto> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Id = @Id";
                
            var result = await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { Id = id },
                splitOn: "CategoriaNome,FornecedorNome");
                
            return result.FirstOrDefault();
        }
        
        public async Task<Produto> GetByCodigoAsync(string codigo)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.Codigo = @Codigo";
                
            var result = await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { Codigo = codigo },
                splitOn: "CategoriaNome,FornecedorNome");
                
            return result.FirstOrDefault();
        }
        
        public async Task<Produto> GetByCodigoBarrasAsync(string codigoBarras)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT p.*, c.Nome as CategoriaNome, f.RazaoSocial as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                WHERE p.CodigoBarras = @CodigoBarras";
                
            var result = await connection.QueryAsync<Produto, Categoria, Fornecedor, Produto>(sql,
                (produto, categoria, fornecedor) =>
                {
                    produto.Categoria = categoria;
                    produto.Fornecedor = fornecedor;
                    return produto;
                },
                new { CodigoBarras = codigoBarras },
                splitOn: "CategoriaNome,FornecedorNome");
                
            return result.FirstOrDefault();
        }
        
        public async Task<IEnumerable<ProdutoListDto>> GetPagedAsync(ProdutoSearchDto search)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            if (!string.IsNullOrEmpty(search.Codigo))
            {
                whereClause += " AND p.Codigo LIKE @Codigo";
                parameters.Add("Codigo", $"%{search.Codigo}%");
            }
            
            if (!string.IsNullOrEmpty(search.Nome))
            {
                whereClause += " AND p.Nome LIKE @Nome";
                parameters.Add("Nome", $"%{search.Nome}%");
            }
            
            if (!string.IsNullOrEmpty(search.CodigoBarras))
            {
                whereClause += " AND p.CodigoBarras LIKE @CodigoBarras";
                parameters.Add("CodigoBarras", $"%{search.CodigoBarras}%");
            }
            
            if (search.CategoriaId.HasValue)
            {
                whereClause += " AND p.CategoriaId = @CategoriaId";
                parameters.Add("CategoriaId", search.CategoriaId.Value);
            }
            
            if (search.FornecedorId.HasValue)
            {
                whereClause += " AND p.FornecedorId = @FornecedorId";
                parameters.Add("FornecedorId", search.FornecedorId.Value);
            }
            
            if (search.Ativo.HasValue)
            {
                whereClause += " AND p.Ativo = @Ativo";
                parameters.Add("Ativo", search.Ativo.Value);
            }
            
            if (search.EstoqueBaixo.HasValue && search.EstoqueBaixo.Value)
            {
                whereClause += " AND p.EstoqueAtual <= p.EstoqueMinimo";
            }
            
            if (search.SemEstoque.HasValue && search.SemEstoque.Value)
            {
                whereClause += " AND p.EstoqueAtual <= 0";
            }
            
            var orderBy = GetOrderByClause(search.OrderBy, search.OrderDesc);
            var offset = (search.Page - 1) * search.PageSize;
            
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", search.PageSize);
            
            var sql = $@"
                SELECT p.Id, p.Codigo, p.Nome, p.CustoUnitario, p.PrecoVenda,
                       p.EstoqueAtual, p.EstoqueMinimo, p.UnidadeMedida, p.Ativo,
                       c.Nome as CategoriaNome,
                       ISNULL(f.NomeFantasia, f.RazaoSocial) as FornecedorNome
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                {whereClause}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                
            return await connection.QueryAsync<ProdutoListDto>(sql, parameters);
        }
        
        public async Task<int> GetTotalCountAsync(ProdutoSearchDto search)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var whereClause = "WHERE 1=1";
            var parameters = new DynamicParameters();
            
            // Aplicar os mesmos filtros do GetPagedAsync
            if (!string.IsNullOrEmpty(search.Codigo))
            {
                whereClause += " AND p.Codigo LIKE @Codigo";
                parameters.Add("Codigo", $"%{search.Codigo}%");
            }
            
            if (!string.IsNullOrEmpty(search.Nome))
            {
                whereClause += " AND p.Nome LIKE @Nome";
                parameters.Add("Nome", $"%{search.Nome}%");
            }
            
            if (!string.IsNullOrEmpty(search.CodigoBarras))
            {
                whereClause += " AND p.CodigoBarras LIKE @CodigoBarras";
                parameters.Add("CodigoBarras", $"%{search.CodigoBarras}%");
            }
            
            if (search.CategoriaId.HasValue)
            {
                whereClause += " AND p.CategoriaId = @CategoriaId";
                parameters.Add("CategoriaId", search.CategoriaId.Value);
            }
            
            if (search.FornecedorId.HasValue)
            {
                whereClause += " AND p.FornecedorId = @FornecedorId";
                parameters.Add("FornecedorId", search.FornecedorId.Value);
            }
            
            if (search.Ativo.HasValue)
            {
                whereClause += " AND p.Ativo = @Ativo";
                parameters.Add("Ativo", search.Ativo.Value);
            }
            
            if (search.EstoqueBaixo.HasValue && search.EstoqueBaixo.Value)
            {
                whereClause += " AND p.EstoqueAtual <= p.EstoqueMinimo";
            }
            
            if (search.SemEstoque.HasValue && search.SemEstoque.Value)
            {
                whereClause += " AND p.EstoqueAtual <= 0";
            }
            
            var sql = $@"
                SELECT COUNT(*)
                FROM Produtos p
                INNER JOIN Categorias c ON p.CategoriaId = c.Id
                LEFT JOIN Fornecedores f ON p.FornecedorId = f.Id
                {whereClause}";
                
            return await connection.QuerySingleAsync<int>(sql, parameters);
        }
        
        public async Task<int> CreateAsync(Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                INSERT INTO Produtos (
                    Codigo, CodigoBarras, Nome, Descricao, CategoriaId, FornecedorId,
                    CustoUnitario, PrecoVenda, MargemLucro, EstoqueAtual, EstoqueMinimo,
                    EstoqueMaximo, PontoReposicao, UnidadeMedida, Localizacao, Peso,
                    Dimensoes, ControlaEstoque, PermiteVendaEstoqueNegativo, Ncm, Cest,
                    Foto, Observacoes, Ativo, DataCadastro, UsuarioCadastro
                )
                VALUES (
                    @Codigo, @CodigoBarras, @Nome, @Descricao, @CategoriaId, @FornecedorId,
                    @CustoUnitario, @PrecoVenda, @MargemLucro, @EstoqueAtual, @EstoqueMinimo,
                    @EstoqueMaximo, @PontoReposicao, @UnidadeMedida, @Localizacao, @Peso,
                    @Dimensoes, @ControlaEstoque, @PermiteVendaEstoqueNegativo, @Ncm, @Cest,
                    @Foto, @Observacoes, @Ativo, @DataCadastro, @UsuarioCadastro
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
                
            return await connection.QuerySingleAsync<int>(sql, produto);
        }
        
        public async Task<bool> UpdateAsync(Produto produto)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                UPDATE Produtos SET
                    Codigo = @Codigo,
                    CodigoBarras = @CodigoBarras,
                    Nome = @Nome,
                    Descricao = @Descricao,
                    CategoriaId = @CategoriaId,
                    FornecedorId = @FornecedorId,
                    CustoUnitario = @CustoUnitario,
                    PrecoVenda = @PrecoVenda,
                    MargemLucro = @MargemLucro,
                    EstoqueMinimo = @EstoqueMinimo,
                    EstoqueMaximo = @EstoqueMaximo,
                    PontoReposicao = @PontoReposicao,
                    UnidadeMedida = @UnidadeMedida,
                    Localizacao = @Localizacao,
                    Peso = @Peso,
                    Dimensoes = @Dimensoes,
                    ControlaEstoque = @ControlaEstoque,
                    PermiteVendaEstoqueNegativo = @PermiteVendaEstoqueNegativo,
                    Ncm = @Ncm,
                    Cest = @Cest,
                    Foto = @Foto,
                    Observacoes = @Observacoes,
                    Ativo = @Ativo,
                    DataUltimaAtualizacao = @DataUltimaAtualizacao,
                    UsuarioUltimaAtualizacao = @UsuarioUltimaAtualizacao
                WHERE Id = @Id";
                
            var rowsAffected = await connection.ExecuteAsync(sql, produto);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "DELETE FROM Produtos WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        
        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Produtos WHERE Id = @Id";
            
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }
        
        public async Task<bool> ExistsCodigoAsync(string codigo, int? excludeId = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Produtos WHERE Codigo = @Codigo";
            var parameters = new { Codigo = codigo };
            
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
                parameters = new { Codigo = codigo, ExcludeId = excludeId.Value };
            }
            
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        
        public async Task<bool> ExistsCodigoBarrasAsync(string codigoBarras, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(codigoBarras)) return false;
            
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = "SELECT COUNT(*) FROM Produtos WHERE CodigoBarras = @CodigoBarras";
            var parameters = new { CodigoBarras = codigoBarras };
            
            if (excludeId.HasValue)
            {
                sql += " AND Id <> @ExcludeId";
                parameters = new { CodigoBarras = codigoBarras, ExcludeId = excludeId.Value };
            }
            
            var count = await connection.QuerySingleAsync<int>(sql, parameters);
            return count > 0;
        }
        
        public async Task<bool> UpdateEstoqueAsync(int produtoId, decimal novoEstoque)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                UPDATE Produtos SET
                    EstoqueAtual = @NovoEstoque,
                    DataUltimaAtualizacao = GETDATE()
                WHERE Id = @ProdutoId";
                
            var rowsAffected = await connection.ExecuteAsync(sql, new { ProdutoId = produtoId, NovoEstoque = novoEstoque });
            return rowsAffected > 0;
        }
        
        private string GetOrderByClause(string orderBy, bool orderDesc)
        {
            var direction = orderDesc ? "DESC" : "ASC";
            
            return orderBy?.ToLower() switch
            {
                "codigo" => $"ORDER BY p.Codigo {direction}",
                "nome" => $"ORDER BY p.Nome {direction}",
                "categoria" => $"ORDER BY c.Nome {direction}",
                "fornecedor" => $"ORDER BY f.RazaoSocial {direction}",
                "estoque" => $"ORDER BY p.EstoqueAtual {direction}",
                "preco" => $"ORDER BY p.PrecoVenda {direction}",
                _ => $"ORDER BY p.Nome {direction}"
            };
        }
        
        // Métodos de consulta específicos continuam na próxima parte...
    }
}
```

## Próximos Passos

Este documento apresenta a implementação dos repositórios base do módulo de Estoque. Os próximos passos incluem:

1. **Continuação do ProdutoRepository** com métodos específicos
2. **Implementação do MovimentacaoEstoqueRepository**
3. **Implementação dos Serviços** (IEstoqueService, IProdutoService, etc.)
4. **Criação das páginas Blazor** (listagem, cadastro, edição)
5. **Implementação dos componentes** (formulários, tabelas, modais)

O módulo está sendo desenvolvido seguindo as melhores práticas de Clean Architecture e SOLID.