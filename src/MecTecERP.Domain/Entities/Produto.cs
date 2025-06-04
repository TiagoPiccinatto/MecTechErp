using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities;

public class Produto : BaseEntity
{
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? Descricao { get; set; }
    
    [StringLength(20, ErrorMessage = "Unidade deve ter no máximo 20 caracteres")]
    public string? Unidade { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço de custo deve ser maior ou igual a zero")]
    public decimal PrecoCusto { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior ou igual a zero")]
    public decimal PrecoVenda { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, double.MaxValue, ErrorMessage = "Estoque atual deve ser maior ou igual a zero")]
    public decimal EstoqueAtual { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, double.MaxValue, ErrorMessage = "Estoque mínimo deve ser maior ou igual a zero")]
    public decimal EstoqueMinimo { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, double.MaxValue, ErrorMessage = "Estoque máximo deve ser maior ou igual a zero")]
    public decimal EstoqueMaximo { get; set; }
    
    [StringLength(500, ErrorMessage = "Localização deve ter no máximo 500 caracteres")]
    public string? Localizacao { get; set; }
    
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    
    [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
    public string? CodigoBarras { get; set; }
    
    // Chaves estrangeiras
    public int CategoriaId { get; set; }
    public int? FornecedorId { get; set; }
    
    // Relacionamentos
    public virtual Categoria Categoria { get; set; } = null!;
    public virtual Fornecedor? Fornecedor { get; set; }
    public virtual ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
    public virtual ICollection<InventarioItem> ItensInventario { get; set; } = new List<InventarioItem>();
    
    // Métodos de negócio
    public bool EstoqueAbaixoDoMinimo()
    {
        return EstoqueAtual < EstoqueMinimo;
    }
    
    public bool EstoqueCritico()
    {
        return EstoqueAtual <= (EstoqueMinimo * 0.5m);
    }
    
    public bool EstoqueZerado()
    {
        return EstoqueAtual <= 0;
    }
    
    public decimal CalcularMargemLucro()
    {
        if (PrecoCusto <= 0) return 0;
        return ((PrecoVenda - PrecoCusto) / PrecoCusto) * 100;
    }
    
    public decimal ValorEstoqueAtual()
    {
        return EstoqueAtual * PrecoCusto;
    }
    
    public void AtualizarEstoque(decimal quantidade)
    {
        EstoqueAtual = Math.Max(0, EstoqueAtual + quantidade);
        MarcarComoAtualizado();
    }
    
    public bool PodeRetirarEstoque(decimal quantidade)
    {
        return EstoqueAtual >= quantidade;
    }
    
    public string ObterStatusEstoque()
    {
        if (EstoqueZerado())
            return "Sem Estoque";
        if (EstoqueCritico())
            return "Crítico";
        if (EstoqueAbaixoDoMinimo())
            return "Baixo";
        return "Normal";
    }
    
    public StatusEstoque StatusEstoqueEnum
    {
        get
        {
            if (EstoqueZerado())
                return StatusEstoque.SemEstoque;
            if (EstoqueCritico() || EstoqueAbaixoDoMinimo())
                return StatusEstoque.EstoqueBaixo;
            return StatusEstoque.Disponivel;
        }
    }
}