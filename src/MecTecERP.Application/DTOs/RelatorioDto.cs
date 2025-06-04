using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.DTOs;

public class RelatorioEstoqueDto
{
    public List<RelatorioEstoqueProdutoDto> Produtos { get; set; } = new();
    public RelatorioEstoqueResumoDto Resumo { get; set; } = new();
    public FiltroRelatorioEstoqueDto Filtros { get; set; } = new();
    public DateTime DataGeracao { get; set; } = DateTime.Now;
}

public class RelatorioEstoqueProdutoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public string FornecedorNome { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueMaximo { get; set; }
    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public string StatusEstoque { get; set; } = string.Empty;
    public string StatusClass { get; set; } = string.Empty;
    public DateTime? UltimaMovimentacao { get; set; }
    public string? Localizacao { get; set; }
}

public class RelatorioEstoqueResumoDto
{
    public int TotalProdutos { get; set; }
    public int ProdutosAtivos { get; set; }
    public int ProdutosInativos { get; set; }
    public int ProdutosEstoqueBaixo { get; set; }
    public int ProdutosEstoqueZerado { get; set; }
    public int ProdutosEstoqueExcesso { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public decimal ValorEstoqueBaixo { get; set; }
    public decimal ValorEstoqueZerado { get; set; }
    public decimal ValorEstoqueExcesso { get; set; }
    public decimal PercentualEstoqueBaixo { get; set; }
    public decimal PercentualEstoqueZerado { get; set; }
    public decimal PercentualEstoqueExcesso { get; set; }
}

public class FiltroRelatorioEstoqueDto
{
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public int? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public StatusEstoque? StatusEstoque { get; set; }
    public bool? Ativo { get; set; }
    public string? CodigoProduto { get; set; }
    public string? NomeProduto { get; set; }
    public decimal? EstoqueMinimo { get; set; }
    public decimal? EstoqueMaximo { get; set; }
    public DateTime? DataUltimaMovimentacao { get; set; }
    public string? OrdenarPor { get; set; } = "Nome";
    public bool OrdemDecrescente { get; set; } = false;
}

public class RelatorioMovimentacoesDto
{
    public List<RelatorioMovimentacaoItemDto> Movimentacoes { get; set; } = new();
    public RelatorioMovimentacoesResumoDto Resumo { get; set; } = new();
    public FiltroRelatorioMovimentacoesDto Filtros { get; set; } = new();
    public DateTime DataGeracao { get; set; } = DateTime.Now;
}

public class RelatorioMovimentacaoItemDto
{
    public int Id { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public string TipoTexto { get; set; } = string.Empty;
    public string TipoClass { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal EstoqueAnterior { get; set; }
    public decimal EstoquePosterior { get; set; }
    public string? Documento { get; set; }
    public string? Observacoes { get; set; }
    public string? InventarioDescricao { get; set; }
}

public class RelatorioMovimentacoesResumoDto
{
    public int TotalMovimentacoes { get; set; }
    public int TotalEntradas { get; set; }
    public int TotalSaidas { get; set; }
    public int TotalAjustes { get; set; }
    public int TotalInventarios { get; set; }
    public decimal QuantidadeTotalEntradas { get; set; }
    public decimal QuantidadeTotalSaidas { get; set; }
    public decimal QuantidadeTotalAjustes { get; set; }
    public decimal ValorTotalEntradas { get; set; }
    public decimal ValorTotalSaidas { get; set; }
    public decimal ValorTotalAjustes { get; set; }
    public decimal SaldoQuantidade { get; set; }
    public decimal SaldoValor { get; set; }
    public decimal TicketMedioEntrada { get; set; }
    public decimal TicketMedioSaida { get; set; }
}

public class FiltroRelatorioMovimentacoesDto
{
    public DateTime DataInicio { get; set; } = DateTime.Now.AddDays(-30);
    public DateTime DataFim { get; set; } = DateTime.Now;
    public int? ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public TipoMovimentacaoEstoque? Tipo { get; set; }
    public int? InventarioId { get; set; }
    public string? Documento { get; set; }
    public string? OrdenarPor { get; set; } = "DataMovimentacao";
    public bool OrdemDecrescente { get; set; } = true;
}

public class RelatorioInventarioDto
{
    public InventarioDto Inventario { get; set; } = new();
    public List<RelatorioInventarioItemDto> Itens { get; set; } = new();
    public RelatorioInventarioResumoDto Resumo { get; set; } = new();
    public DateTime DataGeracao { get; set; } = DateTime.Now;
}

public class RelatorioInventarioItemDto
{
    public int Id { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal EstoqueSistema { get; set; }
    public decimal? EstoqueContado { get; set; }
    public decimal? Diferenca { get; set; }
    public decimal? DiferencaPercentual { get; set; }
    public decimal? ValorDiferenca { get; set; }
    public DateTime? DataContagem { get; set; }
    public string? Observacoes { get; set; }
    public bool Contado { get; set; }
    public bool TemDivergencia { get; set; }
    public string StatusTexto { get; set; } = string.Empty;
    public string StatusClass { get; set; } = string.Empty;
}

public class RelatorioInventarioResumoDto
{
    public int TotalItens { get; set; }
    public int ItensContados { get; set; }
    public int ItensPendentes { get; set; }
    public int ItensComDivergencia { get; set; }
    public int ItensSemDivergencia { get; set; }
    public decimal PercentualConcluido { get; set; }
    public decimal PercentualDivergencia { get; set; }
    public decimal ValorTotalDivergencias { get; set; }
    public decimal ValorDivergenciasPositivas { get; set; }
    public decimal ValorDivergenciasNegativas { get; set; }
    public decimal MaiorDivergenciaPositiva { get; set; }
    public decimal MaiorDivergenciaNegativa { get; set; }
}

public class RelatorioValorizacaoEstoqueDto
{
    public List<RelatorioValorizacaoItemDto> Itens { get; set; } = new();
    public RelatorioValorizacaoResumoDto Resumo { get; set; } = new();
    public FiltroRelatorioValorizacaoDto Filtros { get; set; } = new();
    public DateTime DataGeracao { get; set; } = DateTime.Now;
}

public class RelatorioValorizacaoItemDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public string FornecedorNome { get; set; } = string.Empty;
    public decimal EstoqueAtual { get; set; }
    public decimal PrecoCompra { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal ValorEstoqueCompra { get; set; }
    public decimal ValorEstoqueVenda { get; set; }
    public decimal MargemLucro { get; set; }
    public decimal PercentualMargemLucro { get; set; }
    public decimal PercentualValorTotal { get; set; }
}

public class RelatorioValorizacaoResumoDto
{
    public int TotalProdutos { get; set; }
    public decimal ValorTotalCompra { get; set; }
    public decimal ValorTotalVenda { get; set; }
    public decimal MargemLucroTotal { get; set; }
    public decimal PercentualMargemMedia { get; set; }
    public decimal MaiorValorIndividual { get; set; }
    public decimal MenorValorIndividual { get; set; }
    public decimal ValorMedioItem { get; set; }
}

public class FiltroRelatorioValorizacaoDto
{
    public int? CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public int? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public bool ApenasComEstoque { get; set; } = true;
    public bool? Ativo { get; set; }
    public string? OrdenarPor { get; set; } = "ValorEstoqueVenda";
    public bool OrdemDecrescente { get; set; } = true;
}