using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Entities;

public class Fornecedor : BaseEntity
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(18, ErrorMessage = "CNPJ deve ter no máximo 18 caracteres")]
    public string? Cnpj { get; set; }
    
    [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }
    
    [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
    public string? Endereco { get; set; }
    
    [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
    public string? Cidade { get; set; }
    
    [StringLength(2, ErrorMessage = "UF deve ter no máximo 2 caracteres")]
    public string? Uf { get; set; }
    
    [StringLength(10, ErrorMessage = "CEP deve ter no máximo 10 caracteres")]
    public string? Cep { get; set; }
    
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