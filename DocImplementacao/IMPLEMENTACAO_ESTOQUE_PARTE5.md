# MecTecERP - Implementação do Módulo de Estoque - Parte 5

## Páginas Blazor para Inventário

### 1. Página Principal de Inventários - Inventario/Index.razor

```razor
@page "/inventario"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Enums
@using MecTecERP.Domain.Interfaces
@inject IInventarioService InventarioService
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

<PageTitle>Inventários de Estoque</PageTitle>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h4 class="mb-0">
                        <i class="fas fa-clipboard-list me-2"></i>
                        Inventários de Estoque
                    </h4>
                    <button class="btn btn-primary" @onclick="NovoInventario">
                        <i class="fas fa-plus me-1"></i>
                        Novo Inventário
                    </button>
                </div>

                <div class="card-body">
                    <!-- Filtros -->
                    <div class="row mb-3">
                        <div class="col-md-6">
                            <div class="input-group">
                                <span class="input-group-text">
                                    <i class="fas fa-search"></i>
                                </span>
                                <input type="text" class="form-control" placeholder="Buscar inventários..."
                                       @bind="searchTerm" @onkeypress="OnSearchKeyPress" />
                            </div>
                        </div>
                        <div class="col-md-3">
                            <select class="form-select" @bind="statusFilter">
                                <option value="">Todos os Status</option>
                                <option value="@StatusInventario.Planejado">Planejado</option>
                                <option value="@StatusInventario.EmAndamento">Em Andamento</option>
                                <option value="@StatusInventario.Finalizado">Finalizado</option>
                                <option value="@StatusInventario.Cancelado">Cancelado</option>
                            </select>
                        </div>
                        <div class="col-md-3">
                            <button class="btn btn-outline-primary" @onclick="Pesquisar">
                                <i class="fas fa-search me-1"></i>
                                Pesquisar
                            </button>
                            <button class="btn btn-outline-secondary ms-2" @onclick="LimparFiltros">
                                <i class="fas fa-times me-1"></i>
                                Limpar
                            </button>
                        </div>
                    </div>

                    <!-- Loading -->
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
                        <!-- Tabela -->
                        <div class="table-responsive">
                            <table class="table table-striped table-hover">
                                <thead class="table-dark">
                                    <tr>
                                        <th>ID</th>
                                        <th>Descrição</th>
                                        <th>Data Criação</th>
                                        <th>Data Início</th>
                                        <th>Data Fim</th>
                                        <th>Status</th>
                                        <th class="text-center">Ações</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @if (inventarios?.Any() == true)
                                    {
                                        @foreach (var inventario in inventarios)
                                        {
                                            <tr>
                                                <td>@inventario.Id</td>
                                                <td>@inventario.Descricao</td>
                                                <td>@inventario.DataCriacao.ToString("dd/MM/yyyy")</td>
                                                <td>@(inventario.DataInicio?.ToString("dd/MM/yyyy") ?? "-")</td>
                                                <td>@(inventario.DataFim?.ToString("dd/MM/yyyy") ?? "-")</td>
                                                <td>
                                                    <span class="badge @GetStatusBadgeClass(inventario.Status)">
                                                        @inventario.StatusDescricao
                                                    </span>
                                                </td>
                                                <td class="text-center">
                                                    <div class="btn-group" role="group">
                                                        <button class="btn btn-sm btn-outline-primary" 
                                                                @onclick="() => VisualizarInventario(inventario.Id)"
                                                                title="Visualizar">
                                                            <i class="fas fa-eye"></i>
                                                        </button>
                                                        
                                                        @if (inventario.Status == StatusInventario.Planejado)
                                                        {
                                                            <button class="btn btn-sm btn-outline-warning" 
                                                                    @onclick="() => EditarInventario(inventario.Id)"
                                                                    title="Editar">
                                                                <i class="fas fa-edit"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-outline-success" 
                                                                    @onclick="() => IniciarInventario(inventario.Id)"
                                                                    title="Iniciar">
                                                                <i class="fas fa-play"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-outline-danger" 
                                                                    @onclick="() => ExcluirInventario(inventario.Id)"
                                                                    title="Excluir">
                                                                <i class="fas fa-trash"></i>
                                                            </button>
                                                        }
                                                        
                                                        @if (inventario.Status == StatusInventario.EmAndamento)
                                                        {
                                                            <button class="btn btn-sm btn-outline-success" 
                                                                    @onclick="() => FinalizarInventario(inventario.Id)"
                                                                    title="Finalizar">
                                                                <i class="fas fa-check"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-outline-secondary" 
                                                                    @onclick="() => CancelarInventario(inventario.Id)"
                                                                    title="Cancelar">
                                                                <i class="fas fa-times"></i>
                                                            </button>
                                                        }
                                                    </div>
                                                </td>
                                            </tr>
                                        }
                                    }
                                    else
                                    {
                                        <tr>
                                            <td colspan="7" class="text-center py-4">
                                                <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                                                <p class="text-muted mb-0">Nenhum inventário encontrado</p>
                                            </td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        </div>

                        <!-- Paginação -->
                        @if (totalPages > 1)
                        {
                            <nav aria-label="Paginação">
                                <ul class="pagination justify-content-center">
                                    <li class="page-item @(currentPage == 1 ? "disabled" : "")">
                                        <button class="page-link" @onclick="() => ChangePage(currentPage - 1)">
                                            <i class="fas fa-chevron-left"></i>
                                        </button>
                                    </li>
                                    
                                    @for (int i = Math.Max(1, currentPage - 2); i <= Math.Min(totalPages, currentPage + 2); i++)
                                    {
                                        <li class="page-item @(i == currentPage ? "active" : "")">
                                            <button class="page-link" @onclick="() => ChangePage(i)">@i</button>
                                        </li>
                                    }
                                    
                                    <li class="page-item @(currentPage == totalPages ? "disabled" : "")">
                                        <button class="page-link" @onclick="() => ChangePage(currentPage + 1)">
                                            <i class="fas fa-chevron-right"></i>
                                        </button>
                                    </li>
                                </ul>
                            </nav>
                        }
                    }
                </div>
            </div>
        </div>
    </div>
</div>

@code {
    private IEnumerable<InventarioListDto> inventarios = new List<InventarioListDto>();
    private bool loading = true;
    private string searchTerm = string.Empty;
    private string statusFilter = string.Empty;
    private int currentPage = 1;
    private int pageSize = 10;
    private int totalCount = 0;
    private int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);

    protected override async Task OnInitializedAsync()
    {
        await CarregarInventarios();
    }

    private async Task CarregarInventarios()
    {
        loading = true;
        try
        {
            StatusInventario? status = null;
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<StatusInventario>(statusFilter, out var parsedStatus))
            {
                status = parsedStatus;
            }

            var (items, total) = await InventarioService.GetPagedAsync(
                currentPage, pageSize, searchTerm, status);
            
            inventarios = items;
            totalCount = total;
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar inventários: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task Pesquisar()
    {
        currentPage = 1;
        await CarregarInventarios();
    }

    private async Task LimparFiltros()
    {
        searchTerm = string.Empty;
        statusFilter = string.Empty;
        currentPage = 1;
        await CarregarInventarios();
    }

    private async Task ChangePage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
            await CarregarInventarios();
        }
    }

    private async Task OnSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await Pesquisar();
        }
    }

    private void NovoInventario()
    {
        Navigation.NavigateTo("/inventario/novo");
    }

    private void VisualizarInventario(int id)
    {
        Navigation.NavigateTo($"/inventario/{id}");
    }

    private void EditarInventario(int id)
    {
        Navigation.NavigateTo($"/inventario/{id}/editar");
    }

    private async Task IniciarInventario(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja iniciar este inventário?"))
        {
            try
            {
                await InventarioService.IniciarInventarioAsync(id);
                await CarregarInventarios();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário iniciado com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao iniciar inventário: {ex.Message}");
            }
        }
    }

    private async Task FinalizarInventario(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja finalizar este inventário? Esta ação aplicará os ajustes no estoque."))
        {
            try
            {
                await InventarioService.FinalizarInventarioAsync(id);
                await CarregarInventarios();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário finalizado com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao finalizar inventário: {ex.Message}");
            }
        }
    }

    private async Task CancelarInventario(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja cancelar este inventário?"))
        {
            try
            {
                await InventarioService.CancelarInventarioAsync(id);
                await CarregarInventarios();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário cancelado com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao cancelar inventário: {ex.Message}");
            }
        }
    }

    private async Task ExcluirInventario(int id)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja excluir este inventário? Esta ação não pode ser desfeita."))
        {
            try
            {
                await InventarioService.DeleteAsync(id);
                await CarregarInventarios();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário excluído com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao excluir inventário: {ex.Message}");
            }
        }
    }

    private string GetStatusBadgeClass(StatusInventario status)
    {
        return status switch
        {
            StatusInventario.Planejado => "bg-secondary",
            StatusInventario.EmAndamento => "bg-warning",
            StatusInventario.Finalizado => "bg-success",
            StatusInventario.Cancelado => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
```

### 2. Página de Criação/Edição - Inventario/Form.razor

```razor
@page "/inventario/novo"
@page "/inventario/{Id:int}/editar"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Interfaces
@inject IInventarioService InventarioService
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

<PageTitle>@(Id.HasValue ? "Editar Inventário" : "Novo Inventário")</PageTitle>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="mb-0">
                        <i class="fas fa-clipboard-list me-2"></i>
                        @(Id.HasValue ? "Editar Inventário" : "Novo Inventário")
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
                                <div class="col-md-8">
                                    <div class="mb-3">
                                        <label for="descricao" class="form-label">Descrição *</label>
                                        <InputText id="descricao" class="form-control" @bind-Value="model.Descricao" />
                                        <ValidationMessage For="@(() => model.Descricao)" />
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="mb-3">
                                        <label for="dataInicio" class="form-label">Data de Início</label>
                                        <InputDate id="dataInicio" class="form-control" @bind-Value="model.DataInicio" />
                                        <ValidationMessage For="@(() => model.DataInicio)" />
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12">
                                    <div class="mb-3">
                                        <label for="observacoes" class="form-label">Observações</label>
                                        <InputTextArea id="observacoes" class="form-control" rows="4" @bind-Value="model.Observacoes" />
                                        <ValidationMessage For="@(() => model.Observacoes)" />
                                    </div>
                                </div>
                            </div>

                            <div class="d-flex justify-content-between">
                                <button type="button" class="btn btn-secondary" @onclick="Voltar">
                                    <i class="fas fa-arrow-left me-1"></i>
                                    Voltar
                                </button>
                                <button type="submit" class="btn btn-primary" disabled="@saving">
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
    [Parameter] public int? Id { get; set; }

    private InventarioFormModel model = new();
    private bool loading = true;
    private bool saving = false;

    protected override async Task OnInitializedAsync()
    {
        if (Id.HasValue)
        {
            await CarregarInventario();
        }
        else
        {
            model.DataInicio = DateTime.Today;
            model.UsuarioId = 1; // TODO: Obter do contexto de usuário
            loading = false;
        }
    }

    private async Task CarregarInventario()
    {
        try
        {
            var inventario = await InventarioService.GetByIdAsync(Id.Value);
            if (inventario != null)
            {
                model = new InventarioFormModel
                {
                    Descricao = inventario.Descricao,
                    DataInicio = inventario.DataInicio,
                    Observacoes = inventario.Observacoes
                };
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Inventário não encontrado.");
                Navigation.NavigateTo("/inventario");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar inventário: {ex.Message}");
            Navigation.NavigateTo("/inventario");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task HandleValidSubmit()
    {
        saving = true;
        try
        {
            if (Id.HasValue)
            {
                var updateDto = new InventarioUpdateDto
                {
                    Descricao = model.Descricao,
                    DataInicio = model.DataInicio,
                    Observacoes = model.Observacoes
                };

                await InventarioService.UpdateAsync(Id.Value, updateDto);
                await JSRuntime.InvokeVoidAsync("alert", "Inventário atualizado com sucesso!");
            }
            else
            {
                var createDto = new InventarioCreateDto
                {
                    Descricao = model.Descricao,
                    DataInicio = model.DataInicio,
                    UsuarioId = model.UsuarioId,
                    Observacoes = model.Observacoes
                };

                await InventarioService.CreateAsync(createDto);
                await JSRuntime.InvokeVoidAsync("alert", "Inventário criado com sucesso!");
            }

            Navigation.NavigateTo("/inventario");
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao salvar inventário: {ex.Message}");
        }
        finally
        {
            saving = false;
        }
    }

    private void Voltar()
    {
        Navigation.NavigateTo("/inventario");
    }

    public class InventarioFormModel
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(200, ErrorMessage = "Descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        public DateTime? DataInicio { get; set; }

        [StringLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Observacoes { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
    }
}
```

### 3. Página de Detalhes do Inventário - Inventario/Details.razor

```razor
@page "/inventario/{Id:int}"
@using MecTecERP.Domain.DTOs
@using MecTecERP.Domain.Enums
@using MecTecERP.Domain.Interfaces
@inject IInventarioService InventarioService
@inject IProdutoService ProdutoService
@inject NavigationManager Navigation
@inject IJSRuntime JSRuntime

<PageTitle>Detalhes do Inventário</PageTitle>

<div class="container-fluid">
    @if (loading)
    {
        <div class="text-center py-4">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Carregando...</span>
            </div>
        </div>
    }
    else if (inventario != null)
    {
        <!-- Cabeçalho -->
        <div class="row mb-4">
            <div class="col-12">
                <div class="card">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h4 class="mb-0">
                            <i class="fas fa-clipboard-list me-2"></i>
                            Inventário #@inventario.Id - @inventario.Descricao
                        </h4>
                        <div>
                            <span class="badge @GetStatusBadgeClass(inventario.Status) fs-6">
                                @inventario.Status.ToString()
                            </span>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-3">
                                <strong>Data de Criação:</strong><br>
                                @inventario.DataCriacao.ToString("dd/MM/yyyy HH:mm")
                            </div>
                            <div class="col-md-3">
                                <strong>Data de Início:</strong><br>
                                @(inventario.DataInicio?.ToString("dd/MM/yyyy") ?? "Não definida")
                            </div>
                            <div class="col-md-3">
                                <strong>Data de Fim:</strong><br>
                                @(inventario.DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "Em andamento")
                            </div>
                            <div class="col-md-3">
                                <strong>Total de Itens:</strong><br>
                                @resumo?.TotalItens ?? 0
                            </div>
                        </div>
                        @if (!string.IsNullOrEmpty(inventario.Observacoes))
                        {
                            <div class="row mt-3">
                                <div class="col-12">
                                    <strong>Observações:</strong><br>
                                    @inventario.Observacoes
                                </div>
                            </div>
                        }
                    </div>
                </div>
            </div>
        </div>

        <!-- Ações -->
        <div class="row mb-3">
            <div class="col-12">
                <div class="d-flex justify-content-between align-items-center">
                    <button class="btn btn-secondary" @onclick="Voltar">
                        <i class="fas fa-arrow-left me-1"></i>
                        Voltar
                    </button>
                    
                    <div>
                        @if (inventario.Status == StatusInventario.EmAndamento)
                        {
                            <button class="btn btn-success me-2" @onclick="AdicionarItem">
                                <i class="fas fa-plus me-1"></i>
                                Adicionar Item
                            </button>
                        }
                        
                        @if (inventario.Status == StatusInventario.Planejado)
                        {
                            <button class="btn btn-warning me-2" @onclick="EditarInventario">
                                <i class="fas fa-edit me-1"></i>
                                Editar
                            </button>
                            <button class="btn btn-success me-2" @onclick="IniciarInventario">
                                <i class="fas fa-play me-1"></i>
                                Iniciar
                            </button>
                        }
                        
                        @if (inventario.Status == StatusInventario.EmAndamento)
                        {
                            <button class="btn btn-primary me-2" @onclick="FinalizarInventario">
                                <i class="fas fa-check me-1"></i>
                                Finalizar
                            </button>
                        }
                    </div>
                </div>
            </div>
        </div>

        <!-- Lista de Itens -->
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0">
                            <i class="fas fa-list me-2"></i>
                            Itens do Inventário
                        </h5>
                    </div>
                    <div class="card-body">
                        @if (loadingItens)
                        {
                            <div class="text-center py-4">
                                <div class="spinner-border" role="status">
                                    <span class="visually-hidden">Carregando itens...</span>
                                </div>
                            </div>
                        }
                        else
                        {
                            <div class="table-responsive">
                                <table class="table table-striped table-hover">
                                    <thead class="table-dark">
                                        <tr>
                                            <th>Código</th>
                                            <th>Produto</th>
                                            <th class="text-end">Estoque Sistema</th>
                                            <th class="text-end">Estoque Contado</th>
                                            <th class="text-end">Diferença</th>
                                            <th>Observações</th>
                                            @if (inventario.Status == StatusInventario.EmAndamento)
                                            {
                                                <th class="text-center">Ações</th>
                                            }
                                        </tr>
                                    </thead>
                                    <tbody>
                                        @if (itens?.Any() == true)
                                        {
                                            @foreach (var item in itens)
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
                                                    @if (inventario.Status == StatusInventario.EmAndamento)
                                                    {
                                                        <td class="text-center">
                                                            <button class="btn btn-sm btn-outline-warning me-1" 
                                                                    @onclick="() => EditarItem(item.Id)"
                                                                    title="Editar">
                                                                <i class="fas fa-edit"></i>
                                                            </button>
                                                            <button class="btn btn-sm btn-outline-danger" 
                                                                    @onclick="() => RemoverItem(item.Id)"
                                                                    title="Remover">
                                                                <i class="fas fa-trash"></i>
                                                            </button>
                                                        </td>
                                                    }
                                                </tr>
                                            }
                                        }
                                        else
                                        {
                                            <tr>
                                                <td colspan="@(inventario.Status == StatusInventario.EmAndamento ? "7" : "6")" class="text-center py-4">
                                                    <i class="fas fa-inbox fa-2x text-muted mb-2"></i>
                                                    <p class="text-muted mb-0">Nenhum item adicionado ao inventário</p>
                                                </td>
                                            </tr>
                                        }
                                    </tbody>
                                </table>
                            </div>
                        }
                    </div>
                </div>
            </div>
        </div>
    }
    else
    {
        <div class="alert alert-warning">
            <i class="fas fa-exclamation-triangle me-2"></i>
            Inventário não encontrado.
        </div>
    }
</div>

@code {
    [Parameter] public int Id { get; set; }

    private InventarioDto inventario;
    private InventarioResumoDto resumo;
    private IEnumerable<InventarioItemDto> itens = new List<InventarioItemDto>();
    private bool loading = true;
    private bool loadingItens = false;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        loading = true;
        try
        {
            inventario = await InventarioService.GetByIdAsync(Id);
            if (inventario != null)
            {
                resumo = await InventarioService.GetResumoAsync(Id);
                await CarregarItens();
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar inventário: {ex.Message}");
        }
        finally
        {
            loading = false;
        }
    }

    private async Task CarregarItens()
    {
        loadingItens = true;
        try
        {
            itens = await InventarioService.GetItensAsync(Id);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Erro ao carregar itens: {ex.Message}");
        }
        finally
        {
            loadingItens = false;
        }
    }

    private void Voltar()
    {
        Navigation.NavigateTo("/inventario");
    }

    private void EditarInventario()
    {
        Navigation.NavigateTo($"/inventario/{Id}/editar");
    }

    private void AdicionarItem()
    {
        Navigation.NavigateTo($"/inventario/{Id}/item/novo");
    }

    private void EditarItem(int itemId)
    {
        Navigation.NavigateTo($"/inventario/{Id}/item/{itemId}/editar");
    }

    private async Task IniciarInventario()
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja iniciar este inventário?"))
        {
            try
            {
                await InventarioService.IniciarInventarioAsync(Id);
                await CarregarDados();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário iniciado com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao iniciar inventário: {ex.Message}");
            }
        }
    }

    private async Task FinalizarInventario()
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja finalizar este inventário? Esta ação aplicará os ajustes no estoque."))
        {
            try
            {
                await InventarioService.FinalizarInventarioAsync(Id);
                await CarregarDados();
                await JSRuntime.InvokeVoidAsync("alert", "Inventário finalizado com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao finalizar inventário: {ex.Message}");
            }
        }
    }

    private async Task RemoverItem(int itemId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Deseja remover este item do inventário?"))
        {
            try
            {
                await InventarioService.RemoveItemAsync(itemId);
                await CarregarItens();
                await JSRuntime.InvokeVoidAsync("alert", "Item removido com sucesso!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Erro ao remover item: {ex.Message}");
            }
        }
    }

    private string GetStatusBadgeClass(StatusInventario status)
    {
        return status switch
        {
            StatusInventario.Planejado => "bg-secondary",
            StatusInventario.EmAndamento => "bg-warning",
            StatusInventario.Finalizado => "bg-success",
            StatusInventario.Cancelado => "bg-danger",
            _ => "bg-secondary"
        };
    }
}
```

### Próximos Passos

1. **Página de adição/edição de itens** do inventário
2. **Componentes auxiliares** (seletor de produtos, etc.)
3. **Relatórios de inventário** e divergências
4. **Dashboard de estoque** com alertas
5. **Testes e validações** finais