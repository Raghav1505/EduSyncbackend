using AutoMapper;
using EduSync.Models;
using EduSync.DTOs;

namespace EduSync.MappingProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>();

            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(_ => Guid.NewGuid()));

            CreateMap<UpdateCourseDto, Course>()
                .ForAllMembers(opt => opt.Condition((src, _, srcVal) => srcVal != null));
        }
    }
}
