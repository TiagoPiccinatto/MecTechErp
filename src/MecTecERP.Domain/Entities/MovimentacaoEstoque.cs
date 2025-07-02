using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities;

public class MovimentacaoEstoque : BaseEntity
{
    [Required(ErrorMessage = "Tipo de movimentação é obrigatório")]
    public TipoMovimentacaoEstoque Tipo { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor unitário deve ser maior ou igual a zero")]
    public decimal ValorUnitario { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal ValorTotal => Quantidade * ValorUnitario;
    
    [Required(ErrorMessage = "Data da movimentação é obrigatória")]
    public DateTime DataMovimentacao { get; set; } = DateTime.Now;
    
    [StringLength(100, ErrorMessage = "Documento deve ter no máximo 100 caracteres")]
    public string? Documento { get; set; }
    
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal EstoqueAnterior { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal EstoquePosterior { get; set; }
    
    // Chaves estrangeiras
    public int ProdutoId { get; set; }
    public int? InventarioId { get; set; }
    public int? OrdemServicoItemId { get; set; } // Adicionado
    
    // Relacionamentos
    public virtual Produto Produto { get; set; } = null!;
    public virtual Inventario? Inventario { get; set; }
    public virtual OrdemServicoItem? OrdemServicoItem { get; set; } // Adicionado
    
    // Métodos de negócio
    public bool IsEntrada()
    {
        return Tipo == TipoMovimentacaoEstoque.Entrada || 
               (Tipo == TipoMovimentacaoEstoque.Ajuste && Quantidade > 0) ||
               (Tipo == TipoMovimentacaoEstoque.Inventario && EstoquePosterior > EstoqueAnterior);
    }
    
    public bool IsSaida()
    {
        return Tipo == TipoMovimentacaoEstoque.Saida || 
               (Tipo == TipoMovimentacaoEstoque.Ajuste && Quantidade < 0) ||
               (Tipo == TipoMovimentacaoEstoque.Inventario && EstoquePosterior < EstoqueAnterior);
    }
    
    public decimal QuantidadeMovimentada()
    {
        if (Tipo == TipoMovimentacaoEstoque.Inventario)
        {
            return Math.Abs(EstoquePosterior - EstoqueAnterior);
        }
        return Math.Abs(Quantidade);
    }
    
    public string TipoMovimentacaoDescricao()
    {
        return Tipo switch
        {
            TipoMovimentacaoEstoque.Entrada => "Entrada",
            TipoMovimentacaoEstoque.Saida => "Saída",
            TipoMovimentacaoEstoque.Ajuste => "Ajuste",
            TipoMovimentacaoEstoque.Transferencia => "Transferência",
            TipoMovimentacaoEstoque.Inventario => "Inventário",
            _ => "Desconhecido"
        };
    }
    
    public void CalcularEstoquePosterior()
    {
        switch (Tipo)
        {
            case TipoMovimentacaoEstoque.Entrada:
                EstoquePosterior = EstoqueAnterior + Quantidade;
                break;
            case TipoMovimentacaoEstoque.Saida:
                EstoquePosterior = EstoqueAnterior - Quantidade;
                break;
            case TipoMovimentacaoEstoque.Ajuste:
                EstoquePosterior = EstoqueAnterior + Quantidade;
                break;
            case TipoMovimentacaoEstoque.Transferencia:
                EstoquePosterior = EstoqueAnterior - Quantidade;
                break;
            default:
                EstoquePosterior = EstoqueAnterior;
                break;
        }
        
        EstoquePosterior = Math.Max(0, EstoquePosterior);
    }
}