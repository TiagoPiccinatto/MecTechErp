using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities
{
    public class OrdemServico : BaseEntity
    {
        public string Numero { get; set; } = string.Empty;
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public StatusOrdemServico Status { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? DefeitoRelatado { get; set; }
        public string? SolucaoAplicada { get; set; }
        public decimal ValorMaoObra { get; set; }
        public decimal ValorPecas { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal? Desconto { get; set; }
        public string? Observacoes { get; set; }
        public int? Quilometragem { get; set; }
        public DateTime? DataPrevisaoEntrega { get; set; }
        public string? ResponsavelTecnico { get; set; }

        // Relacionamentos
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; } = null!;
        
        public int VeiculoId { get; set; }
        public virtual Veiculo Veiculo { get; set; } = null!;
        
        public virtual ICollection<OrdemServicoItem> Itens { get; set; } = new List<OrdemServicoItem>();
    }
}