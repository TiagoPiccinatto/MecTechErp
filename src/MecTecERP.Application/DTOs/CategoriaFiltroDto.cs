using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.DTOs;

public class CategoriaFiltroDto : FiltroBaseDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public bool? Ativo { get; set; }
    public DateTime? DataCriacaoInicio { get; set; }
    public DateTime? DataCriacaoFim { get; set; }
}