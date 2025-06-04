using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs;

public class CategoriaDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TotalProdutos { get; set; }
}

public class CategoriaCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
}

public class CategoriaUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
}

public class CategoriaListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int TotalProdutos { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}