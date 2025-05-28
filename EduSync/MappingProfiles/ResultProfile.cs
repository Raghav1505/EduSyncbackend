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
                .ForMember(dest => dest.ResultId, opt => opt.MapFrom(_ => Guid.NewGuid()))
    .ForMember(dest => dest.UserId, opt =>
         opt.MapFrom((src, _, _, ctx) =>
             // grab userId from the JWT claims
             Guid.Parse(ctx.Items["UserId"].ToString()!)
         ))
    .ForMember(dest => dest.Score, opt => opt.Ignore())  // we’ll calculate
    .ForMember(dest => dest.AttemptDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateResultDto, Result>()
                .ForAllMembers(opt => opt.Condition((src, _, srcVal) => srcVal != null));
        }
    }
}
