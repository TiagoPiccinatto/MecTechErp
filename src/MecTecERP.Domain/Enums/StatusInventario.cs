using System.ComponentModel;

namespace MecTecERP.Domain.Enums;

public enum StatusInventario
{
    [Description("Planejado")]
    Planejado = 1,
    
    [Description("Em Andamento")]
    EmAndamento = 2,
    
    [Description("Finalizado")]
    Finalizado = 3,
    
    [Description("Cancelado")]
    Cancelado = 4
}