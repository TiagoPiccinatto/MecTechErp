using FluentValidation;
using MecTecERP.Application.DTOs;
using MecTecERP.Domain.Enums; // Para TipoPessoa

namespace MecTecERP.Application.Validators
{
    public class ClienteCreateDtoValidator : AbstractValidator<ClienteCreateDto>
    {
        public ClienteCreateDtoValidator()
        {
            RuleFor(x => x.NomeRazaoSocial)
                .NotEmpty().WithMessage("O Nome/Razão Social é obrigatório.")
                .MaximumLength(200).WithMessage("O Nome/Razão Social deve ter no máximo 200 caracteres.");

            RuleFor(x => x.CpfCnpj)
                .NotEmpty().WithMessage("O CPF/CNPJ é obrigatório.")
                .Length(11, 18).WithMessage("O CPF/CNPJ deve ter entre 11 e 18 caracteres.");
                // TODO: Adicionar validação customizada para formato e validade de CPF/CNPJ

            RuleFor(x => x.TipoPessoa)
                .NotEmpty().WithMessage("O Tipo de Pessoa é obrigatório.")
                .IsInEnum().WithMessage("Tipo de Pessoa inválido.");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email inválido.")
                .MaximumLength(100).WithMessage("O Email deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Telefone1)
                .MaximumLength(20).WithMessage("Telefone 1 deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Telefone1));

            RuleFor(x => x.Telefone2)
                .MaximumLength(20).WithMessage("Telefone 2 deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Telefone2));

            RuleFor(x => x.Cep)
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido. Formato esperado: XXXXX-XXX ou XXXXXXXX.")
                .When(x => !string.IsNullOrEmpty(x.Cep));

            RuleFor(x => x.Logradouro)
                .MaximumLength(200).WithMessage("Logradouro deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Logradouro));

            RuleFor(x => x.Numero)
                .MaximumLength(20).WithMessage("Número deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Numero));

            RuleFor(x => x.Bairro)
                .MaximumLength(100).WithMessage("Bairro deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Bairro));

            RuleFor(x => x.Cidade)
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Cidade));

            RuleFor(x => x.Uf)
                .Length(2).WithMessage("UF deve ter 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Uf));

            RuleFor(x => x.RgIe)
                .MaximumLength(20).WithMessage("RG/IE deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.RgIe));
        }
    }
}
