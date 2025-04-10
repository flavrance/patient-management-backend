using System;
using FluentValidation;
using FinX.Application.DTOs.Appointment;

namespace FinX.Application.Validators.Appointment
{
    public class UpdateAppointmentDtoValidator : AbstractValidator<UpdateAppointmentDto>
    {
        public UpdateAppointmentDtoValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty()
                .WithMessage("Patient ID is required");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty()
                .WithMessage("Appointment date is required");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("Duration is required")
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(480) // 8 hours max
                .WithMessage("Duration cannot exceed 8 hours (480 minutes)");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required")
                .MaximumLength(50)
                .WithMessage("Status cannot exceed 50 characters")
                .Must(status => status == "Scheduled" || 
                               status == "Confirmed" || 
                               status == "Completed" || 
                               status == "Cancelled" || 
                               status == "No-Show")
                .WithMessage("Status must be one of: Scheduled, Confirmed, Completed, Cancelled, No-Show");

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