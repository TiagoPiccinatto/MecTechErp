using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;
using System;
using System.Collections.Generic;

namespace MecTecERP.Domain.Entities
{
    public class Cliente : BaseEntity // Alinhado com BaseEntity.Id (int)
    {
        public TipoPessoa TipoPessoa { get; set; }
        public string NomeRazaoSocial { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string? RgIe { get; set; }

        public string? Telefone1 { get; set; }
        public string? Telefone2 { get; set; }

        public string? Email { get; set; }

        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }

        public string? Observacoes { get; set; }
        // Ativo, DataCadastro, DataUltimaAtualizacao, UsuarioCadastro, UsuarioUltimaAtualizacao vêm de BaseEntity
        // public bool Ativo { get; set; } = true; // PLANO: Ativo -> Vem de BaseEntity
        // public DateTime DataCadastro { get; set; } = DateTime.UtcNow; // PLANO: DataCadastro -> Vem de BaseEntity como DataCriacao
        // public DateTime? DataUltimaAtualizacao { get; set; } // PLANO: DataUltimaAtualizacao -> Vem de BaseEntity

        // public string? UsuarioCadastroId { get; set; } // -> Vem de BaseEntity como UsuarioCriacao (string)
        // public string? UsuarioUltimaAtualizacaoId { get; set; } // -> Vem de BaseEntity como UsuarioAtualizacao (string)

        // Navigation Properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
        public virtual ICollection<OrdemServico> OrdensServico { get; set; } = new List<OrdemServico>();
    }
}