using AutoMapper;
using FinX.Domain.Entities;
using FinX.Application.DTOs.Patient;

namespace FinX.Application.Mappings;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientDto>();
        
        CreateMap<CreatePatientDto, Patient>()
            .ConstructUsing(dto => new Patient(
                dto.FirstName,
                dto.LastName,
                dto.Cpf,
                dto.DateOfBirth,
                dto.Gender,
                dto.Email,
                dto.PhoneNumber,
                dto.Address,
                dto.MedicalHistory));

        CreateMap<UpdatePatientDto, Patient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
} 