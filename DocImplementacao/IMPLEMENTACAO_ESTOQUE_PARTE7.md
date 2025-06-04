# MecTecERP - Implementação do Módulo de Estoque - Parte 7

## Dashboard de Estoque e Sistema de Alertas

### 1. Dashboard Principal - Pages/Dashboard/Estoque.razor

```razor
@page "/dashboard/estoque"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@using MecTecERP.Domain.Enums
@inject IProdutoService ProdutoService
@inject IEstoqueService EstoqueService
@inject IMovimentacaoEstoqueService MovimentacaoService
@inject IJSRuntime JSRuntime

<PageTitle>Dashboard de Estoque</PageTitle>

<div class="container-fluid">
    <!-- Cards de Resumo -->
    <div class="row mb-4">
        <div class="col-xl-3 col-md-6">
            <div class="card bg-primary text-white mb-4">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <div class="text-white-75 small">Total de Produtos</div>
                            <div class="text-lg fw-bold">@totalProdutos</div>
                        </div>
                        <i class="fas fa-boxes fa-2x text-white-50"></i>
                    </div>
                </div>
                <div class="card-footer d-flex align-items-center justify-content-between">
                    <a class="small text-white stretched-link" href="/produtos">Ver Detalhes</a>
                    <div class="small text-white"><i class="fas fa-angle-right"></i></div>
                </div>
            </div>
        </div>
        
        <div class="col-xl-3 col-md-6">
            <div class="card bg-success text-white mb-4">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <div class="text-white-75 small">Valor Total do Estoque</div>
                            <div class="text-lg fw-bold">@valorTotalEstoque.ToString("C")</div>
                        </div>
                        <i class="fas fa-dollar-sign fa-2x text-white-50"></i>
                    </div>
                </div>
                <div class="card-footer d-flex align-items-center justify-content-between">
                    <a class="small text-white stretched-link" href="/relatorios/estoque">Ver Relatório</a>
                    <div class="small text-white"><i class="fas fa-angle-right"></i></div>
                </div>
            </div>
        </div>
        
        <div class="col-xl-3 col-md-6">
            <div class="card bg-warning text-white mb-4">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <div class="text-white-75 small">Estoque Baixo</div>
                            <div class="text-lg fw-bold">@produtosEstoqueBaixo</div>
                        </div>
                        <i class="fas fa-exclamation-triangle fa-2x text-white-50"></i>
                    </div>
                </div>
                <div class="card-footer d-flex align-items-center justify-content-between">
                    <a class="small text-white stretched-link" href="/produtos?filtro=estoque-baixo">Ver Produtos</a>
                    <div class="small text-white"><i class="fas fa-angle-right"></i></div>
                </div>
            </div>
        </div>
        
        <div class="col-xl-3 col-md-6">
            <div class="card bg-danger text-white mb-4">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <div class="text-white-75 small">Sem Estoque</div>
                            <div class="text-lg fw-bold">@produtosSemEstoque</div>
                        </div>
                        <i class="fas fa-times-circle fa-2x text-white-50"></i>
                    </div>
                </div>
                <div class="card-footer d-flex align-items-center justify-content-between">
                    <a class="small text-white stretched-link" href="/produtos?filtro=sem-estoque">Ver Produtos</a>
                    <div class="small text-white"><i class="fas fa-angle-right"></i></div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- Gráfico de Movimentações -->
        <div class="col-xl-6">
            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-chart-line me-1"></i>
                    Movimentações dos Últimos 30 Dias
                </div>
                <div class="card-body">
                    <canvas id="chartMovimentacoes" width="100%" height="40"></canvas>
                </div>
            </div>
        </div>
        
        <!-- Gráfico de Produtos por Categoria -->
        <div class="col-xl-6">
            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-chart-pie me-1"></i>
                    Produtos por Categoria
                </div>
                <div class="card-body">
                    <canvas id="chartCategorias" width="100%" height="40"></canvas>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <!-- Produtos com Estoque Baixo -->
        <div class="col-xl-6">
            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-exclamation-triangle me-1"></i>
                    Produtos com Estoque Baixo
                </div>
                <div class="card-body">
                    @if (produtosEstoqueBaixoLista?.Any() == true)
                    {
                        <div class="table-responsive">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>Produto</th>
                                        <th class="text-end">Atual</th>
                                        <th class="text-end">Mínimo</th>
                                        <th class="text-center">Ação</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @foreach (var produto in produtosEstoqueBaixoLista.Take(10))
                                    {
                                        <tr>
                                            <td>
                                                <strong>@produto.Codigo</strong><br>
                                                <small>@produto.Nome</small>
                                            </td>
                                            <td class="text-end">@produto.EstoqueAtual.ToString("N2")</td>
                                            <td class="text-end">@produto.EstoqueMinimo.ToString("N2")</td>
                                            <td class="text-center">
                                                <button class="btn btn-sm btn-primary" @onclick="() => AbrirModalEntrada(produto.Id)">
                                                    <i class="fas fa-plus"></i>
                                                </button>
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        </div>
                        @if (produtosEstoqueBaixoLista.Count() > 10)
                        {
                            <div class="text-center">
                                <a href="/produtos?filtro=estoque-baixo" class="btn btn-outline-primary btn-sm">
                                    Ver todos (@produtosEstoqueBaixoLista.Count())
                                </a>
                            </div>
                        }
                    }
                    else
                    {
                        <div class="text-center py-3">
                            <i class="fas fa-check-circle fa-2x text-success mb-2"></i>
                            <p class="text-muted mb-0">Nenhum produto com estoque baixo</p>
                        </div>
                    }
                </div>
            </div>
        </div>
        
        <!-- Últimas Movimentações -->
        <div class="col-xl-6">
            <div class="card mb-4">
                <div class="card-header">
                    <i class="fas fa-history me-1"></i>
                    Últimas Movimentações
                </div>
                <div class="card-body">
                    @if (ultimasMovimentacoes?.Any() == true)
                    {
                        <div class="table-responsive">
                            <table class="table table-sm">
                                <thead>
                                    <tr>
                                        <th>Data</th>
                                        <th>Produto</th>
                                        <th>Tipo</th>
                                        <th class="text-end">Quantidade</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @foreach (var mov in ultimasMovimentacoes.Take(10))
                                    {
                                        <tr>
                                            <td>
                                                <small>@mov.DataMovimentacao.ToString("dd/MM HH:mm")</small>
                                            </td>
                                            <td>
                                                <strong>@mov.ProdutoCodigo</strong><br>
                                                <small>@mov.ProdutoNome</small>
                                            </td>
                                            <td>
                                                <span class="badge @(GetBadgeClassTipoMovimentacao(mov.Tipo))">
                                                    @GetDescricaoTipoMovimentacao(mov.Tipo)
                                                </span>
                                            </td>
                                            <td class="text-end">
                                                <span class="@(mov.Tipo == TipoMovimentacao.Entrada ? "text-success" : "text-danger")">
                                                    @(mov.Tipo == TipoMovimentacao.Entrada ? "+" : "-")@mov.Quantidade.ToString("N2")
                                                </span>
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        </div>
                        <div class="text-center">
                            <a href="/movimentacoes" class="btn btn-outline-primary btn-sm">
                                Ver todas as movimentações
                            </a>
                        </div>
                    }
                    else
                    {
                        <div class="text-center py-3">
                            <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                            <p class="text-muted mb-0">Nenhuma movimentação recente</p>
                        </div>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Modal de Entrada Rápida -->
<div class="modal fade" id="modalEntradaRapida" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <i class="fas fa-plus me-2"></i>
                    Entrada Rápida de Estoque
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                @if (produtoEntradaRapida != null)
                {
                    <div class="mb-3">
                        <label class="form-label">Produto</label>
                        <input type="text" class="form-control" value="@($"{produtoEntradaRapida.Codigo} - {produtoEntradaRapida.Nome}")" readonly />
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Estoque Atual</label>
                        <input type="text" class="form-control" value="@produtoEntradaRapida.EstoqueAtual.ToString("N2")" readonly />
                    </div>
                    <div class="mb-3">
                        <label for="quantidadeEntrada" class="form-label">Quantidade de Entrada *</label>
                        <input type="number" id="quantidadeEntrada" class="form-control" @bind="quantidadeEntrada" min="0.01" step="0.01" />
                    </div>
                    <div class="mb-3">
                        <label for="observacaoEntrada" class="form-label">Observação</label>
                        <textarea id="observacaoEntrada" class="form-control" rows="3" @bind="observacaoEntrada" placeholder="Motivo da entrada..."></textarea>
                    </div>
                }
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-primary" @onclick="ProcessarEntradaRapida" disabled="@(quantidadeEntrada <= 0)">
                    <i class="fas fa-save me-1"></i>
                    Confirmar Entrada
                </button>
            </div>
        </div>
    </div>
</div>

@code {
    private int totalProdutos = 0;
    private decimal valorTotalEstoque = 0;
    private int produtosEstoqueBaixo = 0;
    private int produtosSemEstoque = 0;
    
    private IEnumerable<ProdutoListDto> produtosEstoqueBaixoLista = new List<ProdutoListDto>();
    private IEnumerable<MovimentacaoEstoqueDto> ultimasMovimentacoes = new List<MovimentacaoEstoqueDto>();
    
    private ProdutoDto produtoEntradaRapida;
    private decimal quantidadeEntrada = 0;
    private string observacaoEntrada = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
        await CarregarGraficos();
    }

    private async Task CarregarDados()
    {
        try
        {
            // Carregar resumo
            var (produtos, _) = await ProdutoService.GetPagedAsync(1, int.MaxValue);
            totalProdutos = produtos.Count();
            valorTotalEstoque = produtos.Sum(p => p.EstoqueAtual * p.PrecoCusto);
            
            // Produtos com estoque baixo
            produtosEstoqueBaixoLista = await ProdutoService.GetProdutosEstoqueBaixoAsync();
            produtosEstoqueBaixo = produtosEstoqueBaixoLista.Count();
            
            // Produtos sem estoque
            var produtosSemEstoqueLista = await ProdutoService.GetProdutosSemEstoqueAsync();
            produtosSemEstoque = produtosSemEstoqueLista.Count();
            
            // Últimas movimentações
            ultimasMovimentacoes = await MovimentacaoService.GetRecentesAsync(20);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("console.error", $"Erro ao carregar dados: {ex.Message}");
        }
    }

    private async Task CarregarGraficos()
    {
        try
        {
            // Dados para gráfico de movimentações
            var dataInicio = DateTime.Now.AddDays(-30);
            var movimentacoesPeriodo = await MovimentacaoService.GetByPeriodoAsync(dataInicio, DateTime.Now);
            
            var dadosMovimentacoes = movimentacoesPeriodo
                .GroupBy(m => m.DataMovimentacao.Date)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Data = g.Key.ToString("dd/MM"),
                    Entradas = g.Where(m => m.Tipo == TipoMovimentacao.Entrada).Sum(m => m.Quantidade),
                    Saidas = g.Where(m => m.Tipo == TipoMovimentacao.Saida).Sum(m => m.Quantidade)
                })
                .ToList();

            await JSRuntime.InvokeVoidAsync("criarGraficoMovimentacoes", dadosMovimentacoes);
            
            // Dados para gráfico de categorias
            var (produtos, _) = await ProdutoService.GetPagedAsync(1, int.MaxValue);
            var dadosCategorias = produtos
                .GroupBy(p => p.CategoriaNome)
                .Select(g => new {
                    Categoria = g.Key,
                    Quantidade = g.Count()
                })
                .ToList();

            await JSRuntime.InvokeVoidAsync("criarGraficoCategorias", dadosCategorias);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("console.error", $"Erro ao carregar gráficos: {ex.Message}");
        }
    }

    private async Task AbrirModalEntrada(int produtoId)
    {
        try
        {
            produtoEntradaRapida = await ProdutoService.GetByIdAsync(produtoId);
            quantidadeEntrada = 0;
            observacaoEntrada = string.Empty;
            
            await JSRuntime.InvokeVoidAsync("new bootstrap.Modal", "#modalEntradaRapida")?.InvokeVoidAsync("show");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao abrir modal: {ex.Message}");
        }
    }

    private async Task ProcessarEntradaRapida()
    {
        try
        {
            if (produtoEntradaRapida == null || quantidadeEntrada <= 0)
                return;

            await EstoqueService.RegistrarEntradaAsync(new MovimentacaoEstoqueCreateDto
            {
                ProdutoId = produtoEntradaRapida.Id,
                Quantidade = quantidadeEntrada,
                Observacoes = observacaoEntrada,
                NumeroDocumento = $"ENT-{DateTime.Now:yyyyMMddHHmmss}"
            });

            await JSRuntime.InvokeVoidAsync("bootstrap.Modal.getInstance", "#modalEntradaRapida")?.InvokeVoidAsync("hide");
            await JSRuntime.InvokeVoidAsync("alert", "Entrada registrada com sucesso!");
            
            // Recarregar dados
            await CarregarDados();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao processar entrada: {ex.Message}");
        }
    }

    private string GetBadgeClassTipoMovimentacao(TipoMovimentacao tipo)
    {
        return tipo switch
        {
            TipoMovimentacao.Entrada => "bg-success",
            TipoMovimentacao.Saida => "bg-danger",
            TipoMovimentacao.Ajuste => "bg-warning",
            TipoMovimentacao.Transferencia => "bg-info",
            _ => "bg-secondary"
        };
    }

    private string GetDescricaoTipoMovimentacao(TipoMovimentacao tipo)
    {
        return tipo switch
        {
            TipoMovimentacao.Entrada => "Entrada",
            TipoMovimentacao.Saida => "Saída",
            TipoMovimentacao.Ajuste => "Ajuste",
            TipoMovimentacao.Transferencia => "Transferência",
            _ => "Outros"
        };
    }
}
```

### 2. JavaScript para Gráficos - wwwroot/js/dashboard-estoque.js

```javascript
// Função para criar gráfico de movimentações
window.criarGraficoMovimentacoes = function(dados) {
    const ctx = document.getElementById('chartMovimentacoes');
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.chartMovimentacoes) {
        window.chartMovimentacoes.destroy();
    }

    const labels = dados.map(d => d.data);
    const entradas = dados.map(d => d.entradas);
    const saidas = dados.map(d => d.saidas);

    window.chartMovimentacoes = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Entradas',
                data: entradas,
                borderColor: 'rgb(75, 192, 192)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                tension: 0.1
            }, {
                label: 'Saídas',
                data: saidas,
                borderColor: 'rgb(255, 99, 132)',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                tension: 0.1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: false
                },
                legend: {
                    position: 'top',
                }
            },
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};

// Função para criar gráfico de categorias
window.criarGraficoCategorias = function(dados) {
    const ctx = document.getElementById('chartCategorias');
    if (!ctx) return;

    // Destruir gráfico existente se houver
    if (window.chartCategorias) {
        window.chartCategorias.destroy();
    }

    const labels = dados.map(d => d.categoria);
    const quantidades = dados.map(d => d.quantidade);
    
    // Cores para o gráfico
    const cores = [
        '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
        '#9966FF', '#FF9F40', '#FF6384', '#C9CBCF',
        '#4BC0C0', '#FF6384', '#36A2EB', '#FFCE56'
    ];

    window.chartCategorias = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: quantidades,
                backgroundColor: cores.slice(0, labels.length),
                borderWidth: 2,
                borderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: false
                },
                legend: {
                    position: 'right',
                }
            }
        }
    });
};

// Função para atualizar gráficos quando necessário
window.atualizarGraficosDashboard = function() {
    // Esta função pode ser chamada do Blazor para atualizar os gráficos
    if (window.chartMovimentacoes) {
        window.chartMovimentacoes.update();
    }
    if (window.chartCategorias) {
        window.chartCategorias.update();
    }
};
```

### 3. Componente de Alertas de Estoque - Components/AlertasEstoque.razor

```razor
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@inject IProdutoService ProdutoService
@inject IJSRuntime JSRuntime
@implements IDisposable

@if (mostrarAlertas && alertas?.Any() == true)
{
    <div class="position-fixed top-0 end-0 p-3" style="z-index: 1055;">
        @foreach (var alerta in alertas.Take(3))
        {
            <div class="toast show" role="alert" data-alerta-id="@alerta.Id">
                <div class="toast-header">
                    <i class="fas @GetIconeAlerta(alerta.Tipo) me-2 @GetCorAlerta(alerta.Tipo)"></i>
                    <strong class="me-auto">@GetTituloAlerta(alerta.Tipo)</strong>
                    <small class="text-muted">@alerta.DataCriacao.ToString("HH:mm")</small>
                    <button type="button" class="btn-close" @onclick="() => FecharAlerta(alerta.Id)"></button>
                </div>
                <div class="toast-body">
                    @alerta.Mensagem
                    @if (alerta.Tipo == TipoAlerta.EstoqueBaixo || alerta.Tipo == TipoAlerta.EstoqueCritico)
                    {
                        <div class="mt-2">
                            <button class="btn btn-sm btn-primary" @onclick="() => AbrirProduto(alerta.ProdutoId)">
                                <i class="fas fa-eye me-1"></i>
                                Ver Produto
                            </button>
                        </div>
                    }
                </div>
            </div>
        }
    </div>
}

@code {
    [Parameter] public bool MostrarAlertas { get; set; } = true;
    [Parameter] public int IntervaloVerificacao { get; set; } = 300000; // 5 minutos

    private bool mostrarAlertas = true;
    private List<AlertaEstoque> alertas = new();
    private Timer timer;

    protected override async Task OnInitializedAsync()
    {
        await VerificarAlertas();
        
        // Configurar timer para verificação periódica
        timer = new Timer(async _ => await VerificarAlertas(), null, IntervaloVerificacao, IntervaloVerificacao);
    }

    private async Task VerificarAlertas()
    {
        try
        {
            var novosAlertas = new List<AlertaEstoque>();
            
            // Verificar produtos com estoque baixo
            var produtosEstoqueBaixo = await ProdutoService.GetProdutosEstoqueBaixoAsync();
            foreach (var produto in produtosEstoqueBaixo.Take(5))
            {
                if (produto.EstoqueAtual <= 0)
                {
                    novosAlertas.Add(new AlertaEstoque
                    {
                        Id = $"sem-estoque-{produto.Id}",
                        Tipo = TipoAlerta.SemEstoque,
                        ProdutoId = produto.Id,
                        Mensagem = $"Produto {produto.Codigo} - {produto.Nome} está sem estoque!",
                        DataCriacao = DateTime.Now
                    });
                }
                else if (produto.EstoqueAtual <= produto.EstoqueMinimo * 0.5m)
                {
                    novosAlertas.Add(new AlertaEstoque
                    {
                        Id = $"critico-{produto.Id}",
                        Tipo = TipoAlerta.EstoqueCritico,
                        ProdutoId = produto.Id,
                        Mensagem = $"Produto {produto.Codigo} - {produto.Nome} com estoque crítico: {produto.EstoqueAtual:N2}",
                        DataCriacao = DateTime.Now
                    });
                }
                else
                {
                    novosAlertas.Add(new AlertaEstoque
                    {
                        Id = $"baixo-{produto.Id}",
                        Tipo = TipoAlerta.EstoqueBaixo,
                        ProdutoId = produto.Id,
                        Mensagem = $"Produto {produto.Codigo} - {produto.Nome} com estoque baixo: {produto.EstoqueAtual:N2}",
                        DataCriacao = DateTime.Now
                    });
                }
            }
            
            // Atualizar lista de alertas (evitar duplicatas)
            foreach (var novoAlerta in novosAlertas)
            {
                if (!alertas.Any(a => a.Id == novoAlerta.Id))
                {
                    alertas.Add(novoAlerta);
                }
            }
            
            // Remover alertas antigos (mais de 1 hora)
            alertas.RemoveAll(a => a.DataCriacao < DateTime.Now.AddHours(-1));
            
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("console.error", $"Erro ao verificar alertas: {ex.Message}");
        }
    }

    private void FecharAlerta(string alertaId)
    {
        alertas.RemoveAll(a => a.Id == alertaId);
        StateHasChanged();
    }

    private async Task AbrirProduto(int produtoId)
    {
        await JSRuntime.InvokeVoidAsync("window.open", $"/produtos/{produtoId}", "_blank");
    }

    private string GetIconeAlerta(TipoAlerta tipo)
    {
        return tipo switch
        {
            TipoAlerta.EstoqueBaixo => "fa-exclamation-triangle",
            TipoAlerta.EstoqueCritico => "fa-exclamation-circle",
            TipoAlerta.SemEstoque => "fa-times-circle",
            _ => "fa-info-circle"
        };
    }

    private string GetCorAlerta(TipoAlerta tipo)
    {
        return tipo switch
        {
            TipoAlerta.EstoqueBaixo => "text-warning",
            TipoAlerta.EstoqueCritico => "text-danger",
            TipoAlerta.SemEstoque => "text-danger",
            _ => "text-info"
        };
    }

    private string GetTituloAlerta(TipoAlerta tipo)
    {
        return tipo switch
        {
            TipoAlerta.EstoqueBaixo => "Estoque Baixo",
            TipoAlerta.EstoqueCritico => "Estoque Crítico",
            TipoAlerta.SemEstoque => "Sem Estoque",
            _ => "Alerta"
        };
    }

    public void Dispose()
    {
        timer?.Dispose();
    }

    public class AlertaEstoque
    {
        public string Id { get; set; } = string.Empty;
        public TipoAlerta Tipo { get; set; }
        public int ProdutoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }

    public enum TipoAlerta
    {
        EstoqueBaixo,
        EstoqueCritico,
        SemEstoque,
        Geral
    }
}
```

### 4. Configuração no Layout Principal - Shared/MainLayout.razor

```razor
@inherits LayoutView
@using MecTecERP.Components

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>

<!-- Componente de Alertas de Estoque -->
<AlertasEstoque MostrarAlertas="true" IntervaloVerificacao="300000" />

<!-- Scripts necessários para gráficos -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script src="~/js/dashboard-estoque.js"></script>
```

### 5. Serviço de Notificações - Services/NotificacaoService.cs

```csharp
using MecTecERP.Domain.Interfaces;
using Microsoft.JSInterop;

namespace MecTecERP.Services
{
    public interface INotificacaoService
    {
        Task MostrarSucesso(string mensagem);
        Task MostrarErro(string mensagem);
        Task MostrarAviso(string mensagem);
        Task MostrarInfo(string mensagem);
        Task MostrarToast(string titulo, string mensagem, string tipo = "info");
    }

    public class NotificacaoService : INotificacaoService
    {
        private readonly IJSRuntime _jsRuntime;

        public NotificacaoService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task MostrarSucesso(string mensagem)
        {
            await MostrarToast("Sucesso", mensagem, "success");
        }

        public async Task MostrarErro(string mensagem)
        {
            await MostrarToast("Erro", mensagem, "error");
        }

        public async Task MostrarAviso(string mensagem)
        {
            await MostrarToast("Aviso", mensagem, "warning");
        }

        public async Task MostrarInfo(string mensagem)
        {
            await MostrarToast("Informação", mensagem, "info");
        }

        public async Task MostrarToast(string titulo, string mensagem, string tipo = "info")
        {
            await _jsRuntime.InvokeVoidAsync("mostrarToast", titulo, mensagem, tipo);
        }
    }
}
```

### 6. JavaScript para Notificações - wwwroot/js/notificacoes.js

```javascript
// Função para mostrar toast notifications
window.mostrarToast = function(titulo, mensagem, tipo) {
    // Criar elemento do toast
    const toastId = 'toast-' + Date.now();
    const iconClass = getIconClass(tipo);
    const bgClass = getBgClass(tipo);
    
    const toastHtml = `
        <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header ${bgClass} text-white">
                <i class="${iconClass} me-2"></i>
                <strong class="me-auto">${titulo}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${mensagem}
            </div>
        </div>
    `;
    
    // Adicionar ao container de toasts
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container position-fixed top-0 end-0 p-3';
        container.style.zIndex = '1055';
        document.body.appendChild(container);
    }
    
    container.insertAdjacentHTML('beforeend', toastHtml);
    
    // Mostrar o toast
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: tipo === 'error' ? 8000 : 5000
    });
    
    toast.show();
    
    // Remover do DOM após ser ocultado
    toastElement.addEventListener('hidden.bs.toast', function() {
        toastElement.remove();
    });
};

function getIconClass(tipo) {
    switch(tipo) {
        case 'success': return 'fas fa-check-circle';
        case 'error': return 'fas fa-exclamation-circle';
        case 'warning': return 'fas fa-exclamation-triangle';
        case 'info': return 'fas fa-info-circle';
        default: return 'fas fa-info-circle';
    }
}

function getBgClass(tipo) {
    switch(tipo) {
        case 'success': return 'bg-success';
        case 'error': return 'bg-danger';
        case 'warning': return 'bg-warning';
        case 'info': return 'bg-info';
        default: return 'bg-info';
    }
}
```

### Próximos Passos

1. **Testes unitários** para todos os serviços e repositórios
2. **Testes de integração** para as APIs
3. **Documentação técnica** completa
4. **Manual do usuário** com screenshots
5. **Deploy e configuração** em ambiente de produção
6. **Monitoramento** e logs de sistema
7. **Backup** e recuperação de dados
8. **Integração** com outros módulos do ERP