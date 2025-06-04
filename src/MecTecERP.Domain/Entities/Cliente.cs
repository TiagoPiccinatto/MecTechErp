using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public TipoPessoa TipoPessoa { get; set; }
        public string? RgInscricaoEstadual { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public string? Observacoes { get; set; }

        // Relacionamentos
        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public virtual ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
    }
}