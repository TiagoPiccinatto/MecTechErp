using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class ClienteDto : BaseDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; set; }
    public string? RgInscricaoEstadual { get; set; }
    public string Endereco { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public string? Observacoes { get; set; }
    public int TotalVeiculos { get; set; }
    public int TotalOrdensServico { get; set; }
}

public class ClienteCreateDto : BaseCreateDto
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    [StringLength(18, ErrorMessage = "O CPF/CNPJ deve ter no máximo 18 caracteres")]
    public string CpfCnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de pessoa é obrigatório")]
    public TipoPessoa TipoPessoa { get; set; }

    [StringLength(20, ErrorMessage = "O RG/Inscrição Estadual deve ter no máximo 20 caracteres")]
    public string? RgInscricaoEstadual { get; set; }

    [Required(ErrorMessage = "O endereço é obrigatório")]
    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CEP é obrigatório")]
    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}\-\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX")]
    public string Cep { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}

public class ClienteUpdateDto : BaseUpdateDto
{
    [Required(ErrorMessage = "O nome do cliente é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório")]
    [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    [StringLength(18, ErrorMessage = "O CPF/CNPJ deve ter no máximo 18 caracteres")]
    public string CpfCnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de pessoa é obrigatório")]
    public TipoPessoa TipoPessoa { get; set; }

    [StringLength(20, ErrorMessage = "O RG/Inscrição Estadual deve ter no máximo 20 caracteres")]
    public string? RgInscricaoEstadual { get; set; }

    [Required(ErrorMessage = "O endereço é obrigatório")]
    [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
    public string Endereco { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória")]
    [StringLength(100, ErrorMessage = "A cidade deve ter no máximo 100 caracteres")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "O estado é obrigatório")]
    [StringLength(2, ErrorMessage = "O estado deve ter no máximo 2 caracteres")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CEP é obrigatório")]
    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}\-\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX")]
    public string Cep { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
}

public class ClienteListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public int TotalVeiculos { get; set; }
    public int TotalOrdensServico { get; set; }
}

public class ClienteFiltroDto : FiltroBaseDto
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? CpfCnpj { get; set; }
    public TipoPessoa? TipoPessoa { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public DateTime? DataCadastroInicio { get; set; }
    public DateTime? DataCadastroFim { get; set; }
}