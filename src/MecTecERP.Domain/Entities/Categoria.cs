using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Entities;

public class Categoria : BaseEntity
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
    
    // Relacionamentos
    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    
    // Métodos de negócio
    public bool PodeSerExcluida()
    {
        return !Produtos.Any(p => p.Ativo);
    }
    
    public int TotalProdutosAtivos()
    {
        return Produtos.Count(p => p.Ativo);
    }
}