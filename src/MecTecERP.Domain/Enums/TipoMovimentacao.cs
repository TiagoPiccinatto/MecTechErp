using System.ComponentModel;

namespace MecTecERP.Domain.Enums
{
    public enum TipoMovimentacao
    {
        [Description("Entrada")]
        Entrada = 1,
        
        [Description("Saída")]
        Saida = 2,
        
        [Description("Ajuste Positivo")]
        AjustePositivo = 3,
        
        [Description("Ajuste Negativo")]
        AjusteNegativo = 4,
        
        [Description("Transferência")]
        Transferencia = 5,
        
        [Description("Inventário")]
        Inventario = 6,
        
        [Description("Ajuste")]
        Ajuste = 7
    }
}