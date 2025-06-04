using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface IVeiculoService
{
    Task<RespostaDto<PaginacaoDto<VeiculoListDto>>> ObterTodosAsync(VeiculoFiltroDto filtro);
    Task<RespostaDto<VeiculoDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<VeiculoDto>> CriarAsync(VeiculoCreateDto dto);
    Task<RespostaDto<VeiculoDto>> AtualizarAsync(int id, VeiculoUpdateDto dto);
    Task<RespostaDto<bool>> ExcluirAsync(int id);
    Task<RespostaDto<bool>> ExistePlacaAsync(string placa, int? idExcluir = null);
    Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync();
    Task<RespostaDto<List<VeiculoListDto>>> ObterPorClienteAsync(int clienteId);
    Task<RespostaDto<List<VeiculoListDto>>> ObterPorPlacaAsync(string placa);
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterOrdensServicoAsync(int veiculoId);
}