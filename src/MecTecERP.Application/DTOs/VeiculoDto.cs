using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs;

public class VeiculoDto : BaseDto // BaseDto tem Id, DataCriacao, DataAtualizacao, Ativo
{
    [Required(ErrorMessage = "O Cliente é obrigatório")]
    public int ClienteId { get; set; }
    public string? ClienteNome { get; set; } // Para exibição

    [Required(ErrorMessage = "A Placa é obrigatória")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "A Placa deve ter 7 ou 8 caracteres")]
    // Considerar regex mais flexível para placas Mercosul e antigas: @"^[A-Z]{3}-?\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$"
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A Marca é obrigatória")]
    [StringLength(50, ErrorMessage = "A Marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "O Modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Ano de Fabricação é obrigatório")]
    [Range(1900, 2050, ErrorMessage = "O Ano de Fabricação deve ser válido")]
    public int AnoFabricacao { get; set; }

    [Range(1900, 2050, ErrorMessage = "O Ano do Modelo deve ser válido")]
    public int? AnoModelo { get; set; }

    [Required(ErrorMessage = "A Cor é obrigatória")]
    [StringLength(30, ErrorMessage = "A Cor deve ter no máximo 30 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [StringLength(17, MinimumLength = 17, ErrorMessage = "O Chassi deve ter 17 caracteres")]
    public string? Chassi { get; set; }

    [Range(0, 9999999, ErrorMessage = "O Km Atual deve ser um valor válido")]
    public int? KmAtual { get; set; }

    [StringLength(30, ErrorMessage = "O Tipo de Combustível deve ter no máximo 30 caracteres")]
    public string? TipoCombustivel { get; set; }

    [StringLength(11, MinimumLength = 9, ErrorMessage = "O Renavam deve ter entre 9 e 11 caracteres")]
    public string? Renavam { get; set; }

    [StringLength(255, ErrorMessage = "O caminho/URL da Foto deve ter no máximo 255 caracteres")]
    public string? Foto { get; set; } // URL ou caminho da foto

    [StringLength(1000, ErrorMessage = "As Observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    // DataCadastro é DataCriacao de BaseDto
    // public DateTime DataCadastro { get; set; }

    public int TotalOrdensServico { get; set; }

    public ICollection<OrdemServicoDto>? OrdensServico { get; set; }
}

public class VeiculoCreateDto
{
    [Required(ErrorMessage = "O Cliente é obrigatório")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "A Placa é obrigatória")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "A Placa deve ter 7 ou 8 caracteres")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A Marca é obrigatória")]
    [StringLength(50, ErrorMessage = "A Marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "O Modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Ano de Fabricação é obrigatório")]
    [Range(1900, 2050, ErrorMessage = "O Ano de Fabricação deve ser válido")]
    public int AnoFabricacao { get; set; }

    [Range(1900, 2050, ErrorMessage = "O Ano do Modelo deve ser válido")]
    public int? AnoModelo { get; set; }

    [Required(ErrorMessage = "A Cor é obrigatória")]
    [StringLength(30, ErrorMessage = "A Cor deve ter no máximo 30 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [StringLength(17, MinimumLength = 17, ErrorMessage = "O Chassi deve ter 17 caracteres")]
    public string? Chassi { get; set; }

    [Range(0, 9999999, ErrorMessage = "O Km Atual deve ser um valor válido")]
    public int? KmAtual { get; set; }

    [StringLength(30, ErrorMessage = "O Tipo de Combustível deve ter no máximo 30 caracteres")]
    public string? TipoCombustivel { get; set; }

    [StringLength(11, MinimumLength = 9, ErrorMessage = "O Renavam deve ter entre 9 e 11 caracteres")]
    public string? Renavam { get; set; }

    [StringLength(255, ErrorMessage = "O caminho/URL da Foto deve ter no máximo 255 caracteres")]
    public string? Foto { get; set; }

    [StringLength(1000, ErrorMessage = "As Observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    public bool Ativo { get; set; } = true;
}

public class VeiculoUpdateDto
{
    [Required(ErrorMessage = "O ID do Veículo é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O Cliente é obrigatório")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "A Placa é obrigatória")]
    [StringLength(8, MinimumLength = 7, ErrorMessage = "A Placa deve ter 7 ou 8 caracteres")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A Marca é obrigatória")]
    [StringLength(50, ErrorMessage = "A Marca deve ter no máximo 50 caracteres")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Modelo é obrigatório")]
    [StringLength(100, ErrorMessage = "O Modelo deve ter no máximo 100 caracteres")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Ano de Fabricação é obrigatório")]
    [Range(1900, 2050, ErrorMessage = "O Ano de Fabricação deve ser válido")]
    public int AnoFabricacao { get; set; }

    [Range(1900, 2050, ErrorMessage = "O Ano do Modelo deve ser válido")]
    public int? AnoModelo { get; set; }

    [Required(ErrorMessage = "A Cor é obrigatória")]
    [StringLength(30, ErrorMessage = "A Cor deve ter no máximo 30 caracteres")]
    public string Cor { get; set; } = string.Empty;

    [StringLength(17, MinimumLength = 17, ErrorMessage = "O Chassi deve ter 17 caracteres")]
    public string? Chassi { get; set; }

    [Range(0, 9999999, ErrorMessage = "O Km Atual deve ser um valor válido")]
    public int? KmAtual { get; set; }

    [StringLength(30, ErrorMessage = "O Tipo de Combustível deve ter no máximo 30 caracteres")]
    public string? TipoCombustivel { get; set; }

    [StringLength(11, MinimumLength = 9, ErrorMessage = "O Renavam deve ter entre 9 e 11 caracteres")]
    public string? Renavam { get; set; }

    [StringLength(255, ErrorMessage = "O caminho/URL da Foto deve ter no máximo 255 caracteres")]
    public string? Foto { get; set; }

    [StringLength(1000, ErrorMessage = "As Observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    public bool Ativo { get; set; }
}

public class VeiculoListDto
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int AnoFabricacao { get; set; } // Alterado de Ano
    public int? AnoModelo { get; set; } // Adicionado
    public string Cor { get; set; } = string.Empty;
    public int? KmAtual { get; set; } // Alterado de Quilometragem
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; } // Mapeado de DataCriacao
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int TotalOrdensServico { get; set; }
}

public class VeiculoFiltroDto : FiltroBaseDto
{
    public string? Placa { get; set; }
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public int? AnoFabricacaoInicio { get; set; } // Alterado de AnoInicio
    public int? AnoFabricacaoFim { get; set; } // Alterado de AnoFim
    public string? Cor { get; set; }
    public string? TipoCombustivel { get; set; } // Alterado de Combustivel
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; } // Para buscar pelo nome do cliente
    public DateTime? DataCadastroInicio { get; set; }
    public DateTime? DataCadastroFim { get; set; }
    public bool? Ativo { get; set; } // Adicionado
}