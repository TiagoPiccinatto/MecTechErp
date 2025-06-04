using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class ProdutoFiltroDto : FiltroBaseDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Sku { get; set; }
    public int? CategoriaId { get; set; }
    public int? FornecedorId { get; set; }
    public decimal? PrecoVendaMinimo { get; set; }
    public decimal? PrecoVendaMaximo { get; set; }
    public decimal? PrecoCustoMinimo { get; set; }
    public decimal? PrecoCustoMaximo { get; set; }
    public decimal? EstoqueMinimo { get; set; }
    public decimal? EstoqueMaximo { get; set; }
    public UnidadeMedida? UnidadeMedida { get; set; }
    public new bool? Ativo { get; set; }
    public DateTime? DataCriacaoInicio { get; set; }
    public DateTime? DataCriacaoFim { get; set; }
}