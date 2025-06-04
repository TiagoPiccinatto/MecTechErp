using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface IDashboardService
{
    Task<RespostaDto<DashboardEstoqueDto>> ObterDashboardEstoqueAsync(FiltrosDashboardDto? filtros = null);
    Task<RespostaDto<ResumoEstoqueDto>> ObterResumoEstoqueAsync();
    Task<RespostaDto<List<MovimentacaoRecenteDto>>> ObterMovimentacoesRecentesAsync(int limite = 10);
    Task<RespostaDto<List<ProdutoEstoqueBaixoDto>>> ObterProdutosEstoqueBaixoAsync(int limite = 10);
    Task<RespostaDto<List<ProdutoEstoqueZeradoDto>>> ObterProdutosEstoqueZeradoAsync(int limite = 10);
    Task<RespostaDto<List<CategoriaResumoDto>>> ObterResumoCategoriaAsync();
    Task<RespostaDto<List<MovimentacaoPorDiaDto>>> ObterMovimentacoesPorDiaAsync(DateTime dataInicio, DateTime dataFim);
    Task<RespostaDto<List<ValorEstoquePorCategoriaDto>>> ObterValorEstoquePorCategoriaAsync();
    Task<RespostaDto<GraficoMovimentacoesDto>> ObterGraficoMovimentacoesAsync(DateTime dataInicio, DateTime dataFim);
    Task<RespostaDto<GraficoCategoriasDto>> ObterGraficoCategoriasAsync();
    Task<RespostaDto<GraficoValorEstoqueDto>> ObterGraficoValorEstoqueAsync();
    Task<RespostaDto<GraficoBaixoEstoqueDto>> ObterGraficoBaixoEstoqueAsync(int limite = 10);
    Task<RespostaDto<Dictionary<string, object>>> ObterEstatisticasGeraisAsync();
    Task<RespostaDto<Dictionary<string, decimal>>> ObterIndicadoresFinanceirosAsync();
    Task<RespostaDto<List<object>>> ObterAlertasAsync();
}