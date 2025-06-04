using FluentValidation;
using MecTecERP.Application.DTOs;

namespace MecTecERP.Application.Validators;

public class CategoriaCreateDtoValidator : AbstractValidator<CategoriaCreateDto>
{
    public CategoriaCreateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório")
            .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-_]+$").WithMessage("O nome contém caracteres inválidos");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}

public class CategoriaUpdateDtoValidator : AbstractValidator<CategoriaUpdateDto>
{
    public CategoriaUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório")
            .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres")
            .Matches(@"^[a-zA-ZÀ-ÿ0-9\s\-_]+$").WithMessage("O nome contém caracteres inválidos");

        RuleFor(x => x.Descricao)
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descricao));
    }
}