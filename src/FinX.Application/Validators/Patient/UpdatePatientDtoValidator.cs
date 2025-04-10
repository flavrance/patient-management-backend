using System;
using FluentValidation;
using FinX.Application.DTOs.Patient;

namespace FinX.Application.Validators.Patient;

public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required")
            .Length(11).WithMessage("CPF must be 11 digits")
            .Matches(@"^\d+$").WithMessage("CPF must contain only digits")
            .Must(BeAValidCpf).WithMessage("CPF is invalid");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now).WithMessage("Date of birth cannot be in the future")
            .Must(BeAValidAge).WithMessage("Patient must be between 0 and 120 years old");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .MaximumLength(20).WithMessage("Gender cannot exceed 20 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not in a valid format")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only digits");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters");

        RuleFor(x => x.MedicalHistory)
            .MaximumLength(2000).WithMessage("Medical history cannot exceed 2000 characters");
    }
    
    private bool BeAValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        return age >= 0 && age <= 120;
    }

    private bool BeAValidCpf(string cpf)
    {
        if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
            return false;

        // Check if all digits are the same
        bool allDigitsSame = true;
        for (int i = 1; i < cpf.Length; i++)
        {
            if (cpf[i] != cpf[0])
            {
                allDigitsSame = false;
                break;
            }
        }
        if (allDigitsSame) return false;

        // First verification digit
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        int remainder = sum % 11;
        int verificationDigit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != verificationDigit1)
            return false;

        // Second verification digit
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        int verificationDigit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == verificationDigit2;
    }
} 