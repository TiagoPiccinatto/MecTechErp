using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Interfaces;

public interface IInventarioService
{
    Task<RespostaDto<PaginacaoDto<InventarioListDto>>> ObterTodosAsync(InventarioFiltroDto filtro);
    Task<RespostaDto<InventarioDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<InventarioDto>> CriarAsync(InventarioCreateDto dto);
    Task<RespostaDto<InventarioDto>> AtualizarAsync(int id, InventarioUpdateDto dto);
    Task<RespostaDto> ExcluirAsync(int id);
    Task<RespostaDto<InventarioDto>> IniciarInventarioAsync(int id);
    Task<RespostaDto<InventarioDto>> FinalizarInventarioAsync(int id);
    Task<RespostaDto<InventarioDto>> CancelarInventarioAsync(int id);
    Task<RespostaDto<InventarioItemDto>> AtualizarItemAsync(int inventarioId, InventarioItemUpdateDto dto);
    Task<RespostaDto<List<InventarioItemDto>>> AtualizarItensAsync(int inventarioId, List<InventarioItemUpdateDto> itens);
    Task<RespostaDto<List<InventarioItemDto>>> ObterItensAsync(int inventarioId);
    Task<RespostaDto<List<InventarioItemDto>>> ObterItensComDivergenciaAsync(int inventarioId);
    Task<RespostaDto<List<InventarioItemDto>>> ObterItensPendentesAsync(int inventarioId);
    Task<RespostaDto<InventarioResumoDto>> ObterResumoAsync();
    Task<RespostaDto<InventarioDto>> ObterInventarioAbertoAsync();
    Task<RespostaDto<bool>> ExisteInventarioAbertoAsync();
    Task<RespostaDto<bool>> PodeIniciarNovoInventarioAsync();
    Task<RespostaDto> ProcessarDivergenciasAsync(int inventarioId, bool aplicarAjustes = false);
    Task<RespostaDto<decimal>> CalcularValorDivergenciasAsync(int inventarioId);
    Task<RespostaDto<List<MovimentacaoEstoqueListDto>>> ObterMovimentacoesInventarioAsync(int inventarioId);
    Task<RespostaDto<ExportacaoDto>> ExportarInventarioAsync(int inventarioId, string formato = "excel");
    Task<RespostaDto<ExportacaoDto>> ExportarDivergenciasAsync(int inventarioId, string formato = "excel");
}