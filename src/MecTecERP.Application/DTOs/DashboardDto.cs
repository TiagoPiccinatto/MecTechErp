namespace MecTecERP.Application.DTOs;

public class DashboardEstoqueDto
{
    public ResumoEstoqueDto ResumoEstoque { get; set; } = new();
    public List<MovimentacaoRecenteDto> MovimentacoesRecentes { get; set; } = new();
    public List<ProdutoEstoqueBaixoDto> ProdutosEstoqueBaixo { get; set; } = new();
    public List<ProdutoEstoqueZeradoDto> ProdutosEstoqueZerado { get; set; } = new();
    public List<CategoriaResumoDto> ResumoCategoria { get; set; } = new();
    public List<MovimentacaoPorDiaDto> MovimentacoesPorDia { get; set; } = new();
    public List<ValorEstoquePorCategoriaDto> ValorEstoquePorCategoria { get; set; } = new();
}

public class ResumoEstoqueDto
{
    public int TotalProdutos { get; set; }
    public int ProdutosAtivos { get; set; }
    public int ProdutosInativos { get; set; }
    public decimal ValorTotalEstoque { get; set; }
    public int ProdutosEstoqueBaixo { get; set; }
    public int ProdutosEstoqueZerado { get; set; }
    public int TotalCategorias { get; set; }
    public int TotalFornecedores { get; set; }
    public int MovimentacoesHoje { get; set; }
    public int MovimentacoesMes { get; set; }
    public decimal ValorMovimentacoesMes { get; set; }
}

public class MovimentacaoRecenteDto
{
    public int Id { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoNome { get; set; } = string.Empty;
    public string TipoTexto { get; set; } = string.Empty;
    public string TipoClass { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataMovimentacao { get; set; }
    public string? Documento { get; set; }
}

public class ProdutoEstoqueBaixoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public decimal EstoqueAtual { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal PercentualEstoque { get; set; }
    public string StatusClass { get; set; } = string.Empty;
}

public class ProdutoEstoqueZeradoDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public DateTime? UltimaMovimentacao { get; set; }
    public int DiasParado { get; set; }
}

public class CategoriaResumoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TotalProdutos { get; set; }
    public decimal ValorEstoque { get; set; }
    public int ProdutosEstoqueBaixo { get; set; }
    public int ProdutosEstoqueZerado { get; set; }
}

public class MovimentacaoPorDiaDto
{
    public DateTime Data { get; set; }
    public string DataFormatada { get; set; } = string.Empty;
    public int TotalEntradas { get; set; }
    public int TotalSaidas { get; set; }
    public decimal ValorEntradas { get; set; }
    public decimal ValorSaidas { get; set; }
    public decimal SaldoQuantidade { get; set; }
    public decimal SaldoValor { get; set; }
}

public class ValorEstoquePorCategoriaDto
{
    public string CategoriaNome { get; set; } = string.Empty;
    public decimal ValorEstoque { get; set; }
    public int TotalProdutos { get; set; }
    public decimal PercentualTotal { get; set; }
    public string Cor { get; set; } = string.Empty;
}

public class FiltrosDashboardDto
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int? CategoriaId { get; set; }
    public int? FornecedorId { get; set; }
    public bool ApenasAtivos { get; set; } = true;
    public int LimiteItens { get; set; } = 10;
}

public class GraficoMovimentacoesDto
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> Entradas { get; set; } = new();
    public List<decimal> Saidas { get; set; } = new();
}

public class GraficoCategoriasDto
{
    public List<string> Labels { get; set; } = new();
    public List<int> Data { get; set; } = new();
    public List<string> BackgroundColors { get; set; } = new();
}

public class GraficoValorEstoqueDto
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> Data { get; set; } = new();
}

public class GraficoBaixoEstoqueDto
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> EstoqueAtual { get; set; } = new();
    public List<decimal> EstoqueMinimo { get; set; } = new();
}