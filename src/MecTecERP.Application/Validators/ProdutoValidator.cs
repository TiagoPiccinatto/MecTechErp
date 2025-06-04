using FluentValidation;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Validators;

public class ProdutoCreateDtoValidator : AbstractValidator<ProdutoCreateDto>
{
    public ProdutoCreateDtoValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código do produto é obrigatório")
            .Length(1, 50).WithMessage("O código deve ter entre 1 e 50 caracteres")
            .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("O código deve conter apenas letras, números, hífens e underscores");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório")
            .Length(2, 200).WithMessage("O nome deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("A categoria é obrigatória");

        RuleFor(x => x.FornecedorId)
            .GreaterThan(0).WithMessage("O fornecedor é obrigatório");

        RuleFor(x => x.UnidadeMedida)
            .NotEmpty().WithMessage("A unidade de medida é obrigatória")
            .Length(1, 10).WithMessage("A unidade de medida deve ter entre 1 e 10 caracteres");

        RuleFor(x => x.PrecoCompra)
            .GreaterThan(0).WithMessage("O preço de compra deve ser maior que zero")
            .LessThan(1000000).WithMessage("O preço de compra deve ser menor que R$ 1.000.000,00");

        RuleFor(x => x.PrecoVenda)
            .GreaterThan(0).WithMessage("O preço de venda deve ser maior que zero")
            .LessThan(1000000).WithMessage("O preço de venda deve ser menor que R$ 1.000.000,00")
            .GreaterThanOrEqualTo(x => x.PrecoCompra).WithMessage("O preço de venda deve ser maior ou igual ao preço de compra");

        RuleFor(x => x.EstoqueMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque mínimo deve ser maior ou igual a zero")
            .LessThan(1000000).WithMessage("O estoque mínimo deve ser menor que 1.000.000");

        RuleFor(x => x.EstoqueMaximo)
            .GreaterThan(0).WithMessage("O estoque máximo deve ser maior que zero")
            .LessThan(1000000).WithMessage("O estoque máximo deve ser menor que 1.000.000")
            .GreaterThan(x => x.EstoqueMinimo).WithMessage("O estoque máximo deve ser maior que o estoque mínimo");

        RuleFor(x => x.Localizacao)
            .MaximumLength(100).WithMessage("A localização deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Localizacao));

        RuleFor(x => x.CodigoBarras)
            .MaximumLength(50).WithMessage("O código de barras deve ter no máximo 50 caracteres")
            .Matches(@"^[0-9]+$").WithMessage("O código de barras deve conter apenas números")
            .When(x => !string.IsNullOrEmpty(x.CodigoBarras));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }
}

public class ProdutoUpdateDtoValidator : AbstractValidator<ProdutoUpdateDto>
{
    public ProdutoUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código do produto é obrigatório")
            .Length(1, 50).WithMessage("O código deve ter entre 1 e 50 caracteres")
            .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("O código deve conter apenas letras, números, hífens e underscores");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do produto é obrigatório")
            .Length(2, 200).WithMessage("O nome deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("A categoria é obrigatória");

        RuleFor(x => x.FornecedorId)
            .GreaterThan(0).WithMessage("O fornecedor é obrigatório");

        RuleFor(x => x.UnidadeMedida)
            .NotEmpty().WithMessage("A unidade de medida é obrigatória")
            .Length(1, 10).WithMessage("A unidade de medida deve ter entre 1 e 10 caracteres");

        RuleFor(x => x.PrecoCompra)
            .GreaterThan(0).WithMessage("O preço de compra deve ser maior que zero")
            .LessThan(1000000).WithMessage("O preço de compra deve ser menor que R$ 1.000.000,00");

        RuleFor(x => x.PrecoVenda)
            .GreaterThan(0).WithMessage("O preço de venda deve ser maior que zero")
            .LessThan(1000000).WithMessage("O preço de venda deve ser menor que R$ 1.000.000,00")
            .GreaterThanOrEqualTo(x => x.PrecoCompra).WithMessage("O preço de venda deve ser maior ou igual ao preço de compra");

        RuleFor(x => x.EstoqueMinimo)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque mínimo deve ser maior ou igual a zero")
            .LessThan(1000000).WithMessage("O estoque mínimo deve ser menor que 1.000.000");

        RuleFor(x => x.EstoqueMaximo)
            .GreaterThan(0).WithMessage("O estoque máximo deve ser maior que zero")
            .LessThan(1000000).WithMessage("O estoque máximo deve ser menor que 1.000.000")
            .GreaterThan(x => x.EstoqueMinimo).WithMessage("O estoque máximo deve ser maior que o estoque mínimo");

        RuleFor(x => x.Localizacao)
            .MaximumLength(100).WithMessage("A localização deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Localizacao));

        RuleFor(x => x.CodigoBarras)
            .MaximumLength(50).WithMessage("O código de barras deve ter no máximo 50 caracteres")
            .Matches(@"^[0-9]+$").WithMessage("O código de barras deve conter apenas números")
            .When(x => !string.IsNullOrEmpty(x.CodigoBarras));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }
}