using MecTecERP.Domain.Common;
using System;
using System.Collections.Generic;

namespace MecTecERP.Domain.Entities
{
    public class Veiculo : BaseEntity // Alinhado com BaseEntity.Id (int)
    {
        public int ClienteId { get; set; } // Alterado para int
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int AnoFabricacao { get; set; }
        public int? AnoModelo { get; set; }
        public string Cor { get; set; } = string.Empty;
        public string? Chassi { get; set; }
        public int? KmAtual { get; set; }
        public string? TipoCombustivel { get; set; }
        public string? Renavam { get; set; }
        public string? Foto { get; set; }
        public string? Observacoes { get; set; }
        // Ativo, DataCadastro, DataUltimaAtualizacao, UsuarioCadastro, UsuarioUltimaAtualizacao vêm de BaseEntity

        // Navigation Properties
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
    }
}