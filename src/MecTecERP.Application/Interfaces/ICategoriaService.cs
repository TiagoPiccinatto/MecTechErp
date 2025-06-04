using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Interfaces;

public interface ICategoriaService
{
    Task<RespostaDto<PaginacaoDto<CategoriaListDto>>> ObterTodosAsync(FiltroBaseDto filtro);
    Task<RespostaDto<CategoriaDto>> ObterPorIdAsync(int id);
    Task<RespostaDto<CategoriaDto>> CriarAsync(CategoriaCreateDto dto);
    Task<RespostaDto<CategoriaDto>> AtualizarAsync(int id, CategoriaUpdateDto dto);
    Task<RespostaDto> ExcluirAsync(int id);
    Task<RespostaDto> AtivarDesativarAsync(int id);
    Task<RespostaDto<List<SelectItemDto>>> ObterParaSelectAsync();
    Task<RespostaDto<bool>> ExisteAsync(int id);
    Task<RespostaDto<bool>> NomeExisteAsync(string nome, int? idExcluir = null);
    Task<RespostaDto<ExportacaoDto>> ExportarAsync(FiltroBaseDto filtro, string formato = "excel");
}