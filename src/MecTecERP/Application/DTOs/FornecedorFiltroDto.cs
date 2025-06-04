using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.DTOs;

public class FornecedorFiltroDto : FiltroBaseDto
{
    public string? Nome { get; set; }
    public string? RazaoSocial { get; set; }
    public string? CnpjCpf { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public bool? Ativo { get; set; }
    public DateTime? DataCriacaoInicio { get; set; }
    public DateTime? DataCriacaoFim { get; set; }
}