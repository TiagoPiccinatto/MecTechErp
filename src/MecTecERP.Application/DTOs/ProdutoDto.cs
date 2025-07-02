using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class ProdutoDto : BaseDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public int? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public UnidadeMedida UnidadeMedida { get; set; }
    public string UnidadeMedidaTexto { get; set; } = string.Empty;
    public decimal PrecoCusto { get; set; } // Alterado de PrecoCompra
    public decimal PrecoVenda { get; set; }
    public decimal EstoqueAtual { get; set; }
    public string? FotoUrl { get; set; } // Adicionado
    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; }
    public string? Localizacao { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Observacoes { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public bool EstoqueBaixo { get; set; }
    public bool EstoqueZerado { get; set; }
}

public class ProdutoCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O código do produto é obrigatório")]
    [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida")]
    public int CategoriaId { get; set; }

    public int? FornecedorId { get; set; }

    [Required(ErrorMessage = "A unidade de medida é obrigatória")]
    public UnidadeMedida UnidadeMedida { get; set; }

    [Required(ErrorMessage = "O preço de custo é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço de custo deve ser maior que zero")]
    public decimal PrecoCusto { get; set; } // Alterado de PrecoCompra

    [Required(ErrorMessage = "O preço de venda é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço de venda deve ser maior que zero")]
    public decimal PrecoVenda { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
    public decimal EstoqueAtual { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
    public decimal EstoqueMinimo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque máximo deve ser maior ou igual a zero")]
    public decimal EstoqueMaximo { get; set; }

    [StringLength(100, ErrorMessage = "A localização deve ter no máximo 100 caracteres")]
    public string? Localizacao { get; set; }

    [StringLength(50, ErrorMessage = "O código de barras deve ter no máximo 50 caracteres")]
    public string? CodigoBarras { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    [StringLength(255, ErrorMessage = "A URL da foto deve ter no máximo 255 caracteres")]
    public string? FotoUrl { get; set; } // Adicionado
}

public class ProdutoUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O código do produto é obrigatório")]
    [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida")]
    public int CategoriaId { get; set; }

    public int? FornecedorId { get; set; }

    [Required(ErrorMessage = "A unidade de medida é obrigatória")]
    public UnidadeMedida UnidadeMedida { get; set; }

    [Required(ErrorMessage = "O preço de compra é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço de compra deve ser maior que zero")]
    public decimal PrecoCompra { get; set; }

    [Required(ErrorMessage = "O preço de venda é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço de venda deve ser maior que zero")]
    public decimal PrecoVenda { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
    public decimal EstoqueAtual { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
    public decimal EstoqueMinimo { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O estoque máximo deve ser maior ou igual a zero")]
    public decimal EstoqueMaximo { get; set; }

    [StringLength(100, ErrorMessage = "A localização deve ter no máximo 100 caracteres")]
    public string? Localizacao { get; set; }

    [StringLength(50, ErrorMessage = "O código de barras deve ter no máximo 50 caracteres")]
    public string? CodigoBarras { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    [StringLength(255, ErrorMessage = "A URL da foto deve ter no máximo 255 caracteres")]
    public string? FotoUrl { get; set; } // Adicionado
}

public class ProdutoListDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public string? FornecedorNome { get; set; }
    public string UnidadeMedidaTexto { get; set; } = string.Empty;
    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public bool EstoqueBaixo { get; set; }
    public bool EstoqueZerado { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

public class ProdutoSelectDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Display => $"{Codigo} - {Nome}";
}