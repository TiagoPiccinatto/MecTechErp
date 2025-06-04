# MecTecERP - Implementação do Módulo de Estoque - Parte 6

## Páginas e Componentes Complementares

### 1. Página de Adição/Edição de Item do Inventário - Inventario/ItemForm.razor

```razor
@page "/inventario/{InventarioId:int}/item/novo"
@page "/inventario/{InventarioId:int}/item/{ItemId:int}/editar"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@inject IInventarioService InventarioService
@inject IProdutoService ProdutoService
@inject IEstoqueService EstoqueService
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

<PageTitle>@(ItemId.HasValue ? "Editar Item" : "Adicionar Item")</PageTitle>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="mb-0">
                        <i class="fas fa-box me-2"></i>
                        @(ItemId.HasValue ? "Editar Item do Inventário" : "Adicionar Item ao Inventário")
                    </h4>
                </div>

                <div class="card-body">
                    @if (loading)
                    {
                        <div class="text-center py-4">
                            <div class="spinner-border" role="status">
                                <span class="visually-hidden">Carregando...</span>
                            </div>
                        </div>
                    }
                    else
                    {
                        <EditForm Model="@model" OnValidSubmit="@HandleValidSubmit">
                            <DataAnnotationsValidator />
                            <ValidationSummary class="alert alert-danger" />

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="mb-3">
                                        <label for="produto" class="form-label">Produto *</label>
                                        @if (ItemId.HasValue)
                                        {
                                            <input type="text" class="form-control" value="@produtoSelecionado?.Nome" readonly />
                                        }
                                        else
                                        {
                                            <div class="input-group">
                                                <input type="text" class="form-control" placeholder="Digite para buscar produto..."
                                                       @bind="termoBusca" @oninput="OnBuscaProdutoChanged" />
                                                <button class="btn btn-outline-secondary" type="button" @onclick="AbrirSeletorProduto">
                                                    <i class="fas fa-search"></i>
                                                </button>
                                            </div>
                                            @if (produtosSugeridos?.Any() == true)
                                            {
                                                <div class="dropdown-menu show w-100 mt-1" style="max-height: 200px; overflow-y: auto;">
                                                    @foreach (var produto in produtosSugeridos)
                                                    {
                                                        <button type="button" class="dropdown-item" @onclick="() => SelecionarProduto(produto)">
                                                            <strong>@produto.Codigo</strong> - @produto.Nome
                                                            <br><small class="text-muted">Estoque: @produto.EstoqueAtual</small>
                                                        </button>
                                                    }
                                                </div>
                                            }
                                        }
                                        <ValidationMessage For="@(() => model.ProdutoId)" />
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="mb-3">
                                        <label for="estoqueSistema" class="form-label">Estoque Sistema</label>
                                        <input type="number" id="estoqueSistema" class="form-control" 
                                               value="@model.EstoqueSistema" readonly />
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="mb-3">
                                        <label for="estoqueContado" class="form-label">Estoque Contado *</label>
                                        <InputNumber id="estoqueContado" class="form-control" 
                                                   @bind-Value="model.EstoqueContado" 
                                                   @oninput="CalcularDiferenca" />
                                        <ValidationMessage For="@(() => model.EstoqueContado)" />
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-3">
                                    <div class="mb-3">
                                        <label for="diferenca" class="form-label">Diferença</label>
                                        <input type="number" id="diferenca" class="form-control" 
                                               value="@diferenca.ToString("+0.00;-0.00;0.00")" readonly
                                               class="@(diferenca > 0 ? "text-success" : diferenca < 0 ? "text-danger" : "")" />
                                    </div>
                                </div>
                                <div class="col-md-9">
                                    <div class="mb-3">
                                        <label for="observacoes" class="form-label">Observações</label>
                                        <InputTextArea id="observacoes" class="form-control" rows="3" @bind-Value="model.Observacoes" />
                                        <ValidationMessage For="@(() => model.Observacoes)" />
                                    </div>
                                </div>
                            </div>

                            @if (produtoSelecionado != null)
                            {
                                <div class="alert alert-info">
                                    <h6><i class="fas fa-info-circle me-2"></i>Informações do Produto</h6>
                                    <div class="row">
                                        <div class="col-md-3">
                                            <strong>Código:</strong> @produtoSelecionado.Codigo
                                        </div>
                                        <div class="col-md-6">
                                            <strong>Nome:</strong> @produtoSelecionado.Nome
                                        </div>
                                        <div class="col-md-3">
                                            <strong>Estoque Atual:</strong> @produtoSelecionado.EstoqueAtual.ToString("N2")
                                        </div>
                                    </div>
                                </div>
                            }

                            <div class="d-flex justify-content-between">
                                <button type="button" class="btn btn-secondary" @onclick="Voltar">
                                    <i class="fas fa-arrow-left me-1"></i>
                                    Voltar
                                </button>
                                <button type="submit" class="btn btn-primary" disabled="@(saving || produtoSelecionado == null)">
                                    @if (saving)
                                    {
                                        <span class="spinner-border spinner-border-sm me-1" role="status"></span>
                                    }
                                    else
                                    {
                                        <i class="fas fa-save me-1"></i>
                                    }
                                    Salvar
                                </button>
                            </div>
                        </EditForm>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    [Parameter] public int InventarioId { get; set; }
    [Parameter] public int? ItemId { get; set; }

    private InventarioItemFormModel model = new();
    private ProdutoListDto produtoSelecionado;
    private IEnumerable<ProdutoListDto> produtosSugeridos = new List<ProdutoListDto>();
    private string termoBusca = string.Empty;
    private decimal diferenca = 0;
    private bool loading = true;
    private bool saving = false;

    protected override async Task OnInitializedAsync()
    {
        if (ItemId.HasValue)
        {
            await CarregarItem();
        }
        else
        {
            model.InventarioId = InventarioId;
            loading = false;
        }
    }

    private async Task CarregarItem()
    {
        try
        {
            var itens = await InventarioService.GetItensAsync(InventarioId);
            var item = itens.FirstOrDefault(i => i.Id == ItemId.Value);
            
            if (item != null)
            {
                model = new InventarioItemFormModel
                {
                    InventarioId = InventarioId,
                    ProdutoId = item.ProdutoId,
                    EstoqueSistema = item.EstoqueSistema,
                    EstoqueContado = item.EstoqueContado,
                    Observacoes = item.Observacoes
                };

                // Carregar dados do produto
                var produto = await ProdutoService.GetByIdAsync(item.ProdutoId);
                if (produto != null)
                {
                    produtoSelecionado = new ProdutoListDto
                    {
                        Id = produto.Id,
                        Codigo = produto.Codigo,
                        Nome = produto.Nome,
                        EstoqueAtual = produto.EstoqueAtual
                    };
                }

                CalcularDiferenca();
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Item não encontrado.");
                Navigation.NavigateTo($"/inventario/{InventarioId}");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar item: {ex.Message}");
            Navigation.NavigateTo($"/inventario/{InventarioId}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task OnBuscaProdutoChanged(ChangeEventArgs e)
    {
        termoBusca = e.Value?.ToString() ?? string.Empty;
        
        if (termoBusca.Length >= 2)
        {
            try
            {
                var (produtos, _) = await ProdutoService.GetPagedAsync(1, 10, termoBusca);
                produtosSugeridos = produtos;
            }
            catch
            {
                produtosSugeridos = new List<ProdutoListDto>();
            }
        }
        else
        {
            produtosSugeridos = new List<ProdutoListDto>();
        }
    }

    private async Task SelecionarProduto(ProdutoListDto produto)
    {
        produtoSelecionado = produto;
        model.ProdutoId = produto.Id;
        model.EstoqueSistema = produto.EstoqueAtual;
        termoBusca = $"{produto.Codigo} - {produto.Nome}";
        produtosSugeridos = new List<ProdutoListDto>();
        
        CalcularDiferenca();
        StateHasChanged();
    }

    private void AbrirSeletorProduto()
    {
        // TODO: Implementar modal de seleção de produto
    }

    private void CalcularDiferenca()
    {
        diferenca = model.EstoqueContado - model.EstoqueSistema;
    }

    private async Task HandleValidSubmit()
    {
        saving = true;
        try
        {
            if (ItemId.HasValue)
            {
                var updateDto = new InventarioItemUpdateDto
                {
                    EstoqueSistema = model.EstoqueSistema,
                    EstoqueContado = model.EstoqueContado,
                    Observacoes = model.Observacoes
                };

                await InventarioService.UpdateItemAsync(ItemId.Value, updateDto);
                await JSRuntime.InvokeVoidAsync("alert", "Item atualizado com sucesso!");
            }
            else
            {
                var createDto = new InventarioItemCreateDto
                {
                    InventarioId = model.InventarioId,
                    ProdutoId = model.ProdutoId,
                    EstoqueContado = model.EstoqueContado,
                    Observacoes = model.Observacoes
                };

                await InventarioService.AddItemAsync(createDto);
                await JSRuntime.InvokeVoidAsync("alert", "Item adicionado com sucesso!");
            }

            Navigation.NavigateTo($"/inventario/{InventarioId}");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao salvar item: {ex.Message}");
        }
        finally
        {
            saving = false;
        }
    }

    private void Voltar()
    {
        Navigation.NavigateTo($"/inventario/{InventarioId}");
    }

    public class InventarioItemFormModel
    {
        public int InventarioId { get; set; }
        
        [Required(ErrorMessage = "Produto é obrigatório")]
        public int ProdutoId { get; set; }
        
        public decimal EstoqueSistema { get; set; }
        
        [Required(ErrorMessage = "Estoque contado é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "Estoque contado deve ser maior ou igual a zero")]
        public decimal EstoqueContado { get; set; }
        
        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes { get; set; } = string.Empty;
    }
}
```

### 2. Componente de Seleção de Produto - Components/ProdutoSelector.razor

```razor
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@inject IProdutoService ProdutoService
@inject IJSRuntime JSRuntime

<div class="produto-selector">
    <div class="input-group">
        <input type="text" class="form-control" placeholder="@Placeholder"
               @bind="termoBusca" @oninput="OnBuscaChanged" @onfocus="OnFocus" />
        <button class="btn btn-outline-secondary" type="button" @onclick="AbrirModal">
            <i class="fas fa-search"></i>
        </button>
        @if (ProdutoSelecionado != null)
        {
            <button class="btn btn-outline-danger" type="button" @onclick="LimparSelecao">
                <i class="fas fa-times"></i>
            </button>
        }
    </div>
    
    @if (mostrarSugestoes && produtosSugeridos?.Any() == true)
    {
        <div class="dropdown-menu show w-100 mt-1" style="max-height: 200px; overflow-y: auto; z-index: 1050;">
            @foreach (var produto in produtosSugeridos)
            {
                <button type="button" class="dropdown-item" @onclick="() => SelecionarProduto(produto)">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <strong>@produto.Codigo</strong> - @produto.Nome
                            <br><small class="text-muted">@produto.CategoriaNome</small>
                        </div>
                        <div class="text-end">
                            <small class="text-muted">Estoque: @produto.EstoqueAtual.ToString("N2")</small>
                        </div>
                    </div>
                </button>
            }
        </div>
    }
    
    @if (ProdutoSelecionado != null)
    {
        <div class="alert alert-info mt-2 mb-0">
            <div class="row">
                <div class="col-md-3">
                    <strong>Código:</strong> @ProdutoSelecionado.Codigo
                </div>
                <div class="col-md-6">
                    <strong>Nome:</strong> @ProdutoSelecionado.Nome
                </div>
                <div class="col-md-3">
                    <strong>Estoque:</strong> @ProdutoSelecionado.EstoqueAtual.ToString("N2")
                </div>
            </div>
        </div>
    }
</div>

<!-- Modal de Seleção -->
<div class="modal fade" id="modalProdutoSelector" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <i class="fas fa-search me-2"></i>
                    Selecionar Produto
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="row mb-3">
                    <div class="col-12">
                        <input type="text" class="form-control" placeholder="Buscar produtos..."
                               @bind="buscaModal" @oninput="OnBuscaModalChanged" />
                    </div>
                </div>
                
                @if (carregandoModal)
                {
                    <div class="text-center py-4">
                        <div class="spinner-border" role="status">
                            <span class="visually-hidden">Carregando...</span>
                        </div>
                    </div>
                }
                else
                {
                    <div class="table-responsive" style="max-height: 400px; overflow-y: auto;">
                        <table class="table table-striped table-hover">
                            <thead class="table-dark sticky-top">
                                <tr>
                                    <th>Código</th>
                                    <th>Nome</th>
                                    <th>Categoria</th>
                                    <th class="text-end">Estoque</th>
                                    <th class="text-center">Ação</th>
                                </tr>
                            </thead>
                            <tbody>
                                @if (produtosModal?.Any() == true)
                                {
                                    @foreach (var produto in produtosModal)
                                    {
                                        <tr>
                                            <td>@produto.Codigo</td>
                                            <td>@produto.Nome</td>
                                            <td>@produto.CategoriaNome</td>
                                            <td class="text-end">@produto.EstoqueAtual.ToString("N2")</td>
                                            <td class="text-center">
                                                <button class="btn btn-sm btn-primary" 
                                                        @onclick="() => SelecionarProdutoModal(produto)">
                                                    <i class="fas fa-check"></i>
                                                </button>
                                            </td>
                                        </tr>
                                    }
                                }
                                else
                                {
                                    <tr>
                                        <td colspan="5" class="text-center py-4">
                                            <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                                            <p class="text-muted mb-0">Nenhum produto encontrado</p>
                                        </td>
                                    }
                                }
                            </tbody>
                        </table>
                    </div>
                }
            </div>
        </div>
    </div>
</div>

@code {
    [Parameter] public ProdutoListDto ProdutoSelecionado { get; set; }
    [Parameter] public EventCallback<ProdutoListDto> ProdutoSelecionadoChanged { get; set; }
    [Parameter] public string Placeholder { get; set; } = "Digite para buscar produto...";
    [Parameter] public bool ApenasComEstoque { get; set; } = false;

    private string termoBusca = string.Empty;
    private string buscaModal = string.Empty;
    private bool mostrarSugestoes = false;
    private bool carregandoModal = false;
    private IEnumerable<ProdutoListDto> produtosSugeridos = new List<ProdutoListDto>();
    private IEnumerable<ProdutoListDto> produtosModal = new List<ProdutoListDto>();

    protected override void OnParametersSet()
    {
        if (ProdutoSelecionado != null)
        {
            termoBusca = $"{ProdutoSelecionado.Codigo} - {ProdutoSelecionado.Nome}";
        }
    }

    private async Task OnBuscaChanged(ChangeEventArgs e)
    {
        termoBusca = e.Value?.ToString() ?? string.Empty;
        
        if (termoBusca.Length >= 2)
        {
            await BuscarProdutos(termoBusca);
            mostrarSugestoes = true;
        }
        else
        {
            produtosSugeridos = new List<ProdutoListDto>();
            mostrarSugestoes = false;
        }
    }

    private async Task OnBuscaModalChanged(ChangeEventArgs e)
    {
        buscaModal = e.Value?.ToString() ?? string.Empty;
        await BuscarProdutosModal();
    }

    private void OnFocus()
    {
        if (produtosSugeridos?.Any() == true)
        {
            mostrarSugestoes = true;
        }
    }

    private async Task BuscarProdutos(string termo)
    {
        try
        {
            var (produtos, _) = await ProdutoService.GetPagedAsync(1, 10, termo);
            
            if (ApenasComEstoque)
            {
                produtos = produtos.Where(p => p.EstoqueAtual > 0);
            }
            
            produtosSugeridos = produtos;
        }
        catch
        {
            produtosSugeridos = new List<ProdutoListDto>();
        }
    }

    private async Task BuscarProdutosModal()
    {
        carregandoModal = true;
        try
        {
            var (produtos, _) = await ProdutoService.GetPagedAsync(1, 50, buscaModal);
            
            if (ApenasComEstoque)
            {
                produtos = produtos.Where(p => p.EstoqueAtual > 0);
            }
            
            produtosModal = produtos;
        }
        catch
        {
            produtosModal = new List<ProdutoListDto>();
        }
        finally
        {
            carregandoModal = false;
        }
    }

    private async Task SelecionarProduto(ProdutoListDto produto)
    {
        ProdutoSelecionado = produto;
        termoBusca = $"{produto.Codigo} - {produto.Nome}";
        mostrarSugestoes = false;
        await ProdutoSelecionadoChanged.InvokeAsync(produto);
    }

    private async Task SelecionarProdutoModal(ProdutoListDto produto)
    {
        await SelecionarProduto(produto);
        await JSRuntime.InvokeVoidAsync("bootstrap.Modal.getInstance", "#modalProdutoSelector")?.InvokeVoidAsync("hide");
    }

    private async Task AbrirModal()
    {
        buscaModal = string.Empty;
        await BuscarProdutosModal();
        await JSRuntime.InvokeVoidAsync("new bootstrap.Modal", "#modalProdutoSelector")?.InvokeVoidAsync("show");
    }

    private async Task LimparSelecao()
    {
        ProdutoSelecionado = null;
        termoBusca = string.Empty;
        mostrarSugestoes = false;
        await ProdutoSelecionadoChanged.InvokeAsync(null);
    }
}

<style>
    .produto-selector {
        position: relative;
    }
    
    .produto-selector .dropdown-menu {
        position: absolute;
        top: 100%;
        left: 0;
        right: 0;
    }
</style>
```

### 3. Página de Relatórios de Estoque - Relatorios/Estoque.razor

```razor
@page "/relatorios/estoque"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@inject IProdutoService ProdutoService
@inject IInventarioService InventarioService
@inject IJSRuntime JSRuntime

<PageTitle>Relatórios de Estoque</PageTitle>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="mb-0">
                        <i class="fas fa-chart-bar me-2"></i>
                        Relatórios de Estoque
                    </h4>
                </div>

                <div class="card-body">
                    <!-- Filtros -->
                    <div class="row mb-4">
                        <div class="col-md-3">
                            <label for="tipoRelatorio" class="form-label">Tipo de Relatório</label>
                            <select id="tipoRelatorio" class="form-select" @bind="tipoRelatorio">
                                <option value="posicao">Posição de Estoque</option>
                                <option value="baixo">Produtos com Estoque Baixo</option>
                                <option value="critico">Produtos com Estoque Crítico</option>
                                <option value="zerado">Produtos sem Estoque</option>
                                <option value="inventario">Relatório de Inventário</option>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <label for="categoria" class="form-label">Categoria</label>
                            <select id="categoria" class="form-select" @bind="categoriaFiltro">
                                <option value="">Todas as Categorias</option>
                                @if (categorias?.Any() == true)
                                {
                                    @foreach (var categoria in categorias)
                                    {
                                        <option value="@categoria.Id">@categoria.Nome</option>
                                    }
                                }
                            </select>
                        </div>
                        <div class="col-md-3">
                            <label for="inventarioId" class="form-label">Inventário</label>
                            <select id="inventarioId" class="form-select" @bind="inventarioId" disabled="@(tipoRelatorio != "inventario")">
                                <option value="">Selecione um inventário</option>
                                @if (inventarios?.Any() == true)
                                {
                                    @foreach (var inventario in inventarios)
                                    {
                                        <option value="@inventario.Id">@inventario.Descricao</option>
                                    }
                                }
                            </select>
                        </div>
                        <div class="col-md-3 d-flex align-items-end">
                            <button class="btn btn-primary me-2" @onclick="GerarRelatorio">
                                <i class="fas fa-search me-1"></i>
                                Gerar Relatório
                            </button>
                            <button class="btn btn-success" @onclick="ExportarExcel" disabled="@(!dadosCarregados)">
                                <i class="fas fa-file-excel me-1"></i>
                                Excel
                            </button>
                        </div>
                    </div>

                    <!-- Loading -->
                    @if (loading)
                    {
                        <div class="text-center py-4">
                            <div class="spinner-border" role="status">
                                <span class="visually-hidden">Gerando relatório...</span>
                            </div>
                        </div>
                    }
                    else if (dadosCarregados)
                    {
                        <!-- Resumo -->
                        <div class="row mb-4">
                            <div class="col-md-3">
                                <div class="card bg-primary text-white">
                                    <div class="card-body text-center">
                                        <h5 class="card-title">@totalProdutos</h5>
                                        <p class="card-text">Total de Produtos</p>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-success text-white">
                                    <div class="card-body text-center">
                                        <h5 class="card-title">@valorTotalEstoque.ToString("C")</h5>
                                        <p class="card-text">Valor Total do Estoque</p>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-warning text-white">
                                    <div class="card-body text-center">
                                        <h5 class="card-title">@produtosEstoqueBaixo</h5>
                                        <p class="card-text">Estoque Baixo</p>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="card bg-danger text-white">
                                    <div class="card-body text-center">
                                        <h5 class="card-title">@produtosSemEstoque</h5>
                                        <p class="card-text">Sem Estoque</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Tabela de Dados -->
                        <div class="table-responsive">
                            @if (tipoRelatorio == "inventario" && itensInventario?.Any() == true)
                            {
                                <table class="table table-striped table-hover">
                                    <thead class="table-dark">
                                        <tr>
                                            <th>Código</th>
                                            <th>Produto</th>
                                            <th class="text-end">Estoque Sistema</th>
                                            <th class="text-end">Estoque Contado</th>
                                            <th class="text-end">Diferença</th>
                                            <th>Observações</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @foreach (var item in itensInventario)
                                        {
                                            <tr class="@(item.Diferenca != 0 ? "table-warning" : "")">
                                                <td>@item.ProdutoCodigo</td>
                                                <td>@item.ProdutoNome</td>
                                                <td class="text-end">@item.EstoqueSistema.ToString("N2")</td>
                                                <td class="text-end">@item.EstoqueContado.ToString("N2")</td>
                                                <td class="text-end">
                                                    <span class="@(item.Diferenca > 0 ? "text-success" : item.Diferenca < 0 ? "text-danger" : "")">
                                                        @item.Diferenca.ToString("+0.00;-0.00;0.00")
                                                    </span>
                                                </td>
                                                <td>@item.Observacoes</td>
                                            </tr>
                                        }
                                    </tbody>
                                </table>
                            }
                            else if (produtos?.Any() == true)
                            {
                                <table class="table table-striped table-hover">
                                    <thead class="table-dark">
                                        <tr>
                                            <th>Código</th>
                                            <th>Nome</th>
                                            <th>Categoria</th>
                                            <th class="text-end">Estoque Atual</th>
                                            <th class="text-end">Estoque Mínimo</th>
                                            <th class="text-end">Preço Custo</th>
                                            <th class="text-end">Valor Total</th>
                                            <th class="text-center">Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @foreach (var produto in produtos)
                                        {
                                            <tr>
                                                <td>@produto.Codigo</td>
                                                <td>@produto.Nome</td>
                                                <td>@produto.CategoriaNome</td>
                                                <td class="text-end">@produto.EstoqueAtual.ToString("N2")</td>
                                                <td class="text-end">@produto.EstoqueMinimo.ToString("N2")</td>
                                                <td class="text-end">@produto.PrecoCusto.ToString("C")</td>
                                                <td class="text-end">@((produto.EstoqueAtual * produto.PrecoCusto).ToString("C"))</td>
                                                <td class="text-center">
                                                    @if (produto.EstoqueAtual <= 0)
                                                    {
                                                        <span class="badge bg-danger">Sem Estoque</span>
                                                    }
                                                    else if (produto.EstoqueAtual <= produto.EstoqueMinimo)
                                                    {
                                                        <span class="badge bg-warning">Estoque Baixo</span>
                                                    }
                                                    else
                                                    {
                                                        <span class="badge bg-success">Normal</span>
                                                    }
                                                </td>
                                            </tr>
                                        }
                                    </tbody>
                                </table>
                            }
                            else
                            {
                                <div class="text-center py-4">
                                    <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                                    <p class="text-muted mb-0">Nenhum dado encontrado para os filtros selecionados</p>
                                </div>
                            }
                        </div>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private string tipoRelatorio = "posicao";
    private string categoriaFiltro = string.Empty;
    private string inventarioId = string.Empty;
    private bool loading = false;
    private bool dadosCarregados = false;

    private IEnumerable<ProdutoListDto> produtos = new List<ProdutoListDto>();
    private IEnumerable<CategoriaListDto> categorias = new List<CategoriaListDto>();
    private IEnumerable<InventarioListDto> inventarios = new List<InventarioListDto>();
    private IEnumerable<InventarioItemDto> itensInventario = new List<InventarioItemDto>();

    private int totalProdutos = 0;
    private decimal valorTotalEstoque = 0;
    private int produtosEstoqueBaixo = 0;
    private int produtosSemEstoque = 0;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDadosIniciais();
    }

    private async Task CarregarDadosIniciais()
    {
        try
        {
            // Carregar categorias
            categorias = await CategoriaService.GetAllAsync();
            
            // Carregar inventários finalizados
            var (inventariosData, _) = await InventarioService.GetPagedAsync(1, 100, null, StatusInventario.Finalizado);
            inventarios = inventariosData;
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar dados iniciais: {ex.Message}");
        }
    }

    private async Task GerarRelatorio()
    {
        loading = true;
        dadosCarregados = false;
        
        try
        {
            switch (tipoRelatorio)
            {
                case "posicao":
                    await GerarRelatorioPosicao();
                    break;
                case "baixo":
                    await GerarRelatorioEstoqueBaixo();
                    break;
                case "critico":
                    await GerarRelatorioEstoqueCritico();
                    break;
                case "zerado":
                    await GerarRelatorioSemEstoque();
                    break;
                case "inventario":
                    await GerarRelatorioInventario();
                    break;
            }
            
            dadosCarregados = true;
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao gerar relatório: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task GerarRelatorioPosicao()
    {
        var (produtosData, total) = await ProdutoService.GetPagedAsync(1, 1000, null, categoriaFiltro);
        produtos = produtosData;
        
        CalcularResumo();
    }

    private async Task GerarRelatorioEstoqueBaixo()
    {
        produtos = await ProdutoService.GetProdutosEstoqueBaixoAsync();
        if (!string.IsNullOrEmpty(categoriaFiltro) && int.TryParse(categoriaFiltro, out var catId))
        {
            produtos = produtos.Where(p => p.CategoriaId == catId);
        }
        
        CalcularResumo();
    }

    private async Task GerarRelatorioEstoqueCritico()
    {
        produtos = await ProdutoService.GetProdutosEstoqueCriticoAsync();
        if (!string.IsNullOrEmpty(categoriaFiltro) && int.TryParse(categoriaFiltro, out var catId))
        {
            produtos = produtos.Where(p => p.CategoriaId == catId);
        }
        
        CalcularResumo();
    }

    private async Task GerarRelatorioSemEstoque()
    {
        produtos = await ProdutoService.GetProdutosSemEstoqueAsync();
        if (!string.IsNullOrEmpty(categoriaFiltro) && int.TryParse(categoriaFiltro, out var catId))
        {
            produtos = produtos.Where(p => p.CategoriaId == catId);
        }
        
        CalcularResumo();
    }

    private async Task GerarRelatorioInventario()
    {
        if (string.IsNullOrEmpty(inventarioId) || !int.TryParse(inventarioId, out var invId))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Selecione um inventário para gerar o relatório.");
            return;
        }
        
        itensInventario = await InventarioService.GetItensAsync(invId);
        
        // Calcular resumo específico do inventário
        totalProdutos = itensInventario.Count();
        valorTotalEstoque = itensInventario.Sum(i => i.EstoqueContado * 0); // TODO: Incluir preço do produto
        produtosEstoqueBaixo = itensInventario.Count(i => i.Diferenca < 0);
        produtosSemEstoque = itensInventario.Count(i => i.EstoqueContado == 0);
    }

    private void CalcularResumo()
    {
        totalProdutos = produtos.Count();
        valorTotalEstoque = produtos.Sum(p => p.EstoqueAtual * p.PrecoCusto);
        produtosEstoqueBaixo = produtos.Count(p => p.EstoqueAtual > 0 && p.EstoqueAtual <= p.EstoqueMinimo);
        produtosSemEstoque = produtos.Count(p => p.EstoqueAtual <= 0);
    }

    private async Task ExportarExcel()
    {
        // TODO: Implementar exportação para Excel
        await JSRuntime.InvokeVoidAsync("alert", "Funcionalidade de exportação será implementada em breve.");
    }
}
```

### Próximos Passos

1. **Dashboard de estoque** com gráficos e indicadores
2. **Sistema de alertas** para estoque baixo/crítico
3. **Integração com sistema de compras** para reposição automática
4. **Relatórios avançados** com gráficos e análises
5. **Testes unitários** e de integração
6. **Documentação** completa do módulo