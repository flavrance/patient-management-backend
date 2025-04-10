using System;
using FluentValidation;
using FinX.Application.DTOs.Appointment;

namespace FinX.Application.Validators.Appointment
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("Patient ID is required");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Appointment date is required")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Appointment date must be in the future");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("Duration is required")
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(480) // 8 hours max
                .WithMessage("Duration cannot exceed 8 hours (480 minutes)");

            RuleFor(x => x.Type)
                .NotEmpty()
                .WithMessage("Appointment type is required")
                .MaximumLength(100)
                .WithMessage("Appointment type cannot exceed 100 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters");
        }
    }
} 