using AutoMapper;
using EduSync.Models;
using EduSync.DTOs;

namespace EduSync.MappingProfiles
{
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            // Map from entity to DTO
            CreateMap<Assessment, AssessmentDto>();

            // Map from Create DTO to entity, generate new GUID
            CreateMap<CreateAssessmentDto, Assessment>()
                .ForMember(dest => dest.AssessmentId,
                           opt => opt.MapFrom(_ => Guid.NewGuid()));

            // Map from Update DTO to entity, but only non-null props
            CreateMap<UpdateAssessmentDto, Assessment>()
                .ForAllMembers(opt => opt.Condition((src, _, srcVal) => srcVal != null));
        }
    }
}
