using System.ComponentModel;

namespace MecTecERP.Domain.Enums;

public enum TipoMovimentacaoEstoque
{
    [Description("Entrada")]
    Entrada = 1,
    
    [Description("Saída")]
    Saida = 2,
    
    [Description("Ajuste")]
    Ajuste = 3,
    
    [Description("Transferência")]
    Transferencia = 4,
    
    [Description("Inventário")]
    Inventario = 5
}