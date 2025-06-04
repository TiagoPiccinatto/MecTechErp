using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface IRelatorioService
{
    Task<RespostaDto<RelatorioEstoqueDto>> GerarRelatorioEstoqueAsync(FiltroRelatorioEstoqueDto filtros);
    Task<RespostaDto<RelatorioMovimentacoesDto>> GerarRelatorioMovimentacoesAsync(FiltroRelatorioMovimentacoesDto filtros);
    Task<RespostaDto<RelatorioInventarioDto>> GerarRelatorioInventarioAsync(int inventarioId);
    Task<RespostaDto<RelatorioValorizacaoEstoqueDto>> GerarRelatorioValorizacaoEstoqueAsync(FiltroRelatorioValorizacaoDto filtros);
    Task<RespostaDto<ExportacaoDto>> ExportarRelatorioEstoqueAsync(FiltroRelatorioEstoqueDto filtros, string formato = "excel");
    Task<RespostaDto<ExportacaoDto>> ExportarRelatorioMovimentacoesAsync(FiltroRelatorioMovimentacoesDto filtros, string formato = "excel");
    Task<RespostaDto<ExportacaoDto>> ExportarRelatorioInventarioAsync(int inventarioId, string formato = "excel");
    Task<RespostaDto<ExportacaoDto>> ExportarRelatorioValorizacaoEstoqueAsync(FiltroRelatorioValorizacaoDto filtros, string formato = "excel");
    Task<RespostaDto<ExportacaoDto>> GerarRelatorioPersonalizadoAsync(string tipoRelatorio, Dictionary<string, object> parametros, string formato = "excel");
    Task<RespostaDto<List<string>>> ObterTiposRelatorioDisponiveis();
    Task<RespostaDto<List<string>>> ObterFormatosExportacaoDisponiveis();
}