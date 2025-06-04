using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs;

public class VeiculoDto : BaseDto
{
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public string? Chassi { get; set; }
    public string? Renavam { get; set; }
    public string? Combustivel { get; set; }
    public int? Quilometragem { get; set; }
    public DateTime DataCadastro { get; set; }
    public string? Observacoes { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int TotalOrdensServico { get; set; }
}

public class VeiculoCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "A placa é obrigatória")]
    [StringLength(8, ErrorMessage = "A placa deve ter no máximo 8 caracteres")]
    [RegularExpression(@"^[A-Z]{3}\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$", ErrorMessage = "Placa deve estar no formato ABC1234 ou ABC1D23")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A marca é obrigatória")]
    [StringLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório")]
    [Range(1900, 2030, ErrorMessage = "O ano deve estar entre 1900 e 2030")]
    public int Ano { get; set; }

    [Required(ErrorMessage = "A cor é obrigatória")]
    [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [StringLength(17, ErrorMessage = "O chassi deve ter no máximo 17 caracteres")]
    public string? Chassi { get; set; }

    [StringLength(11, ErrorMessage = "O Renavam deve ter no máximo 11 caracteres")]
    public string? Renavam { get; set; }

    [StringLength(20, ErrorMessage = "O combustível deve ter no máximo 20 caracteres")]
    public string? Combustivel { get; set; }

    [Range(0, 9999999, ErrorMessage = "A quilometragem deve ser um valor válido")]
    public int? Quilometragem { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    [Required(ErrorMessage = "O cliente é obrigatório")]
    public int ClienteId { get; set; }
}

public class VeiculoUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "A placa é obrigatória")]
    [StringLength(8, ErrorMessage = "A placa deve ter no máximo 8 caracteres")]
    [RegularExpression(@"^[A-Z]{3}\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$", ErrorMessage = "Placa deve estar no formato ABC1234 ou ABC1D23")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A marca é obrigatória")]
    [StringLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "O modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório")]
    [Range(1900, 2030, ErrorMessage = "O ano deve estar entre 1900 e 2030")]
    public int Ano { get; set; }

    [Required(ErrorMessage = "A cor é obrigatória")]
    [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [StringLength(17, ErrorMessage = "O chassi deve ter no máximo 17 caracteres")]
    public string? Chassi { get; set; }

    [StringLength(11, ErrorMessage = "O Renavam deve ter no máximo 11 caracteres")]
    public string? Renavam { get; set; }

    [StringLength(20, ErrorMessage = "O combustível deve ter no máximo 20 caracteres")]
    public string? Combustivel { get; set; }

    [Range(0, 9999999, ErrorMessage = "A quilometragem deve ser um valor válido")]
    public int? Quilometragem { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    [Required(ErrorMessage = "O cliente é obrigatório")]
    public int ClienteId { get; set; }
}

public class VeiculoListDto
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public string Cor { get; set; } = string.Empty;
    public int? Quilometragem { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int TotalOrdensServico { get; set; }
}

public class VeiculoFiltroDto : FiltroBaseDto
{
    public string? Placa { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? AnoInicio { get; set; }
    public int? AnoFim { get; set; }
    public string? Cor { get; set; }
    public string? Combustivel { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime? DataCadastroInicio { get; set; }
    public DateTime? DataCadastroFim { get; set; }
}