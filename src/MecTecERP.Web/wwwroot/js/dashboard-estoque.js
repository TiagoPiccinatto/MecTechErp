// MecTecERP - Dashboard de Estoque

window.DashboardEstoque = {
    // Configurações dos gráficos
    chartConfigs: {
        // Configuração padrão para gráficos
        defaultOptions: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: function(context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            if (context.parsed.y !== null) {
                                if (context.dataset.type === 'currency') {
                                    label += MecTecERP.utils.formatCurrency(context.parsed.y);
                                } else {
                                    label += context.parsed.y.toLocaleString('pt-BR');
                                }
                            }
                            return label;
                        }
                    }
                },
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return value.toLocaleString('pt-BR');
                        }
                    }
                }
            }
        },

        // Cores padrão para gráficos
        colors: {
            primary: '#4e73df',
            success: '#1cc88a',
            warning: '#f6c23e',
            danger: '#e74a3b',
            info: '#36b9cc',
            secondary: '#858796'
        }
    },

    // Instâncias dos gráficos
    charts: {},

    // Inicializar dashboard
    init: function() {
        this.initCharts();
        this.loadData();
        this.setupEventListeners();
    },

    // Inicializar gráficos
    initCharts: function() {
        // Gráfico de Movimentações por Período
        this.initMovimentacoesChart();
        
        // Gráfico de Produtos por Categoria
        this.initCategoriasChart();
        
        // Gráfico de Valor do Estoque
        this.initValorEstoqueChart();
        
        // Gráfico de Produtos com Baixo Estoque
        this.initBaixoEstoqueChart();
    },

    // Gráfico de Movimentações por Período
    initMovimentacoesChart: function() {
        const ctx = document.getElementById('movimentacoesChart');
        if (!ctx) return;

        this.charts.movimentacoes = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Entradas',
                    data: [],
                    borderColor: this.chartConfigs.colors.success,
                    backgroundColor: this.chartConfigs.colors.success + '20',
                    tension: 0.1
                }, {
                    label: 'Saídas',
                    data: [],
                    borderColor: this.chartConfigs.colors.danger,
                    backgroundColor: this.chartConfigs.colors.danger + '20',
                    tension: 0.1
                }]
            },
            options: {
                ...this.chartConfigs.defaultOptions,
                plugins: {
                    ...this.chartConfigs.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Movimentações de Estoque - Últimos 30 dias'
                    }
                }
            }
        });
    },

    // Gráfico de Produtos por Categoria
    initCategoriasChart: function() {
        const ctx = document.getElementById('categoriasChart');
        if (!ctx) return;

        this.charts.categorias = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: [],
                datasets: [{
                    data: [],
                    backgroundColor: [
                        this.chartConfigs.colors.primary,
                        this.chartConfigs.colors.success,
                        this.chartConfigs.colors.warning,
                        this.chartConfigs.colors.danger,
                        this.chartConfigs.colors.info,
                        this.chartConfigs.colors.secondary
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                    },
                    title: {
                        display: true,
                        text: 'Produtos por Categoria'
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const label = context.label || '';
                                const value = context.parsed;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = ((value / total) * 100).toFixed(1);
                                return `${label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });
    },

    // Gráfico de Valor do Estoque
    initValorEstoqueChart: function() {
        const ctx = document.getElementById('valorEstoqueChart');
        if (!ctx) return;

        this.charts.valorEstoque = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: [],
                datasets: [{
                    label: 'Valor do Estoque',
                    data: [],
                    backgroundColor: this.chartConfigs.colors.primary,
                    borderColor: this.chartConfigs.colors.primary,
                    borderWidth: 1,
                    type: 'currency'
                }]
            },
            options: {
                ...this.chartConfigs.defaultOptions,
                plugins: {
                    ...this.chartConfigs.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Valor do Estoque por Categoria'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return MecTecERP.utils.formatCurrency(value);
                            }
                        }
                    }
                }
            }
        });
    },

    // Gráfico de Produtos com Baixo Estoque
    initBaixoEstoqueChart: function() {
        const ctx = document.getElementById('baixoEstoqueChart');
        if (!ctx) return;

        this.charts.baixoEstoque = new Chart(ctx, {
            type: 'horizontalBar',
            data: {
                labels: [],
                datasets: [{
                    label: 'Estoque Atual',
                    data: [],
                    backgroundColor: this.chartConfigs.colors.warning,
                    borderColor: this.chartConfigs.colors.warning,
                    borderWidth: 1
                }, {
                    label: 'Estoque Mínimo',
                    data: [],
                    backgroundColor: this.chartConfigs.colors.danger,
                    borderColor: this.chartConfigs.colors.danger,
                    borderWidth: 1
                }]
            },
            options: {
                ...this.chartConfigs.defaultOptions,
                indexAxis: 'y',
                plugins: {
                    ...this.chartConfigs.defaultOptions.plugins,
                    title: {
                        display: true,
                        text: 'Produtos com Baixo Estoque'
                    }
                }
            }
        });
    },

    // Carregar dados do dashboard
    loadData: function() {
        // Simular dados para demonstração
        // Em produção, estes dados viriam do backend via API
        
        // Dados de movimentações
        const movimentacoesData = this.generateMovimentacoesData();
        this.updateMovimentacoesChart(movimentacoesData);
        
        // Dados de categorias
        const categoriasData = this.generateCategoriasData();
        this.updateCategoriasChart(categoriasData);
        
        // Dados de valor do estoque
        const valorEstoqueData = this.generateValorEstoqueData();
        this.updateValorEstoqueChart(valorEstoqueData);
        
        // Dados de baixo estoque
        const baixoEstoqueData = this.generateBaixoEstoqueData();
        this.updateBaixoEstoqueChart(baixoEstoqueData);
    },

    // Gerar dados de movimentações (simulação)
    generateMovimentacoesData: function() {
        const labels = [];
        const entradas = [];
        const saidas = [];
        
        for (let i = 29; i >= 0; i--) {
            const date = new Date();
            date.setDate(date.getDate() - i);
            labels.push(date.toLocaleDateString('pt-BR', { day: '2-digit', month: '2-digit' }));
            
            entradas.push(Math.floor(Math.random() * 50) + 10);
            saidas.push(Math.floor(Math.random() * 40) + 5);
        }
        
        return { labels, entradas, saidas };
    },

    // Gerar dados de categorias (simulação)
    generateCategoriasData: function() {
        return {
            labels: ['Ferramentas', 'Peças', 'Equipamentos', 'Consumíveis', 'Acessórios'],
            data: [45, 32, 28, 15, 12]
        };
    },

    // Gerar dados de valor do estoque (simulação)
    generateValorEstoqueData: function() {
        return {
            labels: ['Ferramentas', 'Peças', 'Equipamentos', 'Consumíveis', 'Acessórios'],
            data: [125000, 89000, 156000, 34000, 67000]
        };
    },

    // Gerar dados de baixo estoque (simulação)
    generateBaixoEstoqueData: function() {
        return {
            labels: ['Chave Phillips', 'Parafuso M6', 'Óleo Lubrificante', 'Filtro de Ar', 'Correia'],
            estoqueAtual: [3, 8, 2, 5, 1],
            estoqueMinimo: [10, 20, 15, 12, 8]
        };
    },

    // Atualizar gráfico de movimentações
    updateMovimentacoesChart: function(data) {
        if (!this.charts.movimentacoes) return;
        
        this.charts.movimentacoes.data.labels = data.labels;
        this.charts.movimentacoes.data.datasets[0].data = data.entradas;
        this.charts.movimentacoes.data.datasets[1].data = data.saidas;
        this.charts.movimentacoes.update();
    },

    // Atualizar gráfico de categorias
    updateCategoriasChart: function(data) {
        if (!this.charts.categorias) return;
        
        this.charts.categorias.data.labels = data.labels;
        this.charts.categorias.data.datasets[0].data = data.data;
        this.charts.categorias.update();
    },

    // Atualizar gráfico de valor do estoque
    updateValorEstoqueChart: function(data) {
        if (!this.charts.valorEstoque) return;
        
        this.charts.valorEstoque.data.labels = data.labels;
        this.charts.valorEstoque.data.datasets[0].data = data.data;
        this.charts.valorEstoque.update();
    },

    // Atualizar gráfico de baixo estoque
    updateBaixoEstoqueChart: function(data) {
        if (!this.charts.baixoEstoque) return;
        
        this.charts.baixoEstoque.data.labels = data.labels;
        this.charts.baixoEstoque.data.datasets[0].data = data.estoqueAtual;
        this.charts.baixoEstoque.data.datasets[1].data = data.estoqueMinimo;
        this.charts.baixoEstoque.update();
    },

    // Configurar event listeners
    setupEventListeners: function() {
        // Botão de atualizar dados
        const refreshBtn = document.getElementById('refreshDashboard');
        if (refreshBtn) {
            refreshBtn.addEventListener('click', () => {
                this.loadData();
            });
        }
        
        // Filtros de período
        const periodFilter = document.getElementById('periodFilter');
        if (periodFilter) {
            periodFilter.addEventListener('change', (e) => {
                this.filterByPeriod(e.target.value);
            });
        }
    },

    // Filtrar por período
    filterByPeriod: function(period) {
        // Implementar filtro por período
        console.log('Filtrando por período:', period);
        // Recarregar dados com o novo período
        this.loadData();
    },

    // Destruir gráficos
    destroy: function() {
        Object.values(this.charts).forEach(chart => {
            if (chart) {
                chart.destroy();
            }
        });
        this.charts = {};
    },

    // Redimensionar gráficos
    resize: function() {
        Object.values(this.charts).forEach(chart => {
            if (chart) {
                chart.resize();
            }
        });
    }
};

// Inicializar quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function() {
    // Verificar se estamos na página do dashboard
    if (document.getElementById('dashboardEstoque')) {
        DashboardEstoque.init();
    }
});

// Redimensionar gráficos quando a janela for redimensionada
window.addEventListener('resize', function() {
    DashboardEstoque.resize();
});

// Funções para interoperabilidade com Blazor
window.initDashboardEstoque = function() {
    DashboardEstoque.init();
};

window.updateDashboardData = function(data) {
    if (data.movimentacoes) {
        DashboardEstoque.updateMovimentacoesChart(data.movimentacoes);
    }
    if (data.categorias) {
        DashboardEstoque.updateCategoriasChart(data.categorias);
    }
    if (data.valorEstoque) {
        DashboardEstoque.updateValorEstoqueChart(data.valorEstoque);
    }
    if (data.baixoEstoque) {
        DashboardEstoque.updateBaixoEstoqueChart(data.baixoEstoque);
    }
};

window.destroyDashboardCharts = function() {
    DashboardEstoque.destroy();
};