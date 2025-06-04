using Microsoft.Extensions.DependencyInjection;
using MecTecERP.Application.Interfaces;
using MecTecERP.Application.Services;

namespace MecTecERP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(typeof(DependencyInjection));

            // Services
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
            services.AddScoped<IInventarioService, InventarioService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddScoped<IOrdemServicoService, OrdemServicoService>();

            return services;
        }
    }
}