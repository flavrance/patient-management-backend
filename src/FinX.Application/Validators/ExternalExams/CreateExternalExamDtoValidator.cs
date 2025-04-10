using FluentValidation;
using FinX.Application.DTOs.ExternalExams;

namespace FinX.Application.Validators.ExternalExams
{
    public class CreateExternalExamDtoValidator : AbstractValidator<CreateExternalExamDto>
    {
        public CreateExternalExamDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("O ID do paciente é obrigatório");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome do exame é obrigatório")
                .MaximumLength(100)
                .WithMessage("O nome do exame não pode exceder 100 caracteres");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("A data do exame é obrigatória")
                .LessThanOrEqualTo(System.DateTime.UtcNow)
                .WithMessage("A data do exame não pode ser no futuro");

            RuleFor(x => x.Laboratory)
                .NotEmpty()
                .WithMessage("O laboratório é obrigatório")
                .MaximumLength(100)
                .WithMessage("O nome do laboratório não pode exceder 100 caracteres");

            RuleFor(x => x.Result)
                .NotEmpty()
                .WithMessage("O resultado do exame é obrigatório")
                .MaximumLength(1000)
                .WithMessage("O resultado do exame não pode exceder 1000 caracteres");
        }
    }
} 