using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;
using System;

namespace MecTecERP.Domain.Entities
{
    public class OrdemServicoItem : BaseEntity // Alinhado com BaseEntity.Id (int)
    {
        public int OrdemServicoId { get; set; } // Alterado para int

        public TipoItemOrdemServico Tipo { get; set; }
        
        public int? ProdutoId { get; set; }

        public string Descricao { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal? DescontoItem { get; set; }

        public string? ObservacoesItem { get; set; }
        // DataCriacao, Ativo, etc., vêm de BaseEntity

        // Navigation Properties
        public virtual OrdemServico OrdemServico { get; set; } = null!;
        public virtual Produto? Produto { get; set; }
    }
}