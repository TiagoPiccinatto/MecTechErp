using MecTecERP.Domain.Common;
using MecTecERP.Domain.Enums;
using System;
using System.Collections.Generic;

namespace MecTecERP.Domain.Entities
{
    public class OrdemServico : BaseEntity // Alinhado com BaseEntity.Id (int)
    {
        public string Numero { get; set; } = string.Empty;
        public int ClienteId { get; set; } // Alterado para int
        public int VeiculoId { get; set; } // Alterado para int
        
        public DateTime DataEntrada { get; set; } = DateTime.UtcNow;
        public DateTime? DataPrevisaoEntrega { get; set; }
        public DateTime? DataConclusao { get; set; }

        public string? ProblemaRelatado { get; set; }
        public string? DiagnosticoTecnico { get; set; }

        public decimal ValorServicos { get; set; } = 0;
        public decimal ValorPecas { get; set; } = 0;
        public decimal ValorDesconto { get; set; } = 0;
        public decimal ValorTotal { get; set; } = 0;

        public StatusOrdemServico Status { get; set; }
        
        public string? ObservacoesInternas { get; set; }
        public string? ObservacoesCliente { get; set; }

        public int? MecanicoResponsavelId { get; set; } // Alterado para int? (assumindo entidade Funcionario com Id int)
        // public virtual Funcionario MecanicoResponsavel { get; set; } // Se houver entidade Funcionario

        public bool OrcamentoAprovado { get; set; } = false;
        public DateTime? DataAprovacaoOrcamento { get; set; }

        public int? QuilometragemEntrada { get; set; }

        // DataCadastro (DataCriacao), UsuarioCadastro (UsuarioCriacao) vêm de BaseEntity
        // public DateTime DataCadastro { get; set; } = DateTime.UtcNow; // Removido
        // public Guid? UsuarioCadastroId { get; set; } // Removido


        // Navigation Properties
        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Veiculo Veiculo { get; set; } = null!;
        public virtual ICollection<OrdemServicoItem> Itens { get; set; } = new List<OrdemServicoItem>();
        public virtual ICollection<OrdemServicoFoto> Fotos { get; set; } = new List<OrdemServicoFoto>();
    }

    public class OrdemServicoFoto : BaseEntity // Alinhado com BaseEntity.Id (int)
    {
        public int OrdemServicoId { get; set; } // Alterado para int
        public string UrlFoto { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        // DataUpload pode ser DataCriacao de BaseEntity
        // public DateTime DataUpload { get; set; } = DateTime.UtcNow; // Removido, usar DataCriacao de BaseEntity

        public virtual OrdemServico OrdemServico { get; set; } = null!;
    }
}