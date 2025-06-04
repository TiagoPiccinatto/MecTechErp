using MecTecERP.Domain.Common;

namespace MecTecERP.Domain.Entities
{
    public class Veiculo : BaseEntity
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Cor { get; set; } = string.Empty;
        public string? Chassi { get; set; }
        public string? Renavam { get; set; }
        public string? Combustivel { get; set; }
        public int? Quilometragem { get; set; }
        public DateTime DataCadastro { get; set; }
        public string? Observacoes { get; set; }

        // Relacionamentos
        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
    }
}