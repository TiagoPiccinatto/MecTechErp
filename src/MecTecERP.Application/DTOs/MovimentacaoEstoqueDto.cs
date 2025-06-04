using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class MovimentacaoEstoqueDto : BaseDto
{
    public int ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }
    public string TipoTexto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string? Documento { get; set; }
    public string? Observacoes { get; set; }
    public int? InventarioId { get; set; }
    public string? InventarioDescricao { get; set; }
    public decimal EstoqueAnterior { get; set; }
    public decimal EstoquePosterior { get; set; }
}

public class MovimentacaoEstoqueCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O produto é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto válido")]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O tipo de movimentação é obrigatório")]
    public TipoMovimentacao Tipo { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero")]
    public decimal ValorUnitario { get; set; }

    [Required(ErrorMessage = "A data da movimentação é obrigatória")]
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;

    [StringLength(50, ErrorMessage = "O documento deve ter no máximo 50 caracteres")]
    public string? Documento { get; set; }

    [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }

    public int? InventarioId { get; set; }
}

public class MovimentacaoEstoqueUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O produto é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto válido")]
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O tipo de movimentação é obrigatório")]
    public TipoMovimentacao Tipo { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.01, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior que zero")]
    public decimal ValorUnitario { get; set; }

    [Required(ErrorMessage = "A data da movimentação é obrigatória")]
    public DateTime DataMovimentacao { get; set; }

    [StringLength(50, ErrorMessage = "O documento deve ter no máximo 50 caracteres")]
    public string? Documento { get; set; }

    [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }

    public int? InventarioId { get; set; }
}

public class MovimentacaoEstoqueListDto
{
    public int Id { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public TipoMovimentacao Tipo { get; set; }
    public string TipoTexto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string? Documento { get; set; }
    public string? InventarioDescricao { get; set; }
    public bool Ativo { get; set; }
}

public class MovimentacaoEstoqueResumoDto
{
    public DateTime Data { get; set; }
    public int TotalMovimentacoes { get; set; }
    public decimal TotalEntradas { get; set; }
    public decimal TotalSaidas { get; set; }
    public decimal ValorTotalEntradas { get; set; }
    public decimal ValorTotalSaidas { get; set; }
}

public class MovimentacaoEstoqueFiltroDto
{
    public int? ProdutoId { get; set; }
    public TipoMovimentacao? Tipo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Documento { get; set; }
    public int? InventarioId { get; set; }
    public bool? Ativo { get; set; }
    public int Pagina { get; set; } = 1;
    public int ItensPorPagina { get; set; } = 20;
    public string? OrdenarPor { get; set; } = "DataMovimentacao";
    public bool OrdemDecrescente { get; set; } = true;
}