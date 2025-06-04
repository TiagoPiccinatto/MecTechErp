# Implementação do Módulo de Clientes - MecTecERP

## Visão Geral

O módulo de Clientes é fundamental para o sistema MecTecERP, sendo a base para todos os outros módulos. Este documento detalha a implementação completa do módulo, incluindo estrutura de dados, regras de negócio, interfaces e funcionalidades.

## Estrutura de Dados

### Tabela: Clientes

```sql
CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TipoCliente CHAR(2) NOT NULL CHECK (TipoCliente IN ('PF', 'PJ')),
    NomeRazaoSocial NVARCHAR(200) NOT NULL,
    CpfCnpj NVARCHAR(18) NOT NULL UNIQUE,
    Telefone1 NVARCHAR(15) NOT NULL,
    Telefone2 NVARCHAR(15) NULL,
    Email NVARCHAR(100) NULL,
    Cep NVARCHAR(9) NULL,
    Logradouro NVARCHAR(200) NULL,
    Numero NVARCHAR(10) NULL,
    Complemento NVARCHAR(100) NULL,
    Bairro NVARCHAR(100) NULL,
    Cidade NVARCHAR(100) NULL,
    Uf CHAR(2) NULL,
    Observacoes NVARCHAR(MAX) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2 NULL,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT NULL,
    
    CONSTRAINT FK_Clientes_UsuarioCadastro FOREIGN KEY (UsuarioCadastro) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Clientes_UsuarioAtualizacao FOREIGN KEY (UsuarioUltimaAtualizacao) REFERENCES AspNetUsers(Id)
);

-- Índices para performance
CREATE INDEX IX_Clientes_CpfCnpj ON Clientes(CpfCnpj);
CREATE INDEX IX_Clientes_Nome ON Clientes(NomeRazaoSocial);
CREATE INDEX IX_Clientes_Telefone ON Clientes(Telefone1);
CREATE INDEX IX_Clientes_Ativo ON Clientes(Ativo);
CREATE INDEX IX_Clientes_DataCadastro ON Clientes(DataCadastro);
```

## Entidades do Domínio

### Cliente.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tipo de cliente é obrigatório")]
        [StringLength(2, ErrorMessage = "Tipo deve ter 2 caracteres")]
        public string TipoCliente { get; set; } // PF ou PJ
        
        [Required(ErrorMessage = "Nome/Razão Social é obrigatório")]
        [StringLength(200, ErrorMessage = "Nome não pode exceder 200 caracteres")]
        public string NomeRazaoSocial { get; set; }
        
        [Required(ErrorMessage = "CPF/CNPJ é obrigatório")]
        [StringLength(18, ErrorMessage = "CPF/CNPJ inválido")]
        public string CpfCnpj { get; set; }
        
        [Required(ErrorMessage = "Telefone principal é obrigatório")]
        [StringLength(15, ErrorMessage = "Telefone inválido")]
        public string Telefone1 { get; set; }
        
        [StringLength(15, ErrorMessage = "Telefone inválido")]
        public string? Telefone2 { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email não pode exceder 100 caracteres")]
        public string? Email { get; set; }
        
        [StringLength(9, ErrorMessage = "CEP inválido")]
        public string? Cep { get; set; }
        
        [StringLength(200, ErrorMessage = "Logradouro não pode exceder 200 caracteres")]
        public string? Logradouro { get; set; }
        
        [StringLength(10, ErrorMessage = "Número não pode exceder 10 caracteres")]
        public string? Numero { get; set; }
        
        [StringLength(100, ErrorMessage = "Complemento não pode exceder 100 caracteres")]
        public string? Complemento { get; set; }
        
        [StringLength(100, ErrorMessage = "Bairro não pode exceder 100 caracteres")]
        public string? Bairro { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade não pode exceder 100 caracteres")]
        public string? Cidade { get; set; }
        
        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string? Uf { get; set; }
        
        public string? Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        public int UsuarioCadastro { get; set; }
        
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Propriedades calculadas
        public string TipoClienteDescricao => TipoCliente == "PF" ? "Pessoa Física" : "Pessoa Jurídica";
        
        public string DocumentoFormatado => TipoCliente == "PF" 
            ? FormatarCpf(CpfCnpj) 
            : FormatarCnpj(CpfCnpj);
        
        public string EnderecoCompleto => $"{Logradouro}, {Numero} {Complemento} - {Bairro}, {Cidade}/{Uf} - CEP: {Cep}".Trim();
        
        // Métodos auxiliares
        private static string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11) return cpf;
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }
        
        private static string FormatarCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14) return cnpj;
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
    }
}
```

## DTOs e ViewModels

### ClienteDto.cs

```csharp
namespace MecTecERP.Shared.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string TipoCliente { get; set; }
        public string NomeRazaoSocial { get; set; }
        public string CpfCnpj { get; set; }
        public string Telefone1 { get; set; }
        public string? Telefone2 { get; set; }
        public string? Email { get; set; }
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        // Propriedades calculadas
        public string TipoClienteDescricao { get; set; }
        public string DocumentoFormatado { get; set; }
        public string EnderecoCompleto { get; set; }
    }
    
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "Tipo de cliente é obrigatório")]
        public string TipoCliente { get; set; }
        
        [Required(ErrorMessage = "Nome/Razão Social é obrigatório")]
        [StringLength(200)]
        public string NomeRazaoSocial { get; set; }
        
        [Required(ErrorMessage = "CPF/CNPJ é obrigatório")]
        public string CpfCnpj { get; set; }
        
        [Required(ErrorMessage = "Telefone principal é obrigatório")]
        public string Telefone1 { get; set; }
        
        public string? Telefone2 { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }
        
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Observacoes { get; set; }
    }
    
    public class ClienteUpdateDto : ClienteCreateDto
    {
        public int Id { get; set; }
    }
    
    public class ClienteListDto
    {
        public int Id { get; set; }
        public string TipoCliente { get; set; }
        public string NomeRazaoSocial { get; set; }
        public string DocumentoFormatado { get; set; }
        public string Telefone1 { get; set; }
        public string? Email { get; set; }
        public string? Cidade { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int TotalVeiculos { get; set; }
        public int TotalOS { get; set; }
    }
}
```

## Repositório

### IClienteRepository.cs

```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Shared.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<ClienteListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, bool? ativo = null);
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<Cliente?> GetEntityByIdAsync(int id);
        Task<bool> ExistsByCpfCnpjAsync(string cpfCnpj, int? excludeId = null);
        Task<int> CreateAsync(ClienteCreateDto cliente, int usuarioId);
        Task<bool> UpdateAsync(ClienteUpdateDto cliente, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int id, int usuarioId);
        Task<int> GetTotalCountAsync(string? search = null, bool? ativo = null);
        Task<IEnumerable<ClienteDto>> SearchAsync(string term);
    }
}
```

### ClienteRepository.cs

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Shared.DTOs;
using System.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _connectionString;
        
        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public async Task<IEnumerable<ClienteListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, bool? ativo = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    c.Id,
                    c.TipoCliente,
                    c.NomeRazaoSocial,
                    c.CpfCnpj,
                    c.Telefone1,
                    c.Email,
                    c.Cidade,
                    c.Ativo,
                    c.DataCadastro,
                    COUNT(DISTINCT v.Id) as TotalVeiculos,
                    COUNT(DISTINCT os.Id) as TotalOS
                FROM Clientes c
                LEFT JOIN Veiculos v ON c.Id = v.ClienteId AND v.Ativo = 1
                LEFT JOIN OrdemServico os ON c.Id = os.ClienteId
                WHERE (@Search IS NULL OR c.NomeRazaoSocial LIKE '%' + @Search + '%' 
                       OR c.CpfCnpj LIKE '%' + @Search + '%'
                       OR c.Telefone1 LIKE '%' + @Search + '%')
                  AND (@Ativo IS NULL OR c.Ativo = @Ativo)
                GROUP BY c.Id, c.TipoCliente, c.NomeRazaoSocial, c.CpfCnpj, 
                         c.Telefone1, c.Email, c.Cidade, c.Ativo, c.DataCadastro
                ORDER BY c.NomeRazaoSocial
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            var parameters = new
            {
                Search = search,
                Ativo = ativo,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            };
            
            var result = await connection.QueryAsync<ClienteListDto>(sql, parameters);
            
            // Formatar documentos
            foreach (var cliente in result)
            {
                cliente.DocumentoFormatado = cliente.TipoCliente == "PF" 
                    ? FormatarCpf(cliente.CpfCnpj) 
                    : FormatarCnpj(cliente.CpfCnpj);
            }
            
            return result;
        }
        
        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    Id, TipoCliente, NomeRazaoSocial, CpfCnpj, Telefone1, Telefone2,
                    Email, Cep, Logradouro, Numero, Complemento, Bairro, Cidade, Uf,
                    Observacoes, Ativo, DataCadastro, DataUltimaAtualizacao
                FROM Clientes 
                WHERE Id = @Id";
            
            var cliente = await connection.QueryFirstOrDefaultAsync<ClienteDto>(sql, new { Id = id });
            
            if (cliente != null)
            {
                cliente.TipoClienteDescricao = cliente.TipoCliente == "PF" ? "Pessoa Física" : "Pessoa Jurídica";
                cliente.DocumentoFormatado = cliente.TipoCliente == "PF" 
                    ? FormatarCpf(cliente.CpfCnpj) 
                    : FormatarCnpj(cliente.CpfCnpj);
                cliente.EnderecoCompleto = $"{cliente.Logradouro}, {cliente.Numero} {cliente.Complemento} - {cliente.Bairro}, {cliente.Cidade}/{cliente.Uf} - CEP: {cliente.Cep}".Trim();
            }
            
            return cliente;
        }
        
        public async Task<Cliente?> GetEntityByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT * FROM Clientes WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
        }
        
        public async Task<bool> ExistsByCpfCnpjAsync(string cpfCnpj, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT COUNT(1) FROM Clientes WHERE CpfCnpj = @CpfCnpj AND (@ExcludeId IS NULL OR Id != @ExcludeId)";
            var count = await connection.QueryFirstAsync<int>(sql, new { CpfCnpj = cpfCnpj, ExcludeId = excludeId });
            
            return count > 0;
        }
        
        public async Task<int> CreateAsync(ClienteCreateDto cliente, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                INSERT INTO Clientes (
                    TipoCliente, NomeRazaoSocial, CpfCnpj, Telefone1, Telefone2,
                    Email, Cep, Logradouro, Numero, Complemento, Bairro, Cidade, Uf,
                    Observacoes, UsuarioCadastro
                ) VALUES (
                    @TipoCliente, @NomeRazaoSocial, @CpfCnpj, @Telefone1, @Telefone2,
                    @Email, @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Uf,
                    @Observacoes, @UsuarioCadastro
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";
            
            var parameters = new
            {
                cliente.TipoCliente,
                cliente.NomeRazaoSocial,
                cliente.CpfCnpj,
                cliente.Telefone1,
                cliente.Telefone2,
                cliente.Email,
                cliente.Cep,
                cliente.Logradouro,
                cliente.Numero,
                cliente.Complemento,
                cliente.Bairro,
                cliente.Cidade,
                cliente.Uf,
                cliente.Observacoes,
                UsuarioCadastro = usuarioId
            };
            
            return await connection.QueryFirstAsync<int>(sql, parameters);
        }
        
        public async Task<bool> UpdateAsync(ClienteUpdateDto cliente, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                UPDATE Clientes SET
                    TipoCliente = @TipoCliente,
                    NomeRazaoSocial = @NomeRazaoSocial,
                    CpfCnpj = @CpfCnpj,
                    Telefone1 = @Telefone1,
                    Telefone2 = @Telefone2,
                    Email = @Email,
                    Cep = @Cep,
                    Logradouro = @Logradouro,
                    Numero = @Numero,
                    Complemento = @Complemento,
                    Bairro = @Bairro,
                    Cidade = @Cidade,
                    Uf = @Uf,
                    Observacoes = @Observacoes,
                    DataUltimaAtualizacao = GETDATE(),
                    UsuarioUltimaAtualizacao = @UsuarioId
                WHERE Id = @Id";
            
            var parameters = new
            {
                cliente.Id,
                cliente.TipoCliente,
                cliente.NomeRazaoSocial,
                cliente.CpfCnpj,
                cliente.Telefone1,
                cliente.Telefone2,
                cliente.Email,
                cliente.Cep,
                cliente.Logradouro,
                cliente.Numero,
                cliente.Complemento,
                cliente.Bairro,
                cliente.Cidade,
                cliente.Uf,
                cliente.Observacoes,
                UsuarioId = usuarioId
            };
            
            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // Verificar se tem veículos ou OS vinculadas
            var checkSql = @"
                SELECT COUNT(1) FROM Veiculos WHERE ClienteId = @Id
                UNION ALL
                SELECT COUNT(1) FROM OrdemServico WHERE ClienteId = @Id";
            
            var counts = await connection.QueryAsync<int>(checkSql, new { Id = id });
            
            if (counts.Any(c => c > 0))
            {
                // Se tem dependências, apenas inativar
                return await ToggleStatusAsync(id, 0);
            }
            
            // Se não tem dependências, pode excluir
            var deleteSql = "DELETE FROM Clientes WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(deleteSql, new { Id = id });
            
            return rowsAffected > 0;
        }
        
        public async Task<bool> ToggleStatusAsync(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                UPDATE Clientes SET 
                    Ativo = CASE WHEN Ativo = 1 THEN 0 ELSE 1 END,
                    DataUltimaAtualizacao = GETDATE(),
                    UsuarioUltimaAtualizacao = @UsuarioId
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UsuarioId = usuarioId });
            return rowsAffected > 0;
        }
        
        public async Task<int> GetTotalCountAsync(string? search = null, bool? ativo = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT COUNT(1) FROM Clientes 
                WHERE (@Search IS NULL OR NomeRazaoSocial LIKE '%' + @Search + '%' 
                       OR CpfCnpj LIKE '%' + @Search + '%'
                       OR Telefone1 LIKE '%' + @Search + '%')
                  AND (@Ativo IS NULL OR Ativo = @Ativo)";
            
            return await connection.QueryFirstAsync<int>(sql, new { Search = search, Ativo = ativo });
        }
        
        public async Task<IEnumerable<ClienteDto>> SearchAsync(string term)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT TOP 10
                    Id, TipoCliente, NomeRazaoSocial, CpfCnpj, Telefone1, Email
                FROM Clientes 
                WHERE Ativo = 1 
                  AND (NomeRazaoSocial LIKE '%' + @Term + '%' 
                       OR CpfCnpj LIKE '%' + @Term + '%'
                       OR Telefone1 LIKE '%' + @Term + '%')
                ORDER BY NomeRazaoSocial";
            
            var result = await connection.QueryAsync<ClienteDto>(sql, new { Term = term });
            
            // Formatar documentos
            foreach (var cliente in result)
            {
                cliente.DocumentoFormatado = cliente.TipoCliente == "PF" 
                    ? FormatarCpf(cliente.CpfCnpj) 
                    : FormatarCnpj(cliente.CpfCnpj);
            }
            
            return result;
        }
        
        private static string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11) return cpf;
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }
        
        private static string FormatarCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14) return cnpj;
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }
    }
}
```

## Serviços

### IClienteService.cs

```csharp
using MecTecERP.Shared.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IClienteService
    {
        Task<(IEnumerable<ClienteListDto> clientes, int totalCount)> GetAllAsync(int page, int pageSize, string? search, bool? ativo);
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ClienteCreateDto cliente, int usuarioId);
        Task<bool> UpdateAsync(ClienteUpdateDto cliente, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int id, int usuarioId);
        Task<IEnumerable<ClienteDto>> SearchAsync(string term);
        Task<bool> ValidateCpfCnpjAsync(string cpfCnpj, int? excludeId = null);
    }
}
```

### ClienteService.cs

```csharp
using MecTecERP.Domain.Interfaces;
using MecTecERP.Shared.DTOs;
using MecTecERP.Shared.Helpers;

namespace MecTecERP.Domain.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }
        
        public async Task<(IEnumerable<ClienteListDto> clientes, int totalCount)> GetAllAsync(int page, int pageSize, string? search, bool? ativo)
        {
            var clientes = await _clienteRepository.GetAllAsync(page, pageSize, search, ativo);
            var totalCount = await _clienteRepository.GetTotalCountAsync(search, ativo);
            
            return (clientes, totalCount);
        }
        
        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }
        
        public async Task<int> CreateAsync(ClienteCreateDto cliente, int usuarioId)
        {
            // Validações
            await ValidateClienteAsync(cliente);
            
            // Limpar e validar CPF/CNPJ
            cliente.CpfCnpj = DocumentHelper.LimparDocumento(cliente.CpfCnpj);
            
            if (!await ValidateCpfCnpjAsync(cliente.CpfCnpj))
            {
                throw new ArgumentException("CPF/CNPJ inválido ou já cadastrado");
            }
            
            return await _clienteRepository.CreateAsync(cliente, usuarioId);
        }
        
        public async Task<bool> UpdateAsync(ClienteUpdateDto cliente, int usuarioId)
        {
            // Validações
            await ValidateClienteAsync(cliente);
            
            // Limpar e validar CPF/CNPJ
            cliente.CpfCnpj = DocumentHelper.LimparDocumento(cliente.CpfCnpj);
            
            if (!await ValidateCpfCnpjAsync(cliente.CpfCnpj, cliente.Id))
            {
                throw new ArgumentException("CPF/CNPJ inválido ou já cadastrado");
            }
            
            return await _clienteRepository.UpdateAsync(cliente, usuarioId);
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente == null)
            {
                throw new ArgumentException("Cliente não encontrado");
            }
            
            return await _clienteRepository.DeleteAsync(id);
        }
        
        public async Task<bool> ToggleStatusAsync(int id, int usuarioId)
        {
            var cliente = await _clienteRepository.GetEntityByIdAsync(id);
            if (cliente == null)
            {
                throw new ArgumentException("Cliente não encontrado");
            }
            
            return await _clienteRepository.ToggleStatusAsync(id, usuarioId);
        }
        
        public async Task<IEnumerable<ClienteDto>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Enumerable.Empty<ClienteDto>();
            }
            
            return await _clienteRepository.SearchAsync(term);
        }
        
        public async Task<bool> ValidateCpfCnpjAsync(string cpfCnpj, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(cpfCnpj))
                return false;
            
            var documento = DocumentHelper.LimparDocumento(cpfCnpj);
            
            // Validar formato
            bool isValid = documento.Length == 11 
                ? DocumentHelper.ValidarCpf(documento) 
                : DocumentHelper.ValidarCnpj(documento);
            
            if (!isValid)
                return false;
            
            // Verificar se já existe
            var exists = await _clienteRepository.ExistsByCpfCnpjAsync(documento, excludeId);
            return !exists;
        }
        
        private async Task ValidateClienteAsync(ClienteCreateDto cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.NomeRazaoSocial))
                throw new ArgumentException("Nome/Razão Social é obrigatório");
            
            if (string.IsNullOrWhiteSpace(cliente.CpfCnpj))
                throw new ArgumentException("CPF/CNPJ é obrigatório");
            
            if (string.IsNullOrWhiteSpace(cliente.Telefone1))
                throw new ArgumentException("Telefone principal é obrigatório");
            
            if (!string.IsNullOrWhiteSpace(cliente.Email) && !EmailHelper.IsValidEmail(cliente.Email))
                throw new ArgumentException("Email inválido");
            
            if (cliente.TipoCliente != "PF" && cliente.TipoCliente != "PJ")
                throw new ArgumentException("Tipo de cliente deve ser PF ou PJ");
        }
    }
}
```

## Helpers

### DocumentHelper.cs

```csharp
using System.Text.RegularExpressions;

namespace MecTecERP.Shared.Helpers
{
    public static class DocumentHelper
    {
        public static string LimparDocumento(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                return string.Empty;
            
            return Regex.Replace(documento, @"[^0-9]", "");
        }
        
        public static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;
            
            cpf = LimparDocumento(cpf);
            
            if (cpf.Length != 11)
                return false;
            
            // Verificar se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;
            
            // Calcular primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;
            
            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;
            
            // Calcular segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;
            
            return int.Parse(cpf[10].ToString()) == digito2;
        }
        
        public static bool ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;
            
            cnpj = LimparDocumento(cnpj);
            
            if (cnpj.Length != 14)
                return false;
            
            // Verificar se todos os dígitos são iguais
            if (cnpj.All(c => c == cnpj[0]))
                return false;
            
            // Calcular primeiro dígito verificador
            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;
            
            for (int i = 0; i < 12; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
            }
            
            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;
            
            if (int.Parse(cnpj[12].ToString()) != digito1)
                return false;
            
            // Calcular segundo dígito verificador
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            soma = 0;
            
            for (int i = 0; i < 13; i++)
            {
                soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
            }
            
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;
            
            return int.Parse(cnpj[13].ToString()) == digito2;
        }
    }
}
```

## Páginas Blazor

### Clientes/Index.razor

```razor
@page "/clientes"
@using MecTecERP.Shared.DTOs
@inject IClienteService ClienteService
@inject IJSRuntime JSRuntime
@inject NavigationManager Navigation
@attribute [Authorize]

<PageTitle>Clientes - MecTecERP</PageTitle>

<div class="d-flex justify-content-between align-items-center mb-4">
    <h1 class="h3 mb-0">
        <i class="fas fa-users me-2"></i>
        Clientes
    </h1>
    <a href="/clientes/novo" class="btn btn-primary">
        <i class="fas fa-plus me-1"></i>
        Novo Cliente
    </a>
</div>

<!-- Filtros -->
<div class="card mb-4">
    <div class="card-body">
        <div class="row g-3">
            <div class="col-md-4">
                <label class="form-label">Buscar</label>
                <input type="text" class="form-control" @bind="searchTerm" @onkeypress="OnSearchKeyPress" 
                       placeholder="Nome, CPF/CNPJ ou telefone..." />
            </div>
            <div class="col-md-3">
                <label class="form-label">Status</label>
                <select class="form-select" @bind="statusFilter">
                    <option value="">Todos</option>
                    <option value="true">Ativos</option>
                    <option value="false">Inativos</option>
                </select>
            </div>
            <div class="col-md-3">
                <label class="form-label">Registros por página</label>
                <select class="form-select" @bind="pageSize">
                    <option value="25">25</option>
                    <option value="50">50</option>
                    <option value="100">100</option>
                </select>
            </div>
            <div class="col-md-2 d-flex align-items-end">
                <button class="btn btn-outline-primary w-100" @onclick="LoadClientes">
                    <i class="fas fa-search me-1"></i>
                    Buscar
                </button>
            </div>
        </div>
    </div>
</div>

<!-- Tabela de Clientes -->
@if (loading)
{
    <div class="text-center py-5">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Carregando...</span>
        </div>
    </div>
}
else if (clientes?.Any() == true)
{
    <div class="card">
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-hover mb-0">
                    <thead class="table-light">
                        <tr>
                            <th>Nome/Razão Social</th>
                            <th>Documento</th>
                            <th>Telefone</th>
                            <th>Email</th>
                            <th>Cidade</th>
                            <th>Veículos</th>
                            <th>OS</th>
                            <th>Status</th>
                            <th>Cadastro</th>
                            <th width="120">Ações</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var cliente in clientes)
                        {
                            <tr>
                                <td>
                                    <div class="d-flex align-items-center">
                                        <div class="avatar-sm bg-primary text-white rounded-circle d-flex align-items-center justify-content-center me-2">
                                            @cliente.NomeRazaoSocial.Substring(0, 1).ToUpper()
                                        </div>
                                        <div>
                                            <div class="fw-semibold">@cliente.NomeRazaoSocial</div>
                                            <small class="text-muted">@cliente.TipoCliente</small>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    <span class="font-monospace">@cliente.DocumentoFormatado</span>
                                </td>
                                <td>@cliente.Telefone1</td>
                                <td>@cliente.Email</td>
                                <td>@cliente.Cidade</td>
                                <td>
                                    <span class="badge bg-info">@cliente.TotalVeiculos</span>
                                </td>
                                <td>
                                    <span class="badge bg-secondary">@cliente.TotalOS</span>
                                </td>
                                <td>
                                    @if (cliente.Ativo)
                                    {
                                        <span class="badge bg-success">Ativo</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-danger">Inativo</span>
                                    }
                                </td>
                                <td>
                                    <small class="text-muted">@cliente.DataCadastro.ToString("dd/MM/yyyy")</small>
                                </td>
                                <td>
                                    <div class="dropdown">
                                        <button class="btn btn-sm btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown">
                                            <i class="fas fa-ellipsis-v"></i>
                                        </button>
                                        <ul class="dropdown-menu">
                                            <li><a class="dropdown-item" href="/clientes/@cliente.Id">
                                                <i class="fas fa-eye me-2"></i>Visualizar
                                            </a></li>
                                            <li><a class="dropdown-item" href="/clientes/@cliente.Id/editar">
                                                <i class="fas fa-edit me-2"></i>Editar
                                            </a></li>
                                            <li><a class="dropdown-item" href="/veiculos/novo?clienteId=@cliente.Id">
                                                <i class="fas fa-car me-2"></i>Novo Veículo
                                            </a></li>
                                            <li><hr class="dropdown-divider"></li>
                                            <li><button class="dropdown-item" @onclick="() => ToggleStatus(cliente.Id)">
                                                @if (cliente.Ativo)
                                                {
                                                    <i class="fas fa-ban me-2 text-warning"></i>Inativar
                                                }
                                                else
                                                {
                                                    <i class="fas fa-check me-2 text-success"></i>Ativar
                                                }
                                            </button></li>
                                        </ul>
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
    @if (totalPages > 1)
    {
        <nav class="mt-4">
            <ul class="pagination justify-content-center">
                <li class="page-item @(currentPage == 1 ? "disabled" : "")">
                    <button class="page-link" @onclick="() => ChangePage(1)" disabled="@(currentPage == 1)">
                        <i class="fas fa-angle-double-left"></i>
                    </button>
                </li>
                <li class="page-item @(currentPage == 1 ? "disabled" : "")">
                    <button class="page-link" @onclick="() => ChangePage(currentPage - 1)" disabled="@(currentPage == 1)">
                        <i class="fas fa-angle-left"></i>
                    </button>
                </li>
                
                @for (int i = Math.Max(1, currentPage - 2); i <= Math.Min(totalPages, currentPage + 2); i++)
                {
                    int page = i;
                    <li class="page-item @(currentPage == page ? "active" : "")">
                        <button class="page-link" @onclick="() => ChangePage(page)">@page</button>
                    </li>
                }
                
                <li class="page-item @(currentPage == totalPages ? "disabled" : "")">
                    <button class="page-link" @onclick="() => ChangePage(currentPage + 1)" disabled="@(currentPage == totalPages)">
                        <i class="fas fa-angle-right"></i>
                    </button>
                </li>
                <li class="page-item @(currentPage == totalPages ? "disabled" : "")">
                    <button class="page-link" @onclick="() => ChangePage(totalPages)" disabled="@(currentPage == totalPages)">
                        <i class="fas fa-angle-double-right"></i>
                    </button>
                </li>
            </ul>
        </nav>
        
        <div class="text-center text-muted">
            Mostrando @((currentPage - 1) * pageSize + 1) a @Math.Min(currentPage * pageSize, totalCount) de @totalCount registros
        </div>
    }
}
else
{
    <div class="card">
        <div class="card-body text-center py-5">
            <i class="fas fa-users fa-3x text-muted mb-3"></i>
            <h5>Nenhum cliente encontrado</h5>
            <p class="text-muted">@(string.IsNullOrWhiteSpace(searchTerm) ? "Comece cadastrando seu primeiro cliente." : "Tente ajustar os filtros de busca.")</p>
            @if (string.IsNullOrWhiteSpace(searchTerm))
            {
                <a href="/clientes/novo" class="btn btn-primary">
                    <i class="fas fa-plus me-1"></i>
                    Cadastrar Primeiro Cliente
                </a>
            }
        </div>
    </div>
}

@code {
    private IEnumerable<ClienteListDto>? clientes;
    private bool loading = true;
    private string searchTerm = string.Empty;
    private string statusFilter = string.Empty;
    private int currentPage = 1;
    private int pageSize = 50;
    private int totalCount = 0;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    
    protected override async Task OnInitializedAsync()
    {
        await LoadClientes();
    }
    
    private async Task LoadClientes()
    {
        loading = true;
        StateHasChanged();
        
        try
        {
            bool? ativo = string.IsNullOrEmpty(statusFilter) ? null : bool.Parse(statusFilter);
            var result = await ClienteService.GetAllAsync(currentPage, pageSize, searchTerm, ativo);
            
            clientes = result.clientes;
            totalCount = result.totalCount;
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("showToast", "Erro ao carregar clientes: " + ex.Message, "error");
        }
        finally
        {
            loading = false;
            StateHasChanged();
        }
    }
    
    private async Task OnSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            currentPage = 1;
            await LoadClientes();
        }
    }
    
    private async Task ChangePage(int page)
    {
        if (page >= 1 && page <= totalPages && page != currentPage)
        {
            currentPage = page;
            await LoadClientes();
        }
    }
    
    private async Task ToggleStatus(int clienteId)
    {
        try
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Confirma a alteração do status do cliente?");
            if (confirmed)
            {
                await ClienteService.ToggleStatusAsync(clienteId, GetCurrentUserId());
                await JSRuntime.InvokeVoidAsync("showToast", "Status alterado com sucesso!", "success");
                await LoadClientes();
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("showToast", "Erro ao alterar status: " + ex.Message, "error");
        }
    }
    
    private int GetCurrentUserId()
    {
        // Implementar lógica para obter ID do usuário logado
        return 1; // Placeholder
    }
}
```

## Próximos Passos

1. **Implementar páginas de Create, Edit e Details**
2. **Criar componentes reutilizáveis**
3. **Implementar validações client-side**
4. **Adicionar integração com API dos Correios**
5. **Criar testes unitários**
6. **Implementar auditoria**

Este documento será atualizado conforme o progresso da implementação.