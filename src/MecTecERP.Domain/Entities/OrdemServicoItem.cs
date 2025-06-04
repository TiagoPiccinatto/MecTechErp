using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities
{
    public class OrdemServicoItem : BaseEntity
    {
        public TipoItemOrdemServico Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal? Desconto { get; set; }
        public string? Observacoes { get; set; }

        // Relacionamentos
        public int OrdemServicoId { get; set; }
        public virtual OrdemServico OrdemServico { get; set; } = null!;
        
        public int? ProdutoId { get; set; }
        public virtual Produto? Produto { get; set; }
    }
}