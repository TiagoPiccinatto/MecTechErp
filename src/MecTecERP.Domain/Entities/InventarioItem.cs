using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Entities;

public class InventarioItem : BaseEntity
{
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, double.MaxValue, ErrorMessage = "Estoque sistema deve ser maior ou igual a zero")]
    public decimal EstoqueSistema { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    [Range(0, double.MaxValue, ErrorMessage = "Estoque contado deve ser maior ou igual a zero")]
    public decimal EstoqueContado { get; set; }
    
    [Column(TypeName = "decimal(18,3)")]
    public decimal Diferenca => EstoqueContado - EstoqueSistema;
    
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    
    public DateTime? DataContagem { get; set; }
    
    [StringLength(100, ErrorMessage = "Usuário contagem deve ter no máximo 100 caracteres")]
    public string? UsuarioContagem { get; set; }
    
    // Chaves estrangeiras
    public int InventarioId { get; set; }
    public int ProdutoId { get; set; }
    
    // Relacionamentos
    public virtual Inventario Inventario { get; set; } = null!;
    public virtual Produto Produto { get; set; } = null!;
    
    // Métodos de negócio
    public bool TemDiferenca()
    {
        return Math.Abs(Diferenca) > 0.001m; // Considera diferenças maiores que 0.001
    }
    
    public bool DiferencaPositiva()
    {
        return Diferenca > 0;
    }
    
    public bool DiferencaNegativa()
    {
        return Diferenca < 0;
    }
    
    public decimal DiferencaAbsoluta()
    {
        return Math.Abs(Diferenca);
    }
    
    public decimal PercentualDiferenca()
    {
        if (EstoqueSistema == 0)
            return EstoqueContado > 0 ? 100 : 0;
            
        return (Diferenca / EstoqueSistema) * 100;
    }
    
    public decimal ValorDiferenca()
    {
        return Diferenca * Produto.PrecoCusto;
    }
    
    public string TipoDiferenca()
    {
        if (!TemDiferenca())
            return "Sem Diferença";
            
        return DiferencaPositiva() ? "Sobra" : "Falta";
    }
    
    public void RegistrarContagem(decimal estoqueContado, string? usuario = null, string? observacoes = null)
    {
        EstoqueContado = estoqueContado;
        DataContagem = DateTime.Now;
        UsuarioContagem = usuario;
        
        if (!string.IsNullOrEmpty(observacoes))
            Observacoes = observacoes;
            
        MarcarComoAtualizado(usuario);
    }
    
    public bool FoiContado()
    {
        return DataContagem.HasValue;
    }
    
    public string StatusContagem()
    {
        if (!FoiContado())
            return "Pendente";
            
        if (!TemDiferenca())
            return "Conferido";
            
        return "Com Diferença";
    }
}