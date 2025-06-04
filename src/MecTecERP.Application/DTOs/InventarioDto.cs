using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class InventarioDto : BaseDto
{
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public StatusInventario Status { get; set; }
    public string StatusTexto { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public int TotalItens { get; set; }
    public int ItensContados { get; set; }
    public int ItensPendentes { get; set; }
    public int ItensComDivergencia { get; set; }
    public decimal PercentualConcluido { get; set; }
    public List<InventarioItemDto> Itens { get; set; } = new();
}

public class InventarioCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "A descrição do inventário é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de início é obrigatória")]
    public DateTime DataInicio { get; set; } = DateTime.Now;

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    public List<int> ProdutoIds { get; set; } = new();
    public bool IncluirTodosProdutos { get; set; } = true;
    public int? CategoriaId { get; set; }
    public int? FornecedorId { get; set; }
}

public class InventarioUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "A descrição do inventário é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de início é obrigatória")]
    public DateTime DataInicio { get; set; }

    public DateTime? DataFim { get; set; }

    [Required(ErrorMessage = "O status é obrigatório")]
    public StatusInventario Status { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}

public class InventarioListDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public StatusInventario Status { get; set; }
    public string StatusTexto { get; set; } = string.Empty;
    public int TotalItens { get; set; }
    public int ItensContados { get; set; }
    public int ItensComDivergencia { get; set; }
    public decimal PercentualConcluido { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class InventarioItemDto : BaseDto
{
    public int InventarioId { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal EstoqueSistema { get; set; }
    public decimal? EstoqueContado { get; set; }
    public decimal? Diferenca { get; set; }
    public DateTime? DataContagem { get; set; }
    public string? Observacoes { get; set; }
    public bool Contado { get; set; }
    public bool TemDivergencia { get; set; }
}

public class InventarioItemUpdateDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O estoque contado é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "O estoque contado deve ser maior ou igual a zero")]
    public decimal EstoqueContado { get; set; }

    [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
}

public class InventarioResumoDto
{
    public int TotalInventarios { get; set; }
    public int InventariosAbertos { get; set; }
    public int InventariosFechados { get; set; }
    public int InventariosCancelados { get; set; }
    public int TotalItensInventario { get; set; }
    public int ItensComDivergencia { get; set; }
    public decimal PercentualDivergencia { get; set; }
    public decimal ValorTotalDivergencias { get; set; }
}

public class InventarioFiltroDto
{
    public StatusInventario? Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Descricao { get; set; }
    public bool? Ativo { get; set; }
    public int Pagina { get; set; } = 1;
    public int ItensPorPagina { get; set; } = 20;
    public string? OrdenarPor { get; set; } = "DataInicio";
    public bool OrdemDecrescente { get; set; } = true;
}