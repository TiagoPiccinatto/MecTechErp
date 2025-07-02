using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

// DTO para visualização detalhada de um cliente
public class ClienteDto : BaseDto // BaseDto provavelmente tem Id, DataCriacao, DataAtualizacao, Ativo
{
    public TipoPessoa TipoPessoa { get; set; }
    [Required(ErrorMessage = "O Nome/Razão Social é obrigatório")]
    [StringLength(200, ErrorMessage = "O Nome/Razão Social deve ter no máximo 200 caracteres")]
    public string NomeRazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 11, ErrorMessage = "O CPF/CNPJ deve ter entre 11 e 18 caracteres")]
    public string CpfCnpj { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O RG/IE deve ter no máximo 20 caracteres")]
    public string? RgIe { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 1 deve ter no máximo 20 caracteres")]
    public string? Telefone1 { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 2 deve ter no máximo 20 caracteres")]
    public string? Telefone2 { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX ou XXXXXXXX")]
    public string? Cep { get; set; }

    [StringLength(200, ErrorMessage = "O Logradouro deve ter no máximo 200 caracteres")]
    public string? Logradouro { get; set; }

    [StringLength(20, ErrorMessage = "O Número deve ter no máximo 20 caracteres")]
    public string? Numero { get; set; }

    [StringLength(100, ErrorMessage = "O Complemento deve ter no máximo 100 caracteres")]
    public string? Complemento { get; set; }

    [StringLength(100, ErrorMessage = "O Bairro deve ter no máximo 100 caracteres")]
    public string? Bairro { get; set; }

    [StringLength(100, ErrorMessage = "A Cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres")]
    public string? Uf { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    // Propriedades de BaseDto já incluem Ativo, DataCriacao (DataCadastro), DataAtualizacao
    // public bool Ativo { get; set; } // Vem de BaseDto
    // public DateTime DataCadastro { get; set; } // Vem de BaseDto como DataCriacao

    public int TotalVeiculos { get; set; } // Mantido do DTO existente
    public int TotalOrdensServico { get; set; } // Mantido do DTO existente

    public ICollection<VeiculoDto>? Veiculos { get; set; } // Para visualização detalhada
    public ICollection<OrdemServicoDto>? OrdensServico { get; set; } // Para visualização detalhada
}

// DTO para criação de Cliente
public class ClienteCreateDto
{
    [Required(ErrorMessage = "O tipo de pessoa é obrigatório")]
    public TipoPessoa TipoPessoa { get; set; }

    [Required(ErrorMessage = "O Nome/Razão Social é obrigatório")]
    [StringLength(200, ErrorMessage = "O Nome/Razão Social deve ter no máximo 200 caracteres")]
    public string NomeRazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 11, ErrorMessage = "O CPF/CNPJ deve ter entre 11 e 18 caracteres")]
    // Adicionar validação customizada para CPF/CNPJ aqui ou no serviço
    public string CpfCnpj { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O RG/IE deve ter no máximo 20 caracteres")]
    public string? RgIe { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 1 deve ter no máximo 20 caracteres")]
    public string? Telefone1 { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 2 deve ter no máximo 20 caracteres")]
    public string? Telefone2 { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX ou XXXXXXXX")]
    public string? Cep { get; set; }

    [StringLength(200, ErrorMessage = "O Logradouro deve ter no máximo 200 caracteres")]
    public string? Logradouro { get; set; }

    [StringLength(20, ErrorMessage = "O Número deve ter no máximo 20 caracteres")]
    public string? Numero { get; set; }

    [StringLength(100, ErrorMessage = "O Complemento deve ter no máximo 100 caracteres")]
    public string? Complemento { get; set; }

    [StringLength(100, ErrorMessage = "O Bairro deve ter no máximo 100 caracteres")]
    public string? Bairro { get; set; }

    [StringLength(100, ErrorMessage = "A Cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres")]
    public string? Uf { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    public bool Ativo { get; set; } = true; // Permitir definir se já começa ativo ou não
}

// DTO para atualização de Cliente
public class ClienteUpdateDto
{
    [Required(ErrorMessage = "O ID do cliente é obrigatório para atualização")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O tipo de pessoa é obrigatório")]
    public TipoPessoa TipoPessoa { get; set; }

    [Required(ErrorMessage = "O Nome/Razão Social é obrigatório")]
    [StringLength(200, ErrorMessage = "O Nome/Razão Social deve ter no máximo 200 caracteres")]
    public string NomeRazaoSocial { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF/CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 11, ErrorMessage = "O CPF/CNPJ deve ter entre 11 e 18 caracteres")]
    public string CpfCnpj { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "O RG/IE deve ter no máximo 20 caracteres")]
    public string? RgIe { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 1 deve ter no máximo 20 caracteres")]
    public string? Telefone1 { get; set; }

    [StringLength(20, ErrorMessage = "O Telefone 2 deve ter no máximo 20 caracteres")]
    public string? Telefone2 { get; set; }

    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }

    [StringLength(10, ErrorMessage = "O CEP deve ter no máximo 10 caracteres")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP deve estar no formato XXXXX-XXX ou XXXXXXXX")]
    public string? Cep { get; set; }

    [StringLength(200, ErrorMessage = "O Logradouro deve ter no máximo 200 caracteres")]
    public string? Logradouro { get; set; }

    [StringLength(20, ErrorMessage = "O Número deve ter no máximo 20 caracteres")]
    public string? Numero { get; set; }

    [StringLength(100, ErrorMessage = "O Complemento deve ter no máximo 100 caracteres")]
    public string? Complemento { get; set; }

    [StringLength(100, ErrorMessage = "O Bairro deve ter no máximo 100 caracteres")]
    public string? Bairro { get; set; }

    [StringLength(100, ErrorMessage = "A Cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter 2 caracteres")]
    public string? Uf { get; set; }

    [StringLength(1000, ErrorMessage = "As observações devem ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }

    public bool Ativo { get; set; }
}

// DTO para listagem de Clientes (simplificado)
public class ClienteListDto
{
    public int Id { get; set; }
    public string NomeRazaoSocial { get; set; } = string.Empty; // Alterado de Nome
    public string? Email { get; set; } // Email pode ser opcional na listagem
    public string? Telefone1 { get; set; } // Alterado de Telefone
    public string CpfCnpj { get; set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; } // Alterado de Estado
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; } // Mapeado de DataCriacao
    public int TotalVeiculos { get; set; }
    public int TotalOrdensServico { get; set; }
}

// DTO para filtros de Cliente
public class ClienteFiltroDto : FiltroBaseDto // FiltroBaseDto deve ter Pagina, TamanhoPagina, Ordenacao
{
    public string? NomeRazaoSocial { get; set; } // Alterado de Nome
    public string? Email { get; set; }
    public string? CpfCnpj { get; set; }
    public TipoPessoa? TipoPessoa { get; set; }
    public string? Cidade { get; set; }
    public string? Uf { get; set; } // Alterado de Estado
    public DateTime? DataCadastroInicio { get; set; }
    public DateTime? DataCadastroFim { get; set; }
    public bool? Ativo { get; set; } // Adicionado filtro por ativo
}