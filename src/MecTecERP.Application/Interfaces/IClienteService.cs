using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface IClienteService
{
    Task<RespostaDto<PaginacaoDto<ClienteListDto>>> ObterTodosAsync(ClienteFiltroDto filtro);
    Task<RespostaDto<ClienteDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<ClienteDto>> CriarAsync(ClienteCreateDto dto);
    Task<RespostaDto<ClienteDto>> AtualizarAsync(int id, ClienteUpdateDto dto);
    Task<RespostaDto<bool>> ExcluirAsync(int id);
    Task<RespostaDto<bool>> ExisteCpfCnpjAsync(string cpfCnpj, int? idExcluir = null);
    Task<RespostaDto<bool>> ExisteEmailAsync(string email, int? idExcluir = null);
    Task<RespostaDto<List<SelectItemDto>>> ObterSelectListAsync();
    Task<RespostaDto<List<ClienteListDto>>> ObterPorNomeAsync(string nome);
    Task<RespostaDto<List<VeiculoListDto>>> ObterVeiculosAsync(int clienteId);
    Task<RespostaDto<List<OrdemServicoListDto>>> ObterOrdensServicoAsync(int clienteId);
}