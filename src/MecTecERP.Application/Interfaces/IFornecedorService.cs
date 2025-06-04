using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface IFornecedorService
{
    Task<RespostaDto<PaginacaoDto<FornecedorListDto>>> ObterTodosAsync(FiltroBaseDto filtro);
    Task<RespostaDto<FornecedorDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<FornecedorDto>> CriarAsync(FornecedorCreateDto dto);
    Task<RespostaDto<FornecedorDto>> AtualizarAsync(int id, FornecedorUpdateDto dto);
    Task<RespostaDto> ExcluirAsync(int id);
    Task<RespostaDto> AtivarDesativarAsync(int id);
    Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync();
    Task<RespostaDto<bool>> ExisteAsync(int id);
    Task<RespostaDto<bool>> CnpjExisteAsync(string cnpj, int? idExcluir = null);
    Task<RespostaDto<bool>> EmailExisteAsync(string email, int? idExcluir = null);
    Task<RespostaDto<FornecedorDto>> ObterPorCnpjAsync(string cnpj);
    Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel");
}