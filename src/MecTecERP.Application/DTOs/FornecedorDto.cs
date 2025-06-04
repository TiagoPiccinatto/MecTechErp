using System.ComponentModel.DataAnnotations;

namespace MecTecERP.Application.DTOs;

public class FornecedorDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public string? Contato { get; set; }
    public string? Observacoes { get; set; }
    public int TotalProdutos { get; set; }
}

public class FornecedorCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
    [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$", ErrorMessage = "CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX")]
    public string? Cnpj { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
    public string? Endereco { get; set; }

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }

    [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres")]
    public string? Estado { get; set; }

    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}\-\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX")]
    public string? Cep { get; set; }

    [StringLength(100, ErrorMessage = "O contato deve ter no máximo 100 caracteres")]
    public string? Contato { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}

public class FornecedorUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(18, ErrorMessage = "O CNPJ deve ter no máximo 18 caracteres")]
    [RegularExpression(@"^\d{2}\.\d{3}\.\d{3}\/\d{4}\-\d{2}$", ErrorMessage = "CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX")]
    public string? Cnpj { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }

    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
    public string? Endereco { get; set; }

    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }

    [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres")]
    public string? Estado { get; set; }

    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}\-\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX")]
    public string? Cep { get; set; }

    [StringLength(100, ErrorMessage = "O contato deve ter no máximo 100 caracteres")]
    public string? Contato { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}

public class FornecedorListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public int TotalProdutos { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}