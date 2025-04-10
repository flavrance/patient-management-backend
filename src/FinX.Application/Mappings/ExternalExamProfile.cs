using AutoMapper;
using FinX.Domain.Entities;
using FinX.Application.DTOs.ExternalExams;

namespace FinX.Application.Mappings;

public class ExternalExamProfile : Profile
{
    public ExternalExamProfile()
    {
        CreateMap<ExternalExam, ExternalExamDto>();
        
        CreateMap<CreateExternalExamDto, ExternalExam>()
            .ConstructUsing(dto => new ExternalExam(
                dto.PatientId,
                dto.Name,
                dto.Date,
                dto.Laboratory,
                dto.Result));

        CreateMap<UpdateExternalExamDto, ExternalExam>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.Patient, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
} 