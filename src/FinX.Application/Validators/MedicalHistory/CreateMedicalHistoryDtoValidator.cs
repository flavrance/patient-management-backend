using FluentValidation;
using FinX.Application.DTOs.MedicalHistory;

namespace FinX.Application.Validators.MedicalHistory
{
    public class CreateMedicalHistoryDtoValidator : AbstractValidator<CreateMedicalHistoryDto>
    {
        public CreateMedicalHistoryDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("Patient ID is required");

            RuleFor(x => x.Diagnosis)
                .NotEmpty()
                .WithMessage("Diagnosis is required")
                .MaximumLength(500)
                .WithMessage("Diagnosis must not exceed 500 characters");

            RuleFor(x => x.Exams)
                .NotEmpty()
                .WithMessage("Exams are required")
                .MaximumLength(1000)
                .WithMessage("Exams must not exceed 1000 characters");

            RuleFor(x => x.Prescriptions)
                .NotEmpty()
                .WithMessage("Prescriptions are required")
                .MaximumLength(1000)
                .WithMessage("Prescriptions must not exceed 1000 characters");
        }
    }
} 