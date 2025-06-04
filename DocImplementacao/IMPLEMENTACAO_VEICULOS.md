# Implementação do Módulo de Veículos - MecTecERP

## Visão Geral

O módulo de Veículos é responsável pelo cadastro e gerenciamento dos veículos dos clientes. Este módulo está diretamente vinculado ao módulo de Clientes e serve como base para as Ordens de Serviço.

## Estrutura de Dados

### Tabela: Veiculos

```sql
CREATE TABLE Veiculos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Placa NVARCHAR(8) NOT NULL UNIQUE,
    Marca NVARCHAR(50) NOT NULL,
    Modelo NVARCHAR(100) NOT NULL,
    AnoFabricacao INT NOT NULL,
    AnoModelo INT NOT NULL,
    Cor NVARCHAR(30) NOT NULL,
    Chassi NVARCHAR(17) NULL,
    KmAtual INT NOT NULL DEFAULT 0,
    TipoCombustivel NVARCHAR(20) NOT NULL,
    Foto NVARCHAR(500) NULL,
    Observacoes NVARCHAR(MAX) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE(),
    DataUltimaAtualizacao DATETIME2 NULL,
    UsuarioCadastro INT NOT NULL,
    UsuarioUltimaAtualizacao INT NULL,
    
    CONSTRAINT FK_Veiculos_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    CONSTRAINT FK_Veiculos_UsuarioCadastro FOREIGN KEY (UsuarioCadastro) REFERENCES AspNetUsers(Id),
    CONSTRAINT FK_Veiculos_UsuarioAtualizacao FOREIGN KEY (UsuarioUltimaAtualizacao) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_Veiculos_AnoFabricacao CHECK (AnoFabricacao >= 1900 AND AnoFabricacao <= YEAR(GETDATE()) + 1),
    CONSTRAINT CK_Veiculos_AnoModelo CHECK (AnoModelo >= 1900 AND AnoModelo <= YEAR(GETDATE()) + 1),
    CONSTRAINT CK_Veiculos_KmAtual CHECK (KmAtual >= 0)
);

-- Índices para performance
CREATE INDEX IX_Veiculos_ClienteId ON Veiculos(ClienteId);
CREATE INDEX IX_Veiculos_Placa ON Veiculos(Placa);
CREATE INDEX IX_Veiculos_Marca ON Veiculos(Marca);
CREATE INDEX IX_Veiculos_Modelo ON Veiculos(Modelo);
CREATE INDEX IX_Veiculos_Ativo ON Veiculos(Ativo);
CREATE INDEX IX_Veiculos_DataCadastro ON Veiculos(DataCadastro);
```

### Tabela: HistoricoKm

```sql
CREATE TABLE HistoricoKm (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VeiculoId INT NOT NULL,
    KmAnterior INT NOT NULL,
    KmAtual INT NOT NULL,
    DataRegistro DATETIME2 NOT NULL DEFAULT GETDATE(),
    Observacao NVARCHAR(200) NULL,
    UsuarioRegistro INT NOT NULL,
    
    CONSTRAINT FK_HistoricoKm_Veiculo FOREIGN KEY (VeiculoId) REFERENCES Veiculos(Id),
    CONSTRAINT FK_HistoricoKm_Usuario FOREIGN KEY (UsuarioRegistro) REFERENCES AspNetUsers(Id),
    CONSTRAINT CK_HistoricoKm_Valores CHECK (KmAtual >= KmAnterior)
);

CREATE INDEX IX_HistoricoKm_VeiculoId ON HistoricoKm(VeiculoId);
CREATE INDEX IX_HistoricoKm_DataRegistro ON HistoricoKm(DataRegistro);
```

## Entidades do Domínio

### Veiculo.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Domain.Entities
{
    public class Veiculo
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "Placa é obrigatória")]
        [StringLength(8, ErrorMessage = "Placa deve ter no máximo 8 caracteres")]
        public string Placa { get; set; }
        
        [Required(ErrorMessage = "Marca é obrigatória")]
        [StringLength(50, ErrorMessage = "Marca não pode exceder 50 caracteres")]
        public string Marca { get; set; }
        
        [Required(ErrorMessage = "Modelo é obrigatório")]
        [StringLength(100, ErrorMessage = "Modelo não pode exceder 100 caracteres")]
        public string Modelo { get; set; }
        
        [Required(ErrorMessage = "Ano de fabricação é obrigatório")]
        [Range(1900, 2030, ErrorMessage = "Ano de fabricação inválido")]
        public int AnoFabricacao { get; set; }
        
        [Required(ErrorMessage = "Ano do modelo é obrigatório")]
        [Range(1900, 2030, ErrorMessage = "Ano do modelo inválido")]
        public int AnoModelo { get; set; }
        
        [Required(ErrorMessage = "Cor é obrigatória")]
        [StringLength(30, ErrorMessage = "Cor não pode exceder 30 caracteres")]
        public string Cor { get; set; }
        
        [StringLength(17, ErrorMessage = "Chassi deve ter no máximo 17 caracteres")]
        public string? Chassi { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Quilometragem deve ser positiva")]
        public int KmAtual { get; set; }
        
        [Required(ErrorMessage = "Tipo de combustível é obrigatório")]
        [StringLength(20, ErrorMessage = "Tipo de combustível não pode exceder 20 caracteres")]
        public string TipoCombustivel { get; set; }
        
        [StringLength(500, ErrorMessage = "Caminho da foto não pode exceder 500 caracteres")]
        public string? Foto { get; set; }
        
        public string? Observacoes { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        public int UsuarioCadastro { get; set; }
        
        public int? UsuarioUltimaAtualizacao { get; set; }
        
        // Navigation Properties
        public Cliente? Cliente { get; set; }
        
        // Propriedades calculadas
        public string PlacaFormatada => FormatarPlaca(Placa);
        
        public string VeiculoCompleto => $"{Marca} {Modelo} {AnoModelo} - {PlacaFormatada}";
        
        public string AnoCompleto => AnoFabricacao == AnoModelo 
            ? AnoModelo.ToString() 
            : $"{AnoFabricacao}/{AnoModelo}";
        
        public string KmFormatado => KmAtual.ToString("N0") + " km";
        
        // Métodos auxiliares
        private static string FormatarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa)) return placa;
            
            placa = placa.ToUpper().Replace("-", "").Replace(" ", "");
            
            // Placa Mercosul (7 caracteres)
            if (placa.Length == 7)
            {
                return $"{placa.Substring(0, 3)}{placa.Substring(3, 1)}{placa.Substring(4, 1)}{placa.Substring(5, 2)}";
            }
            
            // Placa antiga (7 caracteres)
            if (placa.Length == 7)
            {
                return $"{placa.Substring(0, 3)}-{placa.Substring(3, 4)}";
            }
            
            return placa;
        }
    }
    
    public class HistoricoKm
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public int KmAnterior { get; set; }
        public int KmAtual { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public string? Observacao { get; set; }
        public int UsuarioRegistro { get; set; }
        
        // Navigation Properties
        public Veiculo? Veiculo { get; set; }
        
        // Propriedades calculadas
        public int DiferencaKm => KmAtual - KmAnterior;
        public string DiferencaKmFormatada => DiferencaKm.ToString("N0") + " km";
    }
}
```

## DTOs e ViewModels

### VeiculoDto.cs

```csharp
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Shared.DTOs
{
    public class VeiculoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Cor { get; set; }
        public string? Chassi { get; set; }
        public int KmAtual { get; set; }
        public string TipoCombustivel { get; set; }
        public string? Foto { get; set; }
        public string? Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimaAtualizacao { get; set; }
        
        // Propriedades calculadas
        public string PlacaFormatada { get; set; }
        public string VeiculoCompleto { get; set; }
        public string AnoCompleto { get; set; }
        public string KmFormatado { get; set; }
    }
    
    public class VeiculoCreateDto
    {
        [Required(ErrorMessage = "Cliente é obrigatório")]
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "Placa é obrigatória")]
        [StringLength(8)]
        public string Placa { get; set; }
        
        [Required(ErrorMessage = "Marca é obrigatória")]
        [StringLength(50)]
        public string Marca { get; set; }
        
        [Required(ErrorMessage = "Modelo é obrigatório")]
        [StringLength(100)]
        public string Modelo { get; set; }
        
        [Required(ErrorMessage = "Ano de fabricação é obrigatório")]
        [Range(1900, 2030)]
        public int AnoFabricacao { get; set; }
        
        [Required(ErrorMessage = "Ano do modelo é obrigatório")]
        [Range(1900, 2030)]
        public int AnoModelo { get; set; }
        
        [Required(ErrorMessage = "Cor é obrigatória")]
        [StringLength(30)]
        public string Cor { get; set; }
        
        [StringLength(17)]
        public string? Chassi { get; set; }
        
        [Range(0, int.MaxValue)]
        public int KmAtual { get; set; }
        
        [Required(ErrorMessage = "Tipo de combustível é obrigatório")]
        public string TipoCombustivel { get; set; }
        
        public string? Observacoes { get; set; }
    }
    
    public class VeiculoUpdateDto : VeiculoCreateDto
    {
        public int Id { get; set; }
    }
    
    public class VeiculoListDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; }
        public string PlacaFormatada { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string AnoCompleto { get; set; }
        public string Cor { get; set; }
        public string KmFormatado { get; set; }
        public string TipoCombustivel { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int TotalOS { get; set; }
        public DateTime? UltimaOS { get; set; }
    }
    
    public class VeiculoSearchDto
    {
        public int Id { get; set; }
        public string VeiculoCompleto { get; set; }
        public string PlacaFormatada { get; set; }
        public string ClienteNome { get; set; }
    }
    
    public class HistoricoKmDto
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public int KmAnterior { get; set; }
        public int KmAtual { get; set; }
        public DateTime DataRegistro { get; set; }
        public string? Observacao { get; set; }
        public string UsuarioNome { get; set; }
        public int DiferencaKm { get; set; }
        public string DiferencaKmFormatada { get; set; }
    }
}
```

## Repositório

### IVeiculoRepository.cs

```csharp
using MecTecERP.Domain.Entities;
using MecTecERP.Shared.DTOs;

namespace MecTecERP.Domain.Interfaces
{
    public interface IVeiculoRepository
    {
        Task<IEnumerable<VeiculoListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, int? clienteId = null, bool? ativo = null);
        Task<VeiculoDto?> GetByIdAsync(int id);
        Task<Veiculo?> GetEntityByIdAsync(int id);
        Task<IEnumerable<VeiculoDto>> GetByClienteIdAsync(int clienteId);
        Task<bool> ExistsByPlacaAsync(string placa, int? excludeId = null);
        Task<int> CreateAsync(VeiculoCreateDto veiculo, int usuarioId);
        Task<bool> UpdateAsync(VeiculoUpdateDto veiculo, int usuarioId);
        Task<bool> UpdateKmAsync(int veiculoId, int novoKm, string? observacao, int usuarioId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleStatusAsync(int id, int usuarioId);
        Task<int> GetTotalCountAsync(string? search = null, int? clienteId = null, bool? ativo = null);
        Task<IEnumerable<VeiculoSearchDto>> SearchAsync(string term);
        Task<IEnumerable<HistoricoKmDto>> GetHistoricoKmAsync(int veiculoId);
    }
}
```

### VeiculoRepository.cs

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Shared.DTOs;
using System.Data;

namespace MecTecERP.Infrastructure.Repositories
{
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly string _connectionString;
        
        public VeiculoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        
        public async Task<IEnumerable<VeiculoListDto>> GetAllAsync(int page = 1, int pageSize = 50, string? search = null, int? clienteId = null, bool? ativo = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    v.Id,
                    v.ClienteId,
                    c.NomeRazaoSocial as ClienteNome,
                    v.Placa,
                    v.Marca,
                    v.Modelo,
                    v.AnoFabricacao,
                    v.AnoModelo,
                    v.Cor,
                    v.KmAtual,
                    v.TipoCombustivel,
                    v.Ativo,
                    v.DataCadastro,
                    COUNT(DISTINCT os.Id) as TotalOS,
                    MAX(os.DataEntrada) as UltimaOS
                FROM Veiculos v
                INNER JOIN Clientes c ON v.ClienteId = c.Id
                LEFT JOIN OrdemServico os ON v.Id = os.VeiculoId
                WHERE (@Search IS NULL OR v.Placa LIKE '%' + @Search + '%' 
                       OR v.Marca LIKE '%' + @Search + '%'
                       OR v.Modelo LIKE '%' + @Search + '%'
                       OR c.NomeRazaoSocial LIKE '%' + @Search + '%')
                  AND (@ClienteId IS NULL OR v.ClienteId = @ClienteId)
                  AND (@Ativo IS NULL OR v.Ativo = @Ativo)
                GROUP BY v.Id, v.ClienteId, c.NomeRazaoSocial, v.Placa, v.Marca, v.Modelo,
                         v.AnoFabricacao, v.AnoModelo, v.Cor, v.KmAtual, v.TipoCombustivel,
                         v.Ativo, v.DataCadastro
                ORDER BY c.NomeRazaoSocial, v.Marca, v.Modelo
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
            var parameters = new
            {
                Search = search,
                ClienteId = clienteId,
                Ativo = ativo,
                Offset = (page - 1) * pageSize,
                PageSize = pageSize
            };
            
            var result = await connection.QueryAsync<VeiculoListDto>(sql, parameters);
            
            // Formatar dados
            foreach (var veiculo in result)
            {
                veiculo.PlacaFormatada = FormatarPlaca(veiculo.Placa);
                veiculo.AnoCompleto = veiculo.AnoFabricacao == veiculo.AnoModelo 
                    ? veiculo.AnoModelo.ToString() 
                    : $"{veiculo.AnoFabricacao}/{veiculo.AnoModelo}";
                veiculo.KmFormatado = veiculo.KmAtual.ToString("N0") + " km";
            }
            
            return result;
        }
        
        public async Task<VeiculoDto?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    v.*,
                    c.NomeRazaoSocial as ClienteNome
                FROM Veiculos v
                INNER JOIN Clientes c ON v.ClienteId = c.Id
                WHERE v.Id = @Id";
            
            var veiculo = await connection.QueryFirstOrDefaultAsync<VeiculoDto>(sql, new { Id = id });
            
            if (veiculo != null)
            {
                veiculo.PlacaFormatada = FormatarPlaca(veiculo.Placa);
                veiculo.VeiculoCompleto = $"{veiculo.Marca} {veiculo.Modelo} {veiculo.AnoModelo} - {veiculo.PlacaFormatada}";
                veiculo.AnoCompleto = veiculo.AnoFabricacao == veiculo.AnoModelo 
                    ? veiculo.AnoModelo.ToString() 
                    : $"{veiculo.AnoFabricacao}/{veiculo.AnoModelo}";
                veiculo.KmFormatado = veiculo.KmAtual.ToString("N0") + " km";
            }
            
            return veiculo;
        }
        
        public async Task<Veiculo?> GetEntityByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT * FROM Veiculos WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Veiculo>(sql, new { Id = id });
        }
        
        public async Task<IEnumerable<VeiculoDto>> GetByClienteIdAsync(int clienteId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    v.*,
                    c.NomeRazaoSocial as ClienteNome
                FROM Veiculos v
                INNER JOIN Clientes c ON v.ClienteId = c.Id
                WHERE v.ClienteId = @ClienteId AND v.Ativo = 1
                ORDER BY v.Marca, v.Modelo";
            
            var veiculos = await connection.QueryAsync<VeiculoDto>(sql, new { ClienteId = clienteId });
            
            // Formatar dados
            foreach (var veiculo in veiculos)
            {
                veiculo.PlacaFormatada = FormatarPlaca(veiculo.Placa);
                veiculo.VeiculoCompleto = $"{veiculo.Marca} {veiculo.Modelo} {veiculo.AnoModelo} - {veiculo.PlacaFormatada}";
                veiculo.AnoCompleto = veiculo.AnoFabricacao == veiculo.AnoModelo 
                    ? veiculo.AnoModelo.ToString() 
                    : $"{veiculo.AnoFabricacao}/{veiculo.AnoModelo}";
                veiculo.KmFormatado = veiculo.KmAtual.ToString("N0") + " km";
            }
            
            return veiculos;
        }
        
        public async Task<bool> ExistsByPlacaAsync(string placa, int? excludeId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = "SELECT COUNT(1) FROM Veiculos WHERE Placa = @Placa AND (@ExcludeId IS NULL OR Id != @ExcludeId)";
            var count = await connection.QueryFirstAsync<int>(sql, new { Placa = placa.ToUpper(), ExcludeId = excludeId });
            
            return count > 0;
        }
        
        public async Task<int> CreateAsync(VeiculoCreateDto veiculo, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var sql = @"
                    INSERT INTO Veiculos (
                        ClienteId, Placa, Marca, Modelo, AnoFabricacao, AnoModelo,
                        Cor, Chassi, KmAtual, TipoCombustivel, Observacoes, UsuarioCadastro
                    ) VALUES (
                        @ClienteId, @Placa, @Marca, @Modelo, @AnoFabricacao, @AnoModelo,
                        @Cor, @Chassi, @KmAtual, @TipoCombustivel, @Observacoes, @UsuarioCadastro
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                
                var parameters = new
                {
                    veiculo.ClienteId,
                    Placa = veiculo.Placa.ToUpper(),
                    veiculo.Marca,
                    veiculo.Modelo,
                    veiculo.AnoFabricacao,
                    veiculo.AnoModelo,
                    veiculo.Cor,
                    veiculo.Chassi,
                    veiculo.KmAtual,
                    veiculo.TipoCombustivel,
                    veiculo.Observacoes,
                    UsuarioCadastro = usuarioId
                };
                
                var veiculoId = await connection.QueryFirstAsync<int>(sql, parameters, transaction);
                
                // Registrar histórico inicial de KM
                if (veiculo.KmAtual > 0)
                {
                    var historicoSql = @"
                        INSERT INTO HistoricoKm (VeiculoId, KmAnterior, KmAtual, Observacao, UsuarioRegistro)
                        VALUES (@VeiculoId, 0, @KmAtual, 'Quilometragem inicial', @UsuarioRegistro)";
                    
                    await connection.ExecuteAsync(historicoSql, new
                    {
                        VeiculoId = veiculoId,
                        veiculo.KmAtual,
                        UsuarioRegistro = usuarioId
                    }, transaction);
                }
                
                transaction.Commit();
                return veiculoId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public async Task<bool> UpdateAsync(VeiculoUpdateDto veiculo, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Obter KM atual para comparação
                var kmAtualSql = "SELECT KmAtual FROM Veiculos WHERE Id = @Id";
                var kmAtual = await connection.QueryFirstAsync<int>(kmAtualSql, new { veiculo.Id }, transaction);
                
                var sql = @"
                    UPDATE Veiculos SET
                        ClienteId = @ClienteId,
                        Placa = @Placa,
                        Marca = @Marca,
                        Modelo = @Modelo,
                        AnoFabricacao = @AnoFabricacao,
                        AnoModelo = @AnoModelo,
                        Cor = @Cor,
                        Chassi = @Chassi,
                        KmAtual = @KmAtual,
                        TipoCombustivel = @TipoCombustivel,
                        Observacoes = @Observacoes,
                        DataUltimaAtualizacao = GETDATE(),
                        UsuarioUltimaAtualizacao = @UsuarioId
                    WHERE Id = @Id";
                
                var parameters = new
                {
                    veiculo.Id,
                    veiculo.ClienteId,
                    Placa = veiculo.Placa.ToUpper(),
                    veiculo.Marca,
                    veiculo.Modelo,
                    veiculo.AnoFabricacao,
                    veiculo.AnoModelo,
                    veiculo.Cor,
                    veiculo.Chassi,
                    veiculo.KmAtual,
                    veiculo.TipoCombustivel,
                    veiculo.Observacoes,
                    UsuarioId = usuarioId
                };
                
                var rowsAffected = await connection.ExecuteAsync(sql, parameters, transaction);
                
                // Se a quilometragem mudou, registrar no histórico
                if (veiculo.KmAtual != kmAtual)
                {
                    var historicoSql = @"
                        INSERT INTO HistoricoKm (VeiculoId, KmAnterior, KmAtual, Observacao, UsuarioRegistro)
                        VALUES (@VeiculoId, @KmAnterior, @KmAtual, 'Atualização manual', @UsuarioRegistro)";
                    
                    await connection.ExecuteAsync(historicoSql, new
                    {
                        VeiculoId = veiculo.Id,
                        KmAnterior = kmAtual,
                        KmAtual = veiculo.KmAtual,
                        UsuarioRegistro = usuarioId
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
        
        public async Task<bool> UpdateKmAsync(int veiculoId, int novoKm, string? observacao, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Obter KM atual
                var kmAtualSql = "SELECT KmAtual FROM Veiculos WHERE Id = @Id";
                var kmAtual = await connection.QueryFirstAsync<int>(kmAtualSql, new { Id = veiculoId }, transaction);
                
                if (novoKm < kmAtual)
                {
                    throw new ArgumentException("A nova quilometragem não pode ser menor que a atual");
                }
                
                // Atualizar quilometragem do veículo
                var updateSql = @"
                    UPDATE Veiculos SET 
                        KmAtual = @NovoKm,
                        DataUltimaAtualizacao = GETDATE(),
                        UsuarioUltimaAtualizacao = @UsuarioId
                    WHERE Id = @VeiculoId";
                
                await connection.ExecuteAsync(updateSql, new
                {
                    VeiculoId = veiculoId,
                    NovoKm = novoKm,
                    UsuarioId = usuarioId
                }, transaction);
                
                // Registrar no histórico
                var historicoSql = @"
                    INSERT INTO HistoricoKm (VeiculoId, KmAnterior, KmAtual, Observacao, UsuarioRegistro)
                    VALUES (@VeiculoId, @KmAnterior, @KmAtual, @Observacao, @UsuarioRegistro)";
                
                await connection.ExecuteAsync(historicoSql, new
                {
                    VeiculoId = veiculoId,
                    KmAnterior = kmAtual,
                    KmAtual = novoKm,
                    Observacao = observacao ?? "Atualização de quilometragem",
                    UsuarioRegistro = usuarioId
                }, transaction);
                
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            
            // Verificar se tem OS vinculadas
            var checkSql = "SELECT COUNT(1) FROM OrdemServico WHERE VeiculoId = @Id";
            var count = await connection.QueryFirstAsync<int>(checkSql, new { Id = id });
            
            if (count > 0)
            {
                // Se tem dependências, apenas inativar
                return await ToggleStatusAsync(id, 0);
            }
            
            // Se não tem dependências, pode excluir
            var deleteSql = "DELETE FROM Veiculos WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(deleteSql, new { Id = id });
            
            return rowsAffected > 0;
        }
        
        public async Task<bool> ToggleStatusAsync(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                UPDATE Veiculos SET 
                    Ativo = CASE WHEN Ativo = 1 THEN 0 ELSE 1 END,
                    DataUltimaAtualizacao = GETDATE(),
                    UsuarioUltimaAtualizacao = @UsuarioId
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UsuarioId = usuarioId });
            return rowsAffected > 0;
        }
        
        public async Task<int> GetTotalCountAsync(string? search = null, int? clienteId = null, bool? ativo = null)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT COUNT(1) FROM Veiculos v
                INNER JOIN Clientes c ON v.ClienteId = c.Id
                WHERE (@Search IS NULL OR v.Placa LIKE '%' + @Search + '%' 
                       OR v.Marca LIKE '%' + @Search + '%'
                       OR v.Modelo LIKE '%' + @Search + '%'
                       OR c.NomeRazaoSocial LIKE '%' + @Search + '%')
                  AND (@ClienteId IS NULL OR v.ClienteId = @ClienteId)
                  AND (@Ativo IS NULL OR v.Ativo = @Ativo)";
            
            return await connection.QueryFirstAsync<int>(sql, new { Search = search, ClienteId = clienteId, Ativo = ativo });
        }
        
        public async Task<IEnumerable<VeiculoSearchDto>> SearchAsync(string term)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT TOP 10
                    v.Id,
                    v.Placa,
                    v.Marca,
                    v.Modelo,
                    v.AnoModelo,
                    c.NomeRazaoSocial as ClienteNome
                FROM Veiculos v
                INNER JOIN Clientes c ON v.ClienteId = c.Id
                WHERE v.Ativo = 1 
                  AND (v.Placa LIKE '%' + @Term + '%' 
                       OR v.Marca LIKE '%' + @Term + '%'
                       OR v.Modelo LIKE '%' + @Term + '%'
                       OR c.NomeRazaoSocial LIKE '%' + @Term + '%')
                ORDER BY c.NomeRazaoSocial, v.Marca, v.Modelo";
            
            var result = await connection.QueryAsync<VeiculoSearchDto>(sql, new { Term = term });
            
            // Formatar dados
            foreach (var veiculo in result)
            {
                veiculo.PlacaFormatada = FormatarPlaca(veiculo.Placa);
                veiculo.VeiculoCompleto = $"{veiculo.Marca} {veiculo.Modelo} {veiculo.AnoModelo} - {veiculo.PlacaFormatada}";
            }
            
            return result;
        }
        
        public async Task<IEnumerable<HistoricoKmDto>> GetHistoricoKmAsync(int veiculoId)
        {
            using var connection = new SqlConnection(_connectionString);
            
            var sql = @"
                SELECT 
                    h.*,
                    u.UserName as UsuarioNome
                FROM HistoricoKm h
                INNER JOIN AspNetUsers u ON h.UsuarioRegistro = u.Id
                WHERE h.VeiculoId = @VeiculoId
                ORDER BY h.DataRegistro DESC";
            
            var result = await connection.QueryAsync<HistoricoKmDto>(sql, new { VeiculoId = veiculoId });
            
            // Calcular diferenças
            foreach (var item in result)
            {
                item.DiferencaKm = item.KmAtual - item.KmAnterior;
                item.DiferencaKmFormatada = item.DiferencaKm.ToString("N0") + " km";
            }
            
            return result;
        }
        
        private static string FormatarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa)) return placa;
            
            placa = placa.ToUpper().Replace("-", "").Replace(" ", "");
            
            if (placa.Length == 7)
            {
                return $"{placa.Substring(0, 3)}-{placa.Substring(3, 4)}";
            }
            
            return placa;
        }
    }
}
```

## Próximos Passos

1. **Implementar serviços de domínio**
2. **Criar páginas Blazor**
3. **Implementar upload de fotos**
4. **Criar validações de placa**
5. **Implementar relatórios de veículos**

Este documento será atualizado conforme o progresso da implementação.