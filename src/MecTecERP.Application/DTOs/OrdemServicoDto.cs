using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class OrdemServicoFotoDto : BaseDto
{
    public int OrdemServicoId { get; set; }
    [Required]
    public string UrlFoto { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataUpload { get; set; } // Mapeado de DataCriacao da entidade OrdemServicoFoto
}

public class OrdemServicoFotoCreateDto
{
    [Required]
    public int OrdemServicoId { get; set; } // Necessário para associar ao criar
    [Required(ErrorMessage = "A URL da foto é obrigatória.")]
    [StringLength(500, ErrorMessage = "A URL da foto deve ter no máximo 500 caracteres.")]
    public string UrlFoto { get; set; } = string.Empty;
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
    public string? Descricao { get; set; }
}


public class OrdemServicoDto : BaseDto // Id, DataCriacao (DataCadastroOs), DataAtualizacao, Ativo
{
    [Required]
    [StringLength(20)]
    public string Numero { get; set; } = string.Empty;

    [Required]
    public int ClienteId { get; set; }
    public string? ClienteNome { get; set; }

    [Required]
    public int VeiculoId { get; set; }
    public string? VeiculoPlaca { get; set; }
    public string? VeiculoDescricao { get; set; } // Ex: "Gol 1.0 Preto"

    [Required]
    public DateTime DataEntrada { get; set; }
    public DateTime? DataPrevisaoEntrega { get; set; }
    public DateTime? DataConclusao { get; set; }

    [StringLength(2000)]
    public string? ProblemaRelatado { get; set; }
    [StringLength(2000)]
    public string? DiagnosticoTecnico { get; set; }

    public decimal ValorServicos { get; set; }
    public decimal ValorPecas { get; set; }
    public decimal ValorDesconto { get; set; }
    public decimal ValorTotal { get; set; } // Calculado: Servicos + Pecas - Desconto

    [Required]
    public StatusOrdemServico Status { get; set; }

    [StringLength(1000)]
    public string? ObservacoesInternas { get; set; }
    [StringLength(1000)]
    public string? ObservacoesCliente { get; set; }

    public int? MecanicoResponsavelId { get; set; }
    public string? MecanicoResponsavelNome { get; set; }

    public bool OrcamentoAprovado { get; set; }
    public DateTime? DataAprovacaoOrcamento { get; set; }

    public int? QuilometragemEntrada { get; set; }

    public List<OrdemServicoItemDto> Itens { get; set; } = new();
    public List<OrdemServicoFotoDto> Fotos { get; set; } = new();
}

public class OrdemServicoCreateDto
{
    [Required(ErrorMessage = "O Número da OS é obrigatório.")]
    [StringLength(20, ErrorMessage = "O Número da OS deve ter no máximo 20 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Cliente é obrigatório.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O Veículo é obrigatório.")]
    public int VeiculoId { get; set; }

    [Required(ErrorMessage = "A Data de Entrada é obrigatória.")]
    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;

    public DateTime? DataPrevisaoEntrega { get; set; }

    [StringLength(2000, ErrorMessage = "O Problema Relatado deve ter no máximo 2000 caracteres.")]
    public string? ProblemaRelatado { get; set; }
    [StringLength(2000, ErrorMessage = "O Diagnóstico Técnico deve ter no máximo 2000 caracteres.")]
    public string? DiagnosticoTecnico { get; set; }

    // Valores serão calculados a partir dos itens, mas podem ser informados inicialmente se necessário.
    [Range(0, 9999999.99, ErrorMessage = "Valor de Serviços inválido.")]
    public decimal ValorServicos { get; set; } = 0;
    [Range(0, 9999999.99, ErrorMessage = "Valor de Peças inválido.")]
    public decimal ValorPecas { get; set; } = 0;
    [Range(0, 9999999.99, ErrorMessage = "Valor de Desconto inválido.")]
    public decimal ValorDesconto { get; set; } = 0;

    [Required(ErrorMessage = "O Status inicial é obrigatório.")]
    public StatusOrdemServico Status { get; set; } = StatusOrdemServico.Protocolo;

    [StringLength(1000, ErrorMessage = "Observações Internas devem ter no máximo 1000 caracteres.")]
    public string? ObservacoesInternas { get; set; }
    [StringLength(1000, ErrorMessage = "Observações ao Cliente devem ter no máximo 1000 caracteres.")]
    public string? ObservacoesCliente { get; set; }

    public int? MecanicoResponsavelId { get; set; }

    public bool OrcamentoAprovado { get; set; } = false;
    public DateTime? DataAprovacaoOrcamento { get; set; }

    [Range(0, 9999999, ErrorMessage = "Km de Entrada inválido.")]
    public int? QuilometragemEntrada { get; set; }

    public bool Ativo { get; set; } = true;

    public List<OrdemServicoItemCreateDto> Itens { get; set; } = new();
    public List<OrdemServicoFotoCreateDto> Fotos { get; set; } = new(); // Permitir adicionar fotos na criação
}

public class OrdemServicoUpdateDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O Número da OS é obrigatório.")]
    [StringLength(20, ErrorMessage = "O Número da OS deve ter no máximo 20 caracteres.")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Cliente é obrigatório.")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "O Veículo é obrigatório.")]
    public int VeiculoId { get; set; }

    [Required(ErrorMessage = "A Data de Entrada é obrigatória.")]
    public DateTime DataEntrada { get; set; }
    public DateTime? DataPrevisaoEntrega { get; set; }
    public DateTime? DataConclusao { get; set; }


    [StringLength(2000, ErrorMessage = "O Problema Relatado deve ter no máximo 2000 caracteres.")]
    public string? ProblemaRelatado { get; set; }
    [StringLength(2000, ErrorMessage = "O Diagnóstico Técnico deve ter no máximo 2000 caracteres.")]
    public string? DiagnosticoTecnico { get; set; }

    [Range(0, 9999999.99, ErrorMessage = "Valor de Serviços inválido.")]
    public decimal ValorServicos { get; set; }
    [Range(0, 9999999.99, ErrorMessage = "Valor de Peças inválido.")]
    public decimal ValorPecas { get; set; }
    [Range(0, 9999999.99, ErrorMessage = "Valor de Desconto inválido.")]
    public decimal ValorDesconto { get; set; }

    [Required(ErrorMessage = "O Status é obrigatório.")]
    public StatusOrdemServico Status { get; set; }

    [StringLength(1000, ErrorMessage = "Observações Internas devem ter no máximo 1000 caracteres.")]
    public string? ObservacoesInternas { get; set; }
    [StringLength(1000, ErrorMessage = "Observações ao Cliente devem ter no máximo 1000 caracteres.")]
    public string? ObservacoesCliente { get; set; }

    public int? MecanicoResponsavelId { get; set; }

    public bool OrcamentoAprovado { get; set; }
    public DateTime? DataAprovacaoOrcamento { get; set; }

    [Range(0, 9999999, ErrorMessage = "Km de Entrada inválido.")]
    public int? QuilometragemEntrada { get; set; }

    public bool Ativo { get; set; }

    public List<OrdemServicoItemUpdateDto> Itens { get; set; } = new();
    // Fotos geralmente são gerenciadas em endpoints separados (Upload/Delete), mas pode-se permitir adicionar novas.
    public List<OrdemServicoFotoCreateDto> NovasFotos { get; set; } = new();
    public List<int> FotosParaRemoverIds { get; set; } = new(); // IDs das fotos a serem removidas
}

public class OrdemServicoListDto // DTO para listagens
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime DataEntrada { get; set; } // Era DataAbertura
    public DateTime? DataConclusao { get; set; } // Era DataFechamento
    public StatusOrdemServico Status { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public string VeiculoPlaca { get; set; } = string.Empty;
    public string VeiculoDescricao { get; set; } = string.Empty; // Era VeiculoModelo
    public decimal ValorTotal { get; set; }
    public bool Ativo { get; set; }
    public string? MecanicoResponsavelNome {get;set;} // Era ResponsavelTecnico
}

public class OrdemServicoFiltroDto : FiltroBaseDto
{
    public string? Numero { get; set; }
    public StatusOrdemServico? Status { get; set; }
    public DateTime? DataEntradaInicio { get; set; } // Era DataAberturaInicio
    public DateTime? DataEntradaFim { get; set; } // Era DataAberturaFim
    public DateTime? DataConclusaoInicio { get; set; } // Era DataFechamentoInicio
    public DateTime? DataConclusaoFim { get; set; } // Era DataFechamentoFim
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public int? VeiculoId { get; set; }
    public string? VeiculoPlaca { get; set; }
    public int? MecanicoResponsavelId { get; set; } // Era ResponsavelTecnico (string)
    public decimal? ValorMinimoTotal { get; set; }
    public decimal? ValorMaximoTotal { get; set; }
    public bool? OrcamentoAprovado { get; set; } // Novo filtro
    public bool? Ativo { get; set; } // Novo filtro
}

// DTOs para itens da ordem de serviço (ajustes menores e mantendo a estrutura existente)
public class OrdemServicoItemDto : BaseDto // Adicionado BaseDto para ter Id, etc.
{
    public int OrdemServicoId { get; set; } // Adicionado para referência
    [Required]
    public TipoItemOrdemServico Tipo { get; set; }
    [Required]
    [StringLength(200)]
    public string Descricao { get; set; } = string.Empty;
    [Required]
    public decimal Quantidade { get; set; }
    [Required]
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; } // Calculado
    public decimal? DescontoItem { get; set; } // Renomeado de Desconto
    public int? ProdutoId { get; set; }
    public string? ProdutoNome { get; set; } // Para exibição
    [StringLength(500)]
    public string? ObservacoesItem { get; set; } // Adicionado
}

public class OrdemServicoItemCreateDto
{
    [Required(ErrorMessage = "O tipo do item é obrigatório")]
    public TipoItemOrdemServico Tipo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")] // Ajustado range mínimo
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior ou igual a zero")] // Permitir valor zero se necessário
    public decimal ValorUnitario { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto do item deve ser maior ou igual a zero")]
    public decimal? DescontoItem { get; set; } // Renomeado

    public int? ProdutoId { get; set; }
    [StringLength(500, ErrorMessage = "As observações do item devem ter no máximo 500 caracteres")]
    public string? ObservacoesItem { get; set; } // Adicionado
}

public class OrdemServicoItemUpdateDto
{
    [Required]
    public int Id { get; set; } // Item deve ter ID para ser atualizado

    [Required(ErrorMessage = "O tipo do item é obrigatório")]
    public TipoItemOrdemServico Tipo { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória")]
    [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }

    [Required(ErrorMessage = "O valor unitário é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "O valor unitário deve ser maior ou igual a zero")]
    public decimal ValorUnitario { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O desconto do item deve ser maior ou igual a zero")]
    public decimal? DescontoItem { get; set; } // Renomeado

    public int? ProdutoId { get; set; }
    [StringLength(500, ErrorMessage = "As observações do item devem ter no máximo 500 caracteres")]
    public string? ObservacoesItem { get; set; } // Adicionado
}