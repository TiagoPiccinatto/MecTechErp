using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Interfaces;

public interface IOrdemServicoService
{
    Task<RespostaDto<PaginacaoDto<OrdemServicoListDto>>> ObterTodosAsync(OrdemServicoFiltroDto filtro);
    Task<RespostaDto<OrdemServicoDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<OrdemServicoDto>> CriarAsync(OrdemServicoCreateDto dto);
    Task<RespostaDto<OrdemServicoDto>> AtualizarAsync(int id, OrdemServicoUpdateDto dto);
    Task<RespostaDto<bool>> ExcluirAsync(int id);
    Task<RespostaDto<bool>> ExisteNumeroAsync(string numero, int? idExcluir = null);
    Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync();
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorClienteAsync(int clienteId);
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorVeiculoAsync(int veiculoId);
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorStatusAsync(StatusOrdemServico status);
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterPorNumeroAsync(string numero);
    Task<RespostaDto<OrdemServicoDto>> FecharOrdemServicoAsync(int id, string solucaoAplicada, decimal? desconto = null);
    Task<RespostaDto<OrdemServicoDto>> ReabrirOrdemServicoAsync(int id);
    Task<RespostaDto<bool>> AdicionarItemAsync(int ordemServicoId, OrdemServicoItemCreateDto item);
    Task<RespostaDto<bool>> AtualizarItemAsync(int ordemServicoId, int itemId, OrdemServicoItemUpdateDto item);
    Task<RespostaDto<bool>> RemoverItemAsync(int ordemServicoId, int itemId);
    Task<RespostaDto<decimal>> CalcularValorTotalAsync(int id);
}