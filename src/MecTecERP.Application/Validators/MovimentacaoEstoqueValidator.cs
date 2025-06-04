using FluentValidation;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums;

namespace MecTecERP.Application.Validators;

public class MovimentacaoEstoqueCreateDtoValidator : AbstractValidator<MovimentacaoEstoqueCreateDto>
{
    public MovimentacaoEstoqueCreateDtoValidator()
    {
        RuleFor(x => x.ProdutoId)
            .GreaterThan(0).WithMessage("O produto é obrigatório");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de movimentação inválido");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero")
            .LessThan(1000000).WithMessage("A quantidade deve ser menor que 1.000.000");

        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0).WithMessage("O valor unitário deve ser maior que zero")
            .LessThan(1000000).WithMessage("O valor unitário deve ser menor que R$ 1.000.000,00");

        RuleFor(x => x.DataMovimentacao)
            .NotEmpty().WithMessage("A data da movimentação é obrigatória")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("A data da movimentação não pode ser futura");

        RuleFor(x => x.Documento)
            .MaximumLength(100).WithMessage("O documento deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Documento));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        RuleFor(x => x.InventarioId)
            .GreaterThan(0).WithMessage("ID do inventário inválido")
            .When(x => x.InventarioId.HasValue);

        // Validação específica para movimentações de inventário
        RuleFor(x => x.InventarioId)
            .NotNull().WithMessage("O inventário é obrigatório para movimentações de inventário")
            .When(x => x.Tipo == TipoMovimentacaoEstoque.Inventario);

        RuleFor(x => x.InventarioId)
            .Null().WithMessage("O inventário deve ser nulo para movimentações que não são de inventário")
            .When(x => x.Tipo != TipoMovimentacaoEstoque.Inventario);
    }
}

public class MovimentacaoEstoqueUpdateDtoValidator : AbstractValidator<MovimentacaoEstoqueUpdateDto>
{
    public MovimentacaoEstoqueUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.ProdutoId)
            .GreaterThan(0).WithMessage("O produto é obrigatório");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de movimentação inválido");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero")
            .LessThan(1000000).WithMessage("A quantidade deve ser menor que 1.000.000");

        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0).WithMessage("O valor unitário deve ser maior que zero")
            .LessThan(1000000).WithMessage("O valor unitário deve ser menor que R$ 1.000.000,00");

        RuleFor(x => x.DataMovimentacao)
            .NotEmpty().WithMessage("A data da movimentação é obrigatória")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("A data da movimentação não pode ser futura");

        RuleFor(x => x.Documento)
            .MaximumLength(100).WithMessage("O documento deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Documento));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));

        RuleFor(x => x.InventarioId)
            .GreaterThan(0).WithMessage("ID do inventário inválido")
            .When(x => x.InventarioId.HasValue);

        // Validação específica para movimentações de inventário
        RuleFor(x => x.InventarioId)
            .NotNull().WithMessage("O inventário é obrigatório para movimentações de inventário")
            .When(x => x.Tipo == TipoMovimentacaoEstoque.Inventario);

        RuleFor(x => x.InventarioId)
            .Null().WithMessage("O inventário deve ser nulo para movimentações que não são de inventário")
            .When(x => x.Tipo != TipoMovimentacaoEstoque.Inventario);
    }
}

public class MovimentacaoEstoqueFiltroValidator : AbstractValidator<MovimentacaoEstoqueFiltroDto>
{
    public MovimentacaoEstoqueFiltroValidator()
    {
        RuleFor(x => x.DataInicio)
            .LessThanOrEqualTo(x => x.DataFim)
            .WithMessage("A data de início deve ser menor ou igual à data de fim")
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue);

        RuleFor(x => x.DataFim)
            .LessThanOrEqualTo(DateTime.Now.Date.AddDays(1))
            .WithMessage("A data de fim não pode ser futura")
            .When(x => x.DataFim.HasValue);

        RuleFor(x => x.ProdutoId)
            .GreaterThan(0).WithMessage("ID do produto inválido")
            .When(x => x.ProdutoId.HasValue);

        RuleFor(x => x.InventarioId)
            .GreaterThan(0).WithMessage("ID do inventário inválido")
            .When(x => x.InventarioId.HasValue);

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de movimentação inválido")
            .When(x => x.Tipo.HasValue);

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
            "DataMovimentacao", "ProdutoNome", "Tipo", "Quantidade", 
            "ValorUnitario", "ValorTotal", "Documento"
        };

        return validFields.Contains(sortField, StringComparer.OrdinalIgnoreCase);
    }
}