using FluentValidation;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Validators;

public class InventarioCreateDtoValidator : AbstractValidator<InventarioCreateDto>
{
    public InventarioCreateDtoValidator()
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do inventário é obrigatória")
            .Length(2, 200).WithMessage("A descrição deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.DataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória")
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("A data de início não pode ser anterior a hoje");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("ID da categoria inválido")
            .When(x => x.CategoriaId.HasValue);

        RuleFor(x => x.FornecedorId)
            .GreaterThan(0).WithMessage("ID do fornecedor inválido")
            .When(x => x.FornecedorId.HasValue);

        RuleFor(x => x.ProdutoIds)
            .Must(ids => ids.All(id => id > 0)).WithMessage("Todos os IDs de produtos devem ser válidos")
            .When(x => x.ProdutoIds.Any());

        // Validação condicional: se não incluir todos os produtos, deve ter pelo menos um filtro
        RuleFor(x => x)
            .Must(x => x.IncluirTodosProdutos || x.ProdutoIds.Any() || x.CategoriaId.HasValue || x.FornecedorId.HasValue)
            .WithMessage("Deve incluir todos os produtos ou especificar produtos/categoria/fornecedor específicos");
    }
}

public class InventarioUpdateDtoValidator : AbstractValidator<InventarioUpdateDto>
{
    public InventarioUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do inventário é obrigatória")
            .Length(2, 200).WithMessage("A descrição deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.DataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória");

        RuleFor(x => x.DataFim)
            .GreaterThan(x => x.DataInicio).WithMessage("A data de fim deve ser posterior à data de início")
            .When(x => x.DataFim.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status do inventário inválido");

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        // Validação de status: se finalizado, deve ter data de fim
        RuleFor(x => x.DataFim)
            .NotNull().WithMessage("A data de fim é obrigatória para inventários finalizados")
            .When(x => x.Status == StatusInventario.Finalizado);

        // Validação de status: se cancelado ou finalizado, deve ter data de fim
        RuleFor(x => x.DataFim)
            .NotNull().WithMessage("A data de fim é obrigatória para inventários cancelados")
            .When(x => x.Status == StatusInventario.Cancelado);
    }
}

public class InventarioItemUpdateDtoValidator : AbstractValidator<InventarioItemUpdateDto>
{
    public InventarioItemUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.EstoqueContado)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque contado deve ser maior ou igual a zero")
            .LessThan(1000000).WithMessage("O estoque contado deve ser menor que 1.000.000");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observações devem ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }
}

public class InventarioFiltroValidator : AbstractValidator<InventarioFiltroDto>
{
    public InventarioFiltroValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status do inventário inválido")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.DataInicio)
            .LessThanOrEqualTo(x => x.DataFim)
            .WithMessage("A data de início deve ser menor ou igual à data de fim")
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue);

        RuleFor(x => x.DataFim)
            .LessThanOrEqualTo(DateTime.Now.Date.AddDays(1))
            .WithMessage("A data de fim não pode ser futura")
            .When(x => x.DataFim.HasValue);

        RuleFor(x => x.Descricao)
            .MaximumLength(200).WithMessage("A descrição deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("A página deve ser maior que zero");

        RuleFor(x => x.ItensPorPagina)
            .InclusiveBetween(1, 100).WithMessage("Os itens por página devem estar entre 1 e 100");

        RuleFor(x => x.OrdenarPor)
            .Must(BeValidSortField).WithMessage("Campo de ordenação inválido")
            .When(x => !string.IsNullOrEmpty(x.OrdenarPor));
    }

    private bool BeValidSortField(string? sortField)
    {
        if (string.IsNullOrEmpty(sortField))
            return true;

        var validFields = new[] 
        {
            "DataInicio", "DataFim", "Descricao", "Status", 
            "TotalItens", "PercentualConcluido", "DataCriacao"
        };

        return validFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
    }
}