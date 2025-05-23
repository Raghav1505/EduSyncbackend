using AutoMapper;
using EduSync.Models;
using EduSync.DTOs;

namespace EduSync.MappingProfiles
{
    public class ResultProfile : Profile
    {
        public ResultProfile()
        {
            CreateMap<Result, ResultDto>();

            CreateMap<CreateResultDto, Result>()
                .ForMember(dest => dest.ResultId, opt => opt.MapFrom(_ => Guid.NewGuid()));

            CreateMap<UpdateResultDto, Result>()
                .ForAllMembers(opt => opt.Condition((src, _, srcVal) => srcVal != null));
        }
    }
}
