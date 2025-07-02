using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Entities;

public class Fornecedor : BaseEntity
{
    [Required(ErrorMessage = "Razão Social é obrigatória")]
    [StringLength(200, ErrorMessage = "Razão Social deve ter no máximo 200 caracteres")]
    public string RazaoSocial { get; set; } = string.Empty; // Alterado de Nome

    [StringLength(200, ErrorMessage = "Nome Fantasia deve ter no máximo 200 caracteres")]
    public string? NomeFantasia { get; set; } // Adicionado

    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter 14 ou 18 caracteres (com máscara)")] // CNPJ tem 14 dígitos
    public string? Cnpj { get; set; }
    
    [StringLength(20, ErrorMessage = "Inscrição Estadual deve ter no máximo 20 caracteres")]
    public string? InscricaoEstadual { get; set; } // Adicionado

    [StringLength(20, ErrorMessage = "Telefone 1 deve ter no máximo 20 caracteres")]
    public string? Telefone1 { get; set; } // Alterado de Telefone

    [StringLength(20, ErrorMessage = "Telefone 2 deve ter no máximo 20 caracteres")]
    public string? Telefone2 { get; set; } // Adicionado

    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    
    [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")] // Ex: 99999-999
    public string? Cep { get; set; }

    [StringLength(200, ErrorMessage = "Logradouro deve ter no máximo 200 caracteres")]
    public string? Logradouro { get; set; } // Adicionado (de Endereco)

    [StringLength(20, ErrorMessage = "Número deve ter no máximo 20 caracteres")]
    public string? Numero { get; set; } // Adicionado (de Endereco)

    [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
    public string? Complemento { get; set; } // Adicionado (de Endereco)

    [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
    public string? Bairro { get; set; } // Adicionado (de Endereco)
    
    [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }
    
    [StringLength(2, MinimumLength=2, ErrorMessage = "UF deve ter 2 caracteres")]
    public string? Uf { get; set; }

    [StringLength(100, ErrorMessage = "Nome do Contato deve ter no máximo 100 caracteres")]
    public string? NomeContato { get; set; } // Adicionado

    [StringLength(20, ErrorMessage = "Telefone do Contato deve ter no máximo 20 caracteres")]
    public string? TelefoneContato { get; set; } // Adicionado

    [StringLength(100, ErrorMessage = "Email do Contato deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email do Contato inválido")]
    public string? EmailContato { get; set; } // Adicionado

    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    
    // Métodos de negócio
    public bool PodeSerExcluido()
    {
        return !Produtos.Any(p => p.Ativo);
    }
    
    public int TotalProdutosAtivos()
    {
        return Produtos.Count(p => p.Ativo);
    }
    
    public string EnderecoCompleto()
    {
        var partes = new List<string>();
        
        if (!string.IsNullOrEmpty(Endereco))
            partes.Add(Endereco);
            
        if (!string.IsNullOrEmpty(Cidade))
            partes.Add(Cidade);
            
        if (!string.IsNullOrEmpty(Uf))
            partes.Add(Uf);
            
        if (!string.IsNullOrEmpty(Cep))
            partes.Add($"CEP: {Cep}");
            
        return string.Join(", ", partes);
    }
}