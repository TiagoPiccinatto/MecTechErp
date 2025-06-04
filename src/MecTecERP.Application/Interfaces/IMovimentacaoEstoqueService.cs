using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Interfaces;

public interface IMovimentacaoEstoqueService
{
    Task<RespostaDto<PaginacaoDto<MovimentacaoEstoqueListDto>>> ObterTodosAsync(MovimentacaoEstoqueFiltroDto filtro);
    Task<RespostaDto<MovimentacaoEstoqueDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<MovimentacaoEstoqueDto>> CriarAsync(MovimentacaoEstoqueCreateDto dto);
    Task<RespostaDto<MovimentacaoEstoqueDto>> AtualizarAsync(int id, MovimentacaoEstoqueUpdateDto dto);
    Task<RespostaDto> ExcluirAsync(int id);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorProdutoAsync(int produtoId, int limite = 10);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterRecentesAsync(int limite = 10);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorTipoAsync(TipoMovimentacaoEstoque tipo, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorInventarioAsync(int inventarioId);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterPorDocumentoAsync(string documento);
    Task<RespostaDto<MovimentacaoEstoqueResumoDto>> ObterResumoAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<RespostaDto<List<MovimentacaoPorDiaDto>>> ObterMovimentacoesPorDiaAsync(DateTime dataInicio, DateTime dataFim);
    Task<RespostaDto<decimal>> CalcularSaldoEstoqueAsync(int produtoId, DateTime? dataLimite = null);
    Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarEntradaAsync(int produtoId, decimal quantidade, decimal valorUnitario, string? documento = null, string? observacoes = null);
    Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarSaidaAsync(int produtoId, decimal quantidade, decimal valorUnitario, string? documento = null, string? observacoes = null);
    Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarAjusteAsync(int produtoId, decimal quantidade, decimal valorUnitario, string motivo, string? documento = null);
    Task<RespostaDto<MovimentacaoEstoqueDto>> RegistrarInventarioAsync(int produtoId, int inventarioId, decimal estoqueContado, decimal estoqueAnterior, decimal valorUnitario);
    Task<RespostaDto<bool>> ValidarEstoqueSuficienteAsync(int produtoId, decimal quantidade);
    Task<RespostaDto<ExportacaoDto>> ExportarAsync(MovimentacaoEstoqueFiltroDto filtro, string formato = "excel");
}