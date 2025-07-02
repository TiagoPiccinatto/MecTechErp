using MecTecERP.Infrastructure.Data; // Para IDbConnectionFactory, SqlServerConnectionFactory
using MecTecERP.Application.Interfaces; // Para Services
using MecTecERP.Application.Services;  // Para Services
using MecTecERP.Domain.Interfaces;    // Para Repositories
using MecTecERP.Infrastructure.Repositories; // Para Repositories
using Blazored.Toast;
using Blazored.Modal;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// AutoMapper - AddApplication também registra AutoMapper, mas manter aqui não prejudica e garante que todos os assemblies sejam varridos.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Blazored Components
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

using MecTecERP.Application; // Adicionado para AddApplication
using MecTecERP.Infrastructure; // Adicionado para AddInfrastructure

// Application Services
builder.Services.AddApplication();

// Infrastructure Services (inclui Repositories e DbConnectionFactory)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// A criação do banco de dados e tabelas é feita por scripts SQL manuais com Dapper.
// Não há EnsureCreated() automático.

app.Run();