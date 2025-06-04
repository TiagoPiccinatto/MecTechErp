using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;
using MecTecERP.Infrastructure.Repositories;

namespace MecTecERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do Dapper
        services.AddSingleton<IDbConnectionFactory>(provider => 
            new SqlServerConnectionFactory(configuration));

        // Registro dos repositórios
        // services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>)); // BaseRepository é abstrato
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        // services.AddScoped<IItemOrdemServicoRepository, ItemOrdemServicoRepository>(); // Removido - não implementado
        // services.AddScoped<IServicoRepository, ServicoRepository>(); // Removido - não implementado
        // services.AddScoped<IUsuarioRepository, UsuarioRepository>(); // Removido - não implementado

        return services;
    }
}