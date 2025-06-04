using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities;

public class Inventario : BaseEntity
{
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "Descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Data de início é obrigatória")]
    public DateTime DataInicio { get; set; }
    
    public DateTime? DataFinalizacao { get; set; }
    
    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusInventario Status { get; set; } = StatusInventario.Planejado;
    
    [StringLength(1000, ErrorMessage = "Observações deve ter no máximo 1000 caracteres")]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual ICollection<InventarioItem> Itens { get; set; } = new List<InventarioItem>();
    public virtual ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
    
    // Métodos de negócio
    public bool PodeSerEditado()
    {
        return Status == StatusInventario.Planejado || Status == StatusInventario.EmAndamento;
    }
    
    public bool PodeSerIniciado()
    {
        return Status == StatusInventario.Planejado;
    }
    
    public bool PodeSerFinalizado()
    {
        return Status == StatusInventario.EmAndamento && Itens.Any();
    }
    
    public bool PodeSerCancelado()
    {
        return Status == StatusInventario.Planejado || Status == StatusInventario.EmAndamento;
    }
    
    public void Iniciar(string? usuario = null)
    {
        if (!PodeSerIniciado())
            throw new InvalidOperationException("Inventário não pode ser iniciado no status atual");
            
        Status = StatusInventario.EmAndamento;
        MarcarComoAtualizado(usuario);
    }
    
    public void Finalizar(string? usuario = null)
    {
        if (!PodeSerFinalizado())
            throw new InvalidOperationException("Inventário não pode ser finalizado no status atual ou não possui itens");
            
        Status = StatusInventario.Finalizado;
        DataFinalizacao = DateTime.Now;
        MarcarComoAtualizado(usuario);
    }
    
    public void Cancelar(string? usuario = null)
    {
        if (!PodeSerCancelado())
            throw new InvalidOperationException("Inventário não pode ser cancelado no status atual");
            
        Status = StatusInventario.Cancelado;
        MarcarComoAtualizado(usuario);
    }
    
    public int TotalItens()
    {
        return Itens.Count;
    }
    
    public int TotalItensComDiferenca()
    {
        return Itens.Count(i => i.TemDiferenca());
    }
    
    public decimal TotalDiferenca()
    {
        return Itens.Sum(i => i.Diferenca);
    }
    
    public decimal ValorTotalDiferenca()
    {
        return Itens.Sum(i => i.ValorDiferenca());
    }
    
    public string StatusDescricao()
    {
        return Status switch
        {
            StatusInventario.Planejado => "Planejado",
            StatusInventario.EmAndamento => "Em Andamento",
            StatusInventario.Finalizado => "Finalizado",
            StatusInventario.Cancelado => "Cancelado",
            _ => "Desconhecido"
        };
    }
    
    public TimeSpan? TempoExecucao()
    {
        if (Status == StatusInventario.Finalizado && DataFinalizacao.HasValue)
        {
            return DataFinalizacao.Value - DataInicio;
        }
        
        if (Status == StatusInventario.EmAndamento)
        {
            return DateTime.Now - DataInicio;
        }
        
        return null;
    }
}