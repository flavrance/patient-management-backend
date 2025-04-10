using AutoMapper;
using FinX.Domain.Entities;
using FinX.Application.DTOs.Appointment;
using FinX.Application.DTOs.Patient;

namespace FinX.Application.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient != null ? new PatientDto
                {
                    Id = src.Patient.Id,
                    FirstName = src.Patient.FirstName,
                    LastName = src.Patient.LastName
                } : null));

            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>();
        }
    }
} 