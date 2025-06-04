using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class OrdemServicoDto : BaseDto
{
    public string Numero { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? DefeitoRelatado { get; set; }
    public string? SolucaoAplicada { get; set; }
    public decimal ValorMaoObra { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? Desconto { get; set; }
    public string? Observacoes { get; set; }
    public int? Quilometragem { get; set; }
    public DateTime? DataPrevisaoEntrega { get; set; }
    public string? ResponsavelTecnico { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int VeiculoId { get; set; }
    public string VeiculoPlaca { get; set; } = string.Empty;
    public string VeiculoModelo { get; set; } = string.Empty;
    public List<OrdemServicoItemDto> Itens { get; set; } = new();
}

public class OrdemServicoCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O número da ordem de serviço é obrigatório")]
    [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "O defeito relatado deve ter no máximo 1000 caracteres")]
    public string? DefeitoRelatado { get; set; }

    [StringLength(1000, ErrorMessage = "A solução aplicada deve ter no máximo 1000 caracteres")]
    public string? SolucaoAplicada { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor da mão de obra deve ser maior ou igual a zero")]
    public decimal ValorMaoObra { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor das peças deve ser maior ou igual a zero")]
    public decimal ValorPecas { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto deve ser maior ou igual a zero")]
    public decimal? Desconto { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    [Range(0, 9999999, ErrorMessage = "A quilometragem deve ser um valor válido")]
    public int? Quilometragem { get; set; }

    public DateTime? DataPrevisaoEntrega { get; set; }

    [StringLength(100, ErrorMessage = "O responsável técnico deve ter no máximo 100 caracteres")]
    public string? ResponsavelTecnico { get; set; }

    [Required(ErrorMessage = "O cliente é obrigatório")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O veículo é obrigatório")]
    public int VeiculoId { get; set; }

    public List<OrdemServicoItemCreateDto> Itens { get; set; } = new();
}

public class OrdemServicoUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O número da ordem de serviço é obrigatório")]
    [StringLength(20, ErrorMessage = "O número deve ter no máximo 20 caracteres")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "O status é obrigatório")]
    public StatusOrdemServico Status { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "O defeito relatado deve ter no máximo 1000 caracteres")]
    public string? DefeitoRelatado { get; set; }

    [StringLength(1000, ErrorMessage = "A solução aplicada deve ter no máximo 1000 caracteres")]
    public string? SolucaoAplicada { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor da mão de obra deve ser maior ou igual a zero")]
    public decimal ValorMaoObra { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O valor das peças deve ser maior ou igual a zero")]
    public decimal ValorPecas { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto deve ser maior ou igual a zero")]
    public decimal? Desconto { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    [Range(0, 9999999, ErrorMessage = "A quilometragem deve ser um valor válido")]
    public int? Quilometragem { get; set; }

    public DateTime? DataPrevisaoEntrega { get; set; }

    [StringLength(100, ErrorMessage = "O responsável técnico deve ter no máximo 100 caracteres")]
    public string? ResponsavelTecnico { get; set; }

    [Required(ErrorMessage = "O cliente é obrigatório")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O veículo é obrigatório")]
    public int VeiculoId { get; set; }

    public List<OrdemServicoItemUpdateDto> Itens { get; set; } = new();
}

public class OrdemServicoListDto
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public StatusOrdemServico Status { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public bool Ativo { get; set; }
    public DateTime? DataPrevisaoEntrega { get; set; }
    public string? ResponsavelTecnico { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int VeiculoId { get; set; }
    public string VeiculoPlaca { get; set; } = string.Empty;
    public string VeiculoModelo { get; set; } = string.Empty;
}

public class OrdemServicoFiltroDto : FiltroBaseDto
{
    public string? Numero { get; set; }
    public StatusOrdemServico? Status { get; set; }
    public DateTime? DataAberturaInicio { get; set; }
    public DateTime? DataAberturaFim { get; set; }
    public DateTime? DataFechamentoInicio { get; set; }
    public DateTime? DataFechamentoFim { get; set; }
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public int? VeiculoId { get; set; }
    public string? VeiculoPlaca { get; set; }
    public string? ResponsavelTecnico { get; set; }
    public decimal? ValorMinimoTotal { get; set; }
    public decimal? ValorMaximoTotal { get; set; }
}

// DTOs para itens da ordem de serviço
public class OrdemServicoItemDto
{
    public int Id { get; set; }
    public TipoItemOrdemServico Tipo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal? Desconto { get; set; }
    public int? ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int OrdemServicoId { get; set; }
}

public class OrdemServicoItemCreateDto
{
    [Required(ErrorMessage = "O tipo do item é obrigatório")]
    public TipoItemOrdemServico Tipo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero")]
    public decimal ValorUnitario { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto deve ser maior ou igual a zero")]
    public decimal? Desconto { get; set; }

    public int? ProdutoId { get; set; }
}

public class OrdemServicoItemUpdateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O tipo do item é obrigatório")]
    public TipoItemOrdemServico Tipo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero")]
    public decimal ValorUnitario { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto deve ser maior ou igual a zero")]
    public decimal? Desconto { get; set; }

    public int? ProdutoId { get; set; }
}