using AutoMapper;
using FinX.Domain.Entities;
using FinX.Application.DTOs.MedicalHistory;

namespace FinX.Application.Mappings;

public class MedicalHistoryProfile : Profile
{
    public MedicalHistoryProfile()
    {
        CreateMap<MedicalHistory, MedicalHistoryDto>();
        
        CreateMap<CreateMedicalHistoryDto, MedicalHistory>()
            .ConstructUsing(dto => new MedicalHistory(
                dto.PatientId,
                dto.Diagnosis,
                dto.Exams,
                dto.Prescriptions));

        CreateMap<UpdateMedicalHistoryDto, MedicalHistory>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
} 