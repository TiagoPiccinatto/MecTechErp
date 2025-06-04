using FluentValidation;
using MecTecERP.Application.DTOs;
using System.Text.RegularExpressions;

namespace MecTecERP.Application.Validators;

public class FornecedorCreateDtoValidator : AbstractValidator<FornecedorCreateDto>
{
    public FornecedorCreateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório")
            .Length(2, 200).WithMessage("O nome deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório")
            .Must(ValidarCnpj).WithMessage("CNPJ inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("O email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório")
            .Matches(@"^\(?\d{2}\)?[\s-]?\d{4,5}[\s-]?\d{4}$").WithMessage("Telefone inválido");

        RuleFor(x => x.Endereco)
            .MaximumLength(300).WithMessage("O endereço deve ter no máximo 300 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Endereco));

        RuleFor(x => x.Cidade)
            .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Cidade));

        RuleFor(x => x.Estado)
            .Length(2).WithMessage("O estado deve ter 2 caracteres")
            .Matches(@"^[A-Z]{2}$").WithMessage("Estado inválido (use sigla em maiúsculo)")
            .When(x => !string.IsNullOrEmpty(x.Estado));

        RuleFor(x => x.Cep)
            .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido")
            .When(x => !string.IsNullOrEmpty(x.Cep));

        RuleFor(x => x.Contato)
            .MaximumLength(100).WithMessage("O contato deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Contato));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }

    private bool ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = Regex.Replace(cnpj, @"[^\d]", "");

        // Verifica se tem 14 dígitos
        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Validação do primeiro dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
        
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        
        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        // Validação do segundo dígito verificador
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
        
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        
        return int.Parse(cnpj[13].ToString()) == digito2;
    }
}

public class FornecedorUpdateDtoValidator : AbstractValidator<FornecedorUpdateDto>
{
    public FornecedorUpdateDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID inválido");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório")
            .Length(2, 200).WithMessage("O nome deve ter entre 2 e 200 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório")
            .Must(ValidarCnpj).WithMessage("CNPJ inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("O email deve ter no máximo 200 caracteres");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório")
            .Matches(@"^\(?\d{2}\)?[\s-]?\d{4,5}[\s-]?\d{4}$").WithMessage("Telefone inválido");

        RuleFor(x => x.Endereco)
            .MaximumLength(300).WithMessage("O endereço deve ter no máximo 300 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Endereco));

        RuleFor(x => x.Cidade)
            .MaximumLength(100).WithMessage("A cidade deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Cidade));

        RuleFor(x => x.Estado)
            .Length(2).WithMessage("O estado deve ter 2 caracteres")
            .Matches(@"^[A-Z]{2}$").WithMessage("Estado inválido (use sigla em maiúsculo)")
            .When(x => !string.IsNullOrEmpty(x.Estado));

        RuleFor(x => x.Cep)
            .Matches(@"^\d{5}-?\d{3}$").WithMessage("CEP inválido")
            .When(x => !string.IsNullOrEmpty(x.Cep));

        RuleFor(x => x.Contato)
            .MaximumLength(100).WithMessage("O contato deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Contato));

        RuleFor(x => x.Observacoes)
            .MaximumLength(1000).WithMessage("As observações devem ter no máximo 1000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Observacoes));
    }

    private bool ValidarCnpj(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return false;

        // Remove caracteres não numéricos
        cnpj = Regex.Replace(cnpj, @"[^\d]", "");

        // Verifica se tem 14 dígitos
        if (cnpj.Length != 14)
            return false;

        // Verifica se todos os dígitos são iguais
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Validação do primeiro dígito verificador
        int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 12; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicador1[i];
        
        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;
        
        if (int.Parse(cnpj[12].ToString()) != digito1)
            return false;

        // Validação do segundo dígito verificador
        int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicador2[i];
        
        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;
        
        return int.Parse(cnpj[13].ToString()) == digito2;
    }
}